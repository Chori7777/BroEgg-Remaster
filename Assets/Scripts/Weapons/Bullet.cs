using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float defaultBulletSpeed;
    Rigidbody2D rb;
    public BulletPool pool; //publica para acceder desde las weapons
    Collider2D bulletCollider;
    float timer;
    private int damage;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 2)
        {
            pool.Recycle(this);
            timer = 0;
        }
    }

    public void Setup(Vector2 direction, int damage, Collider2D ownerCollider)
    {
        //guardamos el parametro de damage en la variable de instancia para usarlo en OnTriggerEnter2D y poder hacer daño!
        this.damage = damage;
        rb.linearVelocity = direction * defaultBulletSpeed;
        if (ownerCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, ownerCollider);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);

            pool.Recycle(this);
        }
    }
}
