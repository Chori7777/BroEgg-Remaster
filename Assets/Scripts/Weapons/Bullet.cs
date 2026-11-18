using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float defaultBulletSpeed;
    [SerializeField] float timer = 3f;

    [Header("Tipo de bala (se configura en cada prefab)")]
    [SerializeField] private BulletType bulletType = BulletType.Normal;
    [Tooltip("A cuantos enemigos atraviesa antes de reciclarse. 1 = normal, 3 = penetrante")]
    [SerializeField] private int maxHits = 1;
    [Tooltip("Velocidad del empuje en unidades por segundo. 0 = no empuja")]
    [SerializeField] private float knockbackForce = 0f;
    [Tooltip("Cuanto dura el empuje, en segundos")]
    [SerializeField] private float knockbackDuration = 0.15f;

    Rigidbody2D rb;
    public BulletPool pool; //publica para acceder desde las weapons
    Collider2D bulletCollider;
    private int damage;
    private Vector2 direction;
    private int hitsLeft;
    private float currentKnockbackForce;

    //enemigos que esta bala ya golpeo: una bala que atraviesa no puede danar dos veces al mismo enemigo
    private readonly HashSet<IDamageable> alreadyHit = new HashSet<IDamageable>();

    public float Speed => defaultBulletSpeed;
    public BulletType Type => bulletType;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Recycle();
        }
    }

    public void Setup(Vector2 direction, int damage, Collider2D ownerCollider, BulletType type)
    {
        //guardamos el parametro de damage en la variable de instancia para usarlo en OnTriggerEnter2D y poder hacer dano!
        this.damage = damage;
        this.direction = direction;
        bulletType = type;

        //las balas se reutilizan (pool): hay que dejar el estado limpio en cada disparo
        hitsLeft = bulletType == BulletType.Piercing
            ? Mathf.Max(2, maxHits)
            : Mathf.Max(1, maxHits);
        currentKnockbackForce = bulletType == BulletType.Knockback
            ? Mathf.Max(8f, knockbackForce)
            : knockbackForce;
        alreadyHit.Clear();
        timer = 3f;

        rb.linearVelocity = direction * defaultBulletSpeed;
        if (ownerCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, ownerCollider);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        if (hitsLeft <= 0) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable == null) return;

        //si ya golpeamos a este enemigo (bala penetrante), no lo golpeamos de nuevo
        if (!alreadyHit.Add(damageable)) return;

        //1) DANO. Si es un Enemy le pasamos el tipo de bala: el enemigo aplica su propia resistencia.
        Enemy enemy = damageable as Enemy;
        if (enemy != null) enemy.TakeDamage(damage, bulletType);
        else damageable.TakeDamage(damage);

        //2) EMPUJE (solo si esta bala empuja). KnockbackReceiver.Push se puede usar desde cualquier otro script.
        if (currentKnockbackForce > 0f)
        {
            KnockbackReceiver.Apply(collision.attachedRigidbody, direction * currentKnockbackForce, knockbackDuration);
        }

        //3) A cuantos enemigos mas puede atravesar? Con maxHits = 1 se recicla al primer golpe, como antes.
        hitsLeft--;
        if (hitsLeft <= 0) Recycle();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //una bala penetrante no se frena contra un enemigo con collider solido (el dano lo hace el trigger).
        //Se ignora solo ese choque fisico y se recupera la velocidad por si el choque la freno.
        if (hitsLeft > 1 && collision.collider.GetComponent<IDamageable>() != null)
        {
            Physics2D.IgnoreCollision(collision.otherCollider, collision.collider);
            rb.linearVelocity = direction * defaultBulletSpeed;
            return;
        }

        //contra una pared (o cualquier otra cosa) se recicla
        Recycle();
    }

    private void Recycle()
    {
        if (pool != null) pool.Recycle(this);
        else gameObject.SetActive(false);
    }
}
