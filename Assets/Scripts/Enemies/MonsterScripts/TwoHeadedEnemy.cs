using UnityEngine;

public class TwoHeadedEnemy : Enemy
{
    [Header("Distancia")]
    public float cutDistance = 5f;          // por debajo es cerca, por encima es lejos

    [Header("Acercarse")]
    public float chaseDuration = 2f;        // cuanto camina antes de disparar
    public float minChaseDistance = 2f;     // si llega tan cerca, dispara antes

    [Header("Disparo")]
    public float slowFactor = 0.3f;         // velocidad durante el frenado 
    public float slowDuration = 0.6f;       // cuanto tiempo tarda para ralentizarse  antes de disparar

    [Header("Cabezas")]
    public Transform firePointLeft;
    public Transform firePointRight;
    public GameObject projectilePrefabLeft;
    public GameObject projectilePrefabRight;

    [Header("Visual")]
    public Animator animator;

    StateMachine machine;
    EnemyBrain brain;
    IEnemyBehavior chase;
    IEnemyBehavior current;

    protected override void Update()
    {
        base.Update();

        if (player == null) return;

        if (machine == null)
        {
            CreateBrain();
        }

        machine.UpdateMachine();

        if (current.IsFinished)
        {
            if (current == chase)
            {
                current = brain.Next();   // el cerebro elige la cabeza segun la distancia de ahora
            }
            else
            {
                current = chase;          // despues de disparar, vuelve a caminar
            }

            machine.ChangeState(current);
        }
    }

    void CreateBrain()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();

        machine = new StateMachine();
        brain = new EnemyBrain(true);     // true: recalcula la prioridad en cada decision

        brain.Add(new TwoHeadedShoot(this, false));   // derecha primero
        brain.Add(new TwoHeadedShoot(this, true));    // izquierda despues

        chase = new TwoHeadedChase(this);
        current = chase;
        machine.ChangeState(current);
    }

    // Metodos que usan los states

    public float DistanceToPlayer()
    {
        return Vector2.Distance(player.transform.position, transform.position);
    }

    public bool IsPlayerFar()
    {
        return DistanceToPlayer() > cutDistance;
    }

    public Vector2 DirToPlayer()
    {
        Vector2 direction = player.transform.position - transform.position;
        return direction.normalized;
    }

    public void Move(Vector2 direction, float multiplier)
    {
        rb.linearVelocity = direction * stats.Speed * multiplier;
    }

    public void ShowState(string stateName)
    {
        if (animator != null) animator.Play(stateName);
    }

    public void Shoot(bool isLeft)
    {
        Transform firePoint = isLeft ? firePointLeft : firePointRight;
        GameObject prefab = isLeft ? projectilePrefabLeft : projectilePrefabRight;

        if (firePoint == null || prefab == null)
        {
            Debug.LogError("Falta asignar FirePoint o prefab de la cabeza " + (isLeft ? "izquierda" : "derecha"));
            return;
        }

        Vector2 direction = (player.transform.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(prefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<EnemyBullet>().Setup(direction, stats.Damage);
       
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, cutDistance);

        if (firePointLeft != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(firePointLeft.position, 0.15f);
        }
        if (firePointRight != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePointRight.position, 0.15f);
        }

        if (!Application.isPlaying || player == null) return;

        Gizmos.color = IsPlayerFar() ? Color.cyan : Color.red;
        Gizmos.DrawLine(transform.position, player.transform.position);
    }
}