using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int speed;
    public int dashForce;
    public StateMachine statemachine;

    public Rigidbody2D rb;

    public float x;
    public float y;

    Vector2 direction;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        statemachine = new StateMachine(this);
    }

    
    void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");

        statemachine.UpdateMachine();
    }
}
