using System;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EnemyFacade))]
public class Enemy : MonoBehaviour, IDamageable
{
    public static event Action<int> OnEnemyGoldDrop; // evento que avisa a la UI que el enemigo ha muerto y que debe actualizarse el oro del jugador
    //public static event Action<int> OnEnemyDeath; // Para cuando hagamos xp xd
    // Identificacion
    public string id;
    private bool initialized;


    // Estadisticas
    [SerializeField] protected EnemyStats stats;

    [Header("Multiplicador de dano por tipo de bala")]
    [SerializeField] private float normalDamageMultiplier = 1f;        //1 chaser, 1 ghost, 0.5 suicide, 0.5 two headed
    [SerializeField] private float heavyDamageMultiplier = 1f;         //1.5 chaser, 0.25 ghost, 2 suicide, 0.5 two headed
    [SerializeField] private float piercingDamageMultiplier = 1f;      //0.25 chaser, 1 ghost, 1.5 suicide, 2 two headed
    [SerializeField] private float knockbackDamageMultiplier = 1f;     //2 chaser, 1.5 ghost, 0.5 suicide, 0.5 two headed

    //Referencias
    protected GameObject player;
    protected Rigidbody2D rb;
    private EnemyFacade facade;

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

        facade = GetComponent<EnemyFacade>();

        if(facade==null)
        {
            Debug.LogError("No se encontro el EnemyFacade en el enemigo.");
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
        else         
        {
            facade.OnHit();
        }
    }

    public void TakeDamage(int damage, BulletType bulletType)
    {
        float multiplier = 1f;

        switch (bulletType)
        {
            case BulletType.Heavy:
                multiplier = heavyDamageMultiplier;
                break;
            case BulletType.Piercing:
                multiplier = piercingDamageMultiplier;
                break;
            case BulletType.Knockback:
                multiplier = knockbackDamageMultiplier;
                break;
            case BulletType.Normal:
                multiplier = normalDamageMultiplier;
                break;
        }

        TakeDamage(Mathf.Max(0, Mathf.RoundToInt(damage * multiplier)));
    }
    // Esto es virtual por q la muerte puede variar segun el enemigo!
    protected virtual void Die()
    {
        facade.OnDeath();
        OnEnemyGoldDrop?.Invoke(stats.EnemyGoldDrop);
        Destroy(gameObject);
    }
    protected void FlipTowardsPlayer()
    {
        if (player.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    protected Vector2 GetChaseDirection()
    {
        return PathfindingManager.Instance.GetDirection(transform.position, player.transform.position);
    }

    protected virtual void Update()
    {
        FlipTowardsPlayer();
    }

}
