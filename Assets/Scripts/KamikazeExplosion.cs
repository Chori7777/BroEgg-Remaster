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
        if (byDistance != 0) return byDistance; //aca puede dar 1 o -1 

        // El BST descarta los valores que dan CompareTo == 0. Si dos objetivos estan exactamente
        // a la misma distancia, desempatamos por id para no perder a ninguno.
        return collider.GetInstanceID().CompareTo(other.collider.GetInstanceID());
    }
}

// Una capa concentrica de la explosion.
[Serializable]
public class ExplosionLayer //Una clase de una capa invisible de explosion que tiene una distacia max, un multiplicador daño, y una fureza de empuje
{
    public float maxDistance;
    [Range(0f, 1f)] public float damageMultiplier;
    public float knockbackSpeed;

    public ExplosionLayer(float maxDistance, float damageMultiplier, float knockbackSpeed)
    {
        this.maxDistance = maxDistance;
        this.damageMultiplier = damageMultiplier;
        this.knockbackSpeed = knockbackSpeed;
    }
}


public class KamikazeExplosion : MonoBehaviour
{
    [Header("Capas (de adentro hacia afuera)")] //un array de 3 capas
    [SerializeField] private ExplosionLayer[] layers =
    {
        new ExplosionLayer(1.5f, 1.00f, 18f), // Capa 1: muy cerca, daño maximo y knockback fuerte
        new ExplosionLayer(3.0f, 0.50f, 0f),  // Capa 2: distancia media, daño moderado
        new ExplosionLayer(4.5f, 0.25f, 0f)   // Capa 3: borde, daño bajo
    };

    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float knockbackDuration = 0.25f;

    [Tooltip("Muestra en consola cuantos objetivos hubo y la altura del arbol")]
    [SerializeField] private bool logTreeInfo = false;

    private EnemyStats stats;
    private bool exploded;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>(); //obtiene las stats
    }

    public void Explode()
    {
        // OnCollisionEnter2D puede dispararse mas de una vez antes de que el objeto se destruya
        if (exploded || layers == null || layers.Length == 0) return;
        exploded = true;

        Vector2 epicenter = transform.position; //el OverlapCircle arranca en el centro del enemigo

        // 1) Radio fisico: el borde de la ultima capa
        float radius = layers[layers.Length - 1].maxDistance; //la longitud del centro del circulo hasta el borde de la 3ra capa -> radio = 4.5f
        Collider2D[] hits = Physics2D.OverlapCircleAll(epicenter, radius, targetLayers); //Guardo todas las colisiones que esten en el capa deseada

        // 2) Un objeto puede tener varios colliders: nos quedamos con el mas cercano de cada uno
        Dictionary<IDamageable, ExplosionTarget> closestByTarget = new Dictionary<IDamageable, ExplosionTarget>(); //el diccionario evita que los colliders se dupliquen

        for (int i = 0; i < hits.Length; i++) //recorro todos los hits detectados, y por cada uno de ellos...
        {
            Collider2D hit = hits[i]; //lo guardo de una variable (más comodidad)
            IDamageable damageable = hit.GetComponent<IDamageable>(); //guardo en una variable el componente adquirido de las colision
            if (damageable == null) continue; //IMPORTANTE: si damageable es null, salta al proximo bucle del for y se ahorra todo

            float distance = Vector2.Distance(epicenter, hit.ClosestPoint(epicenter)); //calcula la distancia densde el centro hasta el objetivo hiteado

            if (closestByTarget.TryGetValue(damageable, out ExplosionTarget existing) && existing.distance <= distance) continue; //si ya existia la clave Y la distancia existente <= a la nueva, quedate con lo viejo y no agregues nada
            closestByTarget[damageable] = new ExplosionTarget(damageable, hit, distance); //si no, agregamos este nuevo ExplosionTarget
        }

        // Cada objetivo se inserta en el BST usando su distancia como criterio de orden
        AVL<ExplosionTarget> targets = new AVL<ExplosionTarget>();
        foreach (KeyValuePair<IDamageable, ExplosionTarget> pair in closestByTarget) //recorre el diccionario y guarda cada ExplosionTarget en el arbol
        {
            targets.Insert(pair.Value);
        }

        // LevelOrder() lanza NullReferenceException con el arbol vacio
        if (targets.IsEmpty) return;

        if (logTreeInfo)
        {
            string treeName = "AVL";
            Debug.Log("Explosion: " + targets.Count + " objetivos, arbol " + treeName + " de altura " + targets.height);
        }

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

        if (layer.knockbackSpeed > 0f) //aplicamos un empuje
        {
            Vector2 direction = (Vector2)target.collider.bounds.center - epicenter;
            if(direction.sqrMagnitude > 0.0001f)//si existe una magnitud, o sea, una direccion...
            {
                direction = direction.normalized; //-> se normaliza
            }
            else
            {
                direction = UnityEngine.Random.insideUnitCircle.normalized; //si no, se devuelve un numero reandom de un circulo de radio 1 [1f - 0f]
            }
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

    // Dibujo para que junior vea
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
