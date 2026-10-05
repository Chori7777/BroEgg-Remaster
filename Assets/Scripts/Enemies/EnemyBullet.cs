using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float speed = 6f;      // cada prefab tiene su propia velocidad
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody2D rb;
    private int damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 direction, int damage)
    {
        this.damage = damage;
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifeTime);              // si no pega, se destruye solo
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}