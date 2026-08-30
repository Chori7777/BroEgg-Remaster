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

        player.rb.linearVelocity = direction * player.speed; //lo tuve que cambiar a linear velocity nico, perdon, pero addforce no funciona con moveposition

        if (player.x == 0 && player.y == 0) 
        {
            player.statemachine.ChangeState(player.statemachine.IdleState);
        }

        if(Input.GetKey(KeyCode.Space))
        {
            if(player.DashCooldown >= 2)
            {
                player.statemachine.ChangeState(player.statemachine.DashState);
                player.DashCooldown = 0;
            }
        }
    }
}
