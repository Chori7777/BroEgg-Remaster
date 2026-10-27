using System.Collections.Generic;
using ED262C;
using UnityEngine;
using System;
// Objetivo detectado por la explosion. El BST los ordena por distancia al epicentro.
public class ExplosionTarget : IComparable
{
    public readonly IDamageable damageable;
    public readonly Collider2D collider;
    public readonly float distance;

    public ExplosionTarget(IDamageable damageable, Collider2D collider, float distance) //la clase que se ocupa de guardar el collider, distancia e interfaz del objeto golpeado
    {
        this.damageable = damageable;
        this.collider = collider;
        this.distance = distance;
    }

    public int CompareTo(object obj)
    {
        ExplosionTarget other = obj as ExplosionTarget;
        if (other == null) return 1;

        int byDistance = distance.CompareTo(other.distance);
        if (byDistance != 0) return byDistance;

        // El BST descarta los valores que dan CompareTo == 0. Si dos objetivos estan exactamente
        // a la misma distancia, desempatamos por id para no perder a ninguno.
        return collider.GetInstanceID().CompareTo(other.collider.GetInstanceID());
    }
}

// Una capa concentrica de la explosion.
[Serializable]
public class ExplosionLayer //Una clase de una capa invisible de explosion que tiene una distacia max, un multiplicador daño, y una fureza de empuje
{
    [Tooltip("Distancia maxima al epicentro que cubre esta capa")]
    public float maxDistance;

    [Tooltip("Multiplicador sobre el danio del enemigo (1 = danio completo)")]
    [Range(0f, 1f)] public float damageMultiplier;

    [Tooltip("Velocidad del empuje en unidades por segundo (0 = sin knockback)")]
    public float knockbackSpeed;

    public ExplosionLayer() { }

    public ExplosionLayer(float maxDistance, float damageMultiplier, float knockbackSpeed)
    {
        this.maxDistance = maxDistance;
        this.damageMultiplier = damageMultiplier;
        this.knockbackSpeed = knockbackSpeed;
    }
}

[RequireComponent(typeof(EnemyStats))]
public class KamikazeExplosion : MonoBehaviour
{
    [Header("Capas (de adentro hacia afuera)")] //un array de 3 capas
    [SerializeField] private ExplosionLayer[] layers =
    {
        new ExplosionLayer(1.5f, 1.00f, 18f), // Capa 1: muy cerca, daño maximo y knockback fuerte
        new ExplosionLayer(3.0f, 0.50f, 0f),  // Capa 2: distancia media, daño moderado
        new ExplosionLayer(4.5f, 0.25f, 0f)   // Capa 3: borde, daño bajo
    };

    [Header("Deteccion y knockback")]
    [SerializeField] private LayerMask targetLayers = ~0;
    [SerializeField] private float knockbackDuration = 0.25f;

    private EnemyStats stats;
    private bool exploded;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>(); //obtiene las stats

        // Las capas tienen que estar ordenadas de menor a mayor distancia
        Array.Sort(layers, (a, b) => a.maxDistance.CompareTo(b.maxDistance));
    }

    public void Explode()
    {
        // OnCollisionEnter2D puede dispararse mas de una vez antes de que el objeto se destruya
        if (exploded || layers == null || layers.Length == 0) return;
        exploded = true;

        Vector2 epicenter = transform.position; //el OverlapCircle arranca en el centro del enemigo

        // 1) Radio fisico: el borde de la ultima capa
        float radius = layers[layers.Length - 1].maxDistance; //la longitud del centro del circulo hasta el borde de la 3ra capa => 4.5f
        Collider2D[] hits = Physics2D.OverlapCircleAll(epicenter, radius, targetLayers); //Guardo todas las colisiones que esten en el capa deseada

        // 2) Un objeto puede tener varios colliders: nos quedamos con el mas cercano de cada uno
        Dictionary<IDamageable, ExplosionTarget> closestByTarget = new Dictionary<IDamageable, ExplosionTarget>();

        for (int i = 0; i < hits.Length; i++) //recorro todos los hits detectados, y por cada uno de ellos...
        {
            Collider2D hit = hits[i]; //lo guardo de una variable (más comodidad)

            if (hit.transform.IsChildOf(transform)) continue; //esto es para no dañar al enemigo, pero dsp se termina destruyendo, asi que no se, por las dudas no toco nada

            IDamageable damageable = hit.GetComponent<IDamageable>(); //guardo en una variable el componente adquirido de las colision
            if (damageable == null) continue; //IMPORTANTE: si algo dentro de los circulos no implementa la interfaz IDamageable, no lo daña

            float distance = Vector2.Distance(epicenter, hit.ClosestPoint(epicenter)); //calcula la distancia densde el centro hasta el objetivo hiteado

            if (closestByTarget.TryGetValue(damageable, out ExplosionTarget existing) && existing.distance <= distance) continue;
            closestByTarget[damageable] = new ExplosionTarget(damageable, hit, distance);
        }

        // Cada objetivo se inserta en el BST usando su distancia como criterio de orden
        BST<ExplosionTarget> targets = new BST<ExplosionTarget>();
        foreach (KeyValuePair<IDamageable, ExplosionTarget> pair in closestByTarget) //recorre el diccionario y guarda cada ExplosionTarget en el arbol
        {
            targets.Insert(pair.Value);
        }

        // LevelOrder() lanza NullReferenceException con el arbol vacio
        if (targets.IsEmpty) return;

        // 3) BFS: se visita cada objetivo una vez. La capa sale de su distancia, no de su posicion en el recorrido.
        ISimpleList<ExplosionTarget> byLevels = targets.LevelOrder(); //se guarda en una variable de tipo lista lo que devuelve la funcion de LevelOrder()
        for (int i = 0; i < byLevels.Count; i++)
        {
            ApplyEffect(byLevels[i], epicenter);
        }
    }

    private void ApplyEffect(ExplosionTarget target, Vector2 epicenter)
    {
        if (target.collider == null) return; // ya fue destruido

        ExplosionLayer layer = GetLayer(target.distance); //obtenemos la capa donde ocurre la chicha

        int damage = Mathf.RoundToInt(stats.Damage * layer.damageMultiplier); //un entero que se redondea (25 * capa 1: 1f) = 25 => 25 AURA
                                                                              //(25 * capa 2: 0.5f) = 12.5 => 12 AURA
                                                                              //(25 * capa 3: 0.25f) = 6.25 => 6 AURA
        if (damage > 0)
        {
            target.damageable.TakeDamage(damage); //si hay daño, llamamos a las matematicas para que hagan el laburo por nosotros
        }

        if (layer.knockbackSpeed > 0f) //aplicamos un empuje, no me pregunten porque las fisicas me enferman
        {
            Vector2 direction = (Vector2)target.collider.bounds.center - epicenter;
            direction = direction.sqrMagnitude > 0.0001f //si existe una magnitud, o sea, una direccion...
                ? direction.normalized                   //-> se normaliza
                : UnityEngine.Random.insideUnitCircle.normalized; //si no, se devuelve un numero reandom de un circulo de radio 1 [1f - 0f]

            KnockbackReceiver.Apply(target.collider.attachedRigidbody, direction * layer.knockbackSpeed, knockbackDuration); //FINALMENTE -> se llama a la funcion static y se aplica el empujon
        }
    }

    private ExplosionLayer GetLayer(float distance)
    {
        for (int i = 0; i < layers.Length; i++) //recorre el array de las tres capas
        {
            if (distance <= layers[i].maxDistance) return layers[i]; //devuelve la capa que sea menor o igual a la distancia de la capa del bucle actual 
        }

        // Por si el borde del collider queda apenas afuera del radio
        return layers[layers.Length - 1]; //y en caso que termine el bucle y no se devuelve nada, significa que se tiene que devolver la ultima capa
    }

    // Dibuja las capas en el editor para poder ajustarlas viendolas
    private void OnDrawGizmosSelected()
    {
        if (layers == null) return;

        Color[] colors = { Color.red, new Color(1f, 0.5f, 0f), Color.yellow };
        for (int i = 0; i < layers.Length; i++)
        {
            Gizmos.color = colors[Mathf.Min(i, colors.Length - 1)];
            Gizmos.DrawWireSphere(transform.position, layers[i].maxDistance);
        }
    }
}
