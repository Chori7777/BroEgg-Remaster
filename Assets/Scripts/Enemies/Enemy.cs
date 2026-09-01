using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string id;
    GameObject Player;
    GameObject Bullet;
    Rigidbody2D rb;

    [SerializeField] private int speedEnemy = 1;
    void Start()
    {
       Player = GameObject.FindGameObjectWithTag("Player");
       Bullet = GameObject.FindGameObjectWithTag("Bullet");

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = (Player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * speedEnemy;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }

}
