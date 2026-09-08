using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Rigidbody2D rb;

    
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
        statemachine = new StateMachine(this);
    }

    
    void Update()
    {
        DashCooldown += Time.deltaTime; //esto hace que se sume al cooldown asi lo puede usar, la condicion para usarlo esta en la clase walk

        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        statemachine.UpdateMachine();
    }
}
