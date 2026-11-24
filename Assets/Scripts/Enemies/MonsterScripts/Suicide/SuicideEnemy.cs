using UnityEngine;

public class SuicideEnemy : Enemy
{
    [SerializeField] private GameObject eyePrefab;
    [SerializeField] private int eyeCount = 3;
    [SerializeField] private float spawnRadius = 0.5f;
    [Tooltip("Cada cuantos segundos pide un camino A* nuevo (el jugador se mueve y el camino se desactualiza)")]
    [SerializeField] private float repathInterval = 0.5f;
    [SerializeField] private float reachDistance = 0.4f;
    private KamikazeExplosion explosion;
    private PathFollower pathFollower;

    protected override void Start()
    {
        base.Start();
        explosion = GetComponent<KamikazeExplosion>(); //obtengo el script de la explosion
        pathFollower = new PathFollower(repathInterval, reachDistance); //cada Suicide tiene su propio camino A*
    }

    //Solo el Suicide esquiva obstaculos con A*: los demas enemigos usan la version de Enemy (linea recta)
    protected override Vector2 GetChaseDirection()
    {
        return pathFollower.GetDirection(transform.position, player.transform.position);
    }

    protected override void Update()
    {
        Vector2 direction = GetChaseDirection().normalized; //OJO: sin "base.", si no se saltea el A*
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
        explosion.Explode();
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

    //Seleccionando al Suicide en Play se dibuja su camino A* en verde
    private void OnDrawGizmosSelected()
    {
        if (pathFollower == null || pathFollower.CurrentPath == null || PathfindingManager.Instance == null) return;

        Gizmos.color = Color.green;
        for (int i = 0; i + 1 < pathFollower.CurrentPath.Count; i++)
        {
            Gizmos.DrawLine(PathfindingManager.Instance.NodeToWorld(pathFollower.CurrentPath[i]), PathfindingManager.Instance.NodeToWorld(pathFollower.CurrentPath[i + 1]));
        }
    }
}
