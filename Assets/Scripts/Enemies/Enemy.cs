using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class Enemy : MonoBehaviour, IDamageable
{

    // Identificacion
    public string id;
    private bool initialized;

    // Estadisticas
    [SerializeField] protected EnemyStats stats;

    //Referencias
    protected GameObject player;
    protected Rigidbody2D rb;


    //Encontrar Referencias

    //Es virtual en caso de q en alguno de los enemigos necesitemos hacer algo mas en Start() y no queremos sobreescribir estos das dos referencias, si no se petatearia todo
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");


        if (player == null)
        {
            Debug.LogError("No se encontro el jugador en la escena.");
        }

        rb = GetComponent<Rigidbody2D>();

        if (rb== null)
        {
            Debug.LogError("No se encontro el Rigidbody2D en el enemigo.");
        }

        stats = GetComponent<EnemyStats>();


        if (stats== null)
        {
            Debug.LogError("No se encontro el EnemyStats en el enemigo.");
        }
     


    }
    //Voids 

 
    public void Initialize(int round)
    {
        // Esto evita que el enemigo sea inicializado mas de una vez, lo cual podria causar problemas con las stats del enemigo
        // asi q simplemente verificamos si ya fue inicializado y si es asi mostramos un error en consola y retornamos
        // Mas q nada es una medida de seguridad! ojala y sirva, si no, existira ahi hasta que alguien se equivoque e inicialice un enemigo mas de una vez
        if (initialized)
        {
            Debug.LogError("El enemigo ya fue inicializado.");
            return;
        }
        else
        {
            stats.Initialize(round);
            initialized = true;
        }
         
    }


    public void TakeDamage(int damage)
    {
        stats.TakeDamage(damage);


        if (stats.Health <= 0)
        {
            Die();
        }
    }
    // Esto es virtual por q la muerte puede variar segun el enemigo!
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
