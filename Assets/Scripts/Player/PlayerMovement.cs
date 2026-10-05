using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Rigidbody2D rb;

    public IState IdleState { get; private set; }
    public IState WalkState { get; private set; }
    public IState DashState { get; private set; }


    public Rigidbody2D Rb => rb;
    public PlayerStats PlayerStats => playerStats;
    [field:SerializeField] public float DashForce = 20f;

    public float DashCooldown = 2f;
    [field: SerializeField] public float DashDuration = 0.3f;

    public StateMachine statemachine;


    [field: SerializeField] public float x { get; private set; } //ahi lo encontre nico, public get (se puede leer), private set (se cambia de forma privada)
    [field: SerializeField] public float y { get; private set; }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        IdleState = new Idle(this);
        WalkState = new Walk(this);
        DashState = new Dash(this);

        statemachine = new StateMachine();
        statemachine.ChangeState(IdleState);


    }

    
    void Update()
    {
        DashCooldown += Time.deltaTime; //esto hace que se sume al cooldown asi lo puede usar, la condicion para usarlo esta en la clase walk

        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        statemachine.UpdateMachine();
    }
}
