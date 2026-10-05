using UnityEngine;

public class DasherEnemy : Enemy
{
    // Todos los valores de ajuste estan aca
    [Header("Deteccion")]
    public float detectionRange = 7f;     // a que distancia ve al jugador
    public float alertDuration = 0.6f;    // lo que dura la animacion de alerta

    [Header("Dash")]
    public float dashDuration = 0.5f;
    public float dashSpeedMultiplier = 3.5f;

    [Header("Descanso")]
    public float restDuration = 1f;

    [Header("Acercarse")]
    public float optimalDistance = 4f;    // distancia a la que deja de correr y vuelve a dashear
    public float chaseSpeedMultiplier = 1.2f;
    public float chaseTimeout = 4f;       // limite por si queda trabado y nunca llega

    [Header("Visual")]
    public Animator animator;
  

    StateMachine machine;
    EnemyBrain brain;
    IEnemyBehavior current;

    protected override void Update()
    {

        base.Update();


        if (player == null) return;
       
        // La maquina y el cerebro se crean la primera vez que se llega aca, y no en Start.
        // Por que: no se si tu Enemy ya usa Start o Awake para preparar player y rb. Si
        // yo definiera uno propio, el de Enemy dejaria de ejecutarse.
        if (machine == null)
        {
            CreateBrain();
        }

        machine.UpdateMachine();      // el state actual hace lo suyo

        if (current.IsFinished)       // el state avisa que termino
        {
            current = brain.Next();   // el cerebro elige el siguiente
         
            machine.ChangeState(current);
        }
    }

    void CreateBrain()
    {

        if (animator == null) animator = GetComponentInChildren<Animator>();


        machine = new StateMachine();
        brain = new EnemyBrain(false);   // false = secuencia: Dash, Rest, Chase, y otra vez

        brain.Add(new DasherDash(this));
        brain.Add(new DasherRest(this));
        brain.Add(new DasherChase(this));

        // Idle no se agrega al cerebro: pasa una sola vez, al principio.
        // Arranca directo, sin pasar por la cola.
        current = new DasherIdle(this);
        machine.ChangeState(current);
    }

    // Metodos que usan los states. Estan aca para que los states no toquen rb ni player.

    public float DistanceToPlayer()
    {
        return Vector2.Distance(player.transform.position, transform.position);
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

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
    }

    // Si hay Animator, reproduce el state con ese nombre. Si no, pinta el sprite para
    // poder ver a ojo en que state esta.
    // MODIFICADO: ya no hay rama de colores, solo reproduce el state del Animator.
    public void ShowState(string stateName)
    {
        animator.Play(stateName);
    }


    // Copiado de Zombie: asi el Dasher tambien danea al chocar con el jugador.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(stats.Damage);
        }
    }
}