using UnityEngine;

public class Walk : IState
{
    PlayerMovement player;


    Rigidbody2D rb;

    Vector2 direction;

    public Walk(PlayerMovement player)
    {
        this.player = player;
    }
    public void Enter()
    {
        Debug.Log("WALK");
    }

    public void Exit()
    {

    }

    public void UpdateState()
    {

        direction = new Vector2(player.x, player.y).normalized;

        Vector2 move = direction * player.speed * Time.fixedDeltaTime;

        player.rb.MovePosition(player.rb.position + move);

        if (player.x == 0 && player.y == 0) 
        {
         player.statemachine.ChangeState(player.statemachine.IdleState);
        }

        if(Input.GetKey(KeyCode.Space))
        {
         player.statemachine.ChangeState(player.statemachine.DashState);
        }
    }
}
