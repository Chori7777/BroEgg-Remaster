using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float defaultBulletSpeed;
    Rigidbody2D rb;
    public BulletPool pool; //publica para acceder desde las weapons
    float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

    public void Setup(Vector2 direction)
    {
        rb.linearVelocity = direction * defaultBulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            pool.Recycle(this);
        }
    }
}
