using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class Bullet : MonoBehaviour
{
    [SerializeField] float defaultBulletSpeed;
    Rigidbody2D rb;
    public BulletPool pool; //publica para acceder desde las weapons
    Collider2D bulletCollider;
    [SerializeField] float timer = 3f;
    private int damage;


    public float Speed => defaultBulletSpeed;

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
            pool.Recycle(this);
            Debug.Log("Me recicle por tiempo");
            timer = 3;
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
        if (collision.gameObject.CompareTag("Player")) return;


        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);

            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayImpact(ImpactType.Blood, transform.position, -rb.linearVelocity.normalized);
            }

            pool.Recycle(this);
            Debug.Log("Me recicle por enemigo");
            timer = 3;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.CompareTag("ObstacleGrid"))
            {
                timer = 3;

                if (VFXManager.Instance != null)
                {
                    ContactPoint2D contact = collision.GetContact(0);
                    VFXManager.Instance.PlayImpact(ImpactType.WallSpark, contact.point, contact.normal);
                    VFXManager.Instance.PlayImpact(ImpactType.Dust, contact.point, contact.normal);
                }

                pool.Recycle(this);
                Debug.Log("Me recicle por pegarle a el ObstacleMap");
            }
            if (collision.gameObject.CompareTag("BorderMap"))
            {
                timer = 3;

                if (VFXManager.Instance != null)
                {
                    ContactPoint2D contact = collision.GetContact(0);
                    VFXManager.Instance.PlayImpact(ImpactType.WallSpark, contact.point, contact.normal);
                    VFXManager.Instance.PlayImpact(ImpactType.Dust, contact.point, contact.normal);
                }

                pool.Recycle(this);
                Debug.Log("Me recicle por pegarle a el BorderMap");
            }
        }
    }
}
