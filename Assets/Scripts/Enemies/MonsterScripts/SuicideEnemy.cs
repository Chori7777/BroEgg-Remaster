using UnityEngine;

public class SuicideEnemy : Enemy
{
    [SerializeField] private GameObject eyePrefab;
    [SerializeField] private int eyeCount = 3;
    [SerializeField] private float spawnRadius = 0.5f;


    protected override void Update()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * stats.Speed;
        base.Update();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(stats.Damage);
        }

        SpawnEyes();
        Destroy(gameObject);
    }

    private void SpawnEyes()
    {
        for (int i = 0; i < eyeCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 position = transform.position + (Vector3)offset;

            GameObject eye = Instantiate(eyePrefab, position, Quaternion.identity);

            Enemy eyeEnemy = eye.GetComponent<Enemy>();
            eyeEnemy.Initialize(LevelManager.Instance.CurrentRound);

        }
    }
}