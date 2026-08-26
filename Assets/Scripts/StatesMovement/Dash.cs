using UnityEngine;

public class Dash : IState
{
    PlayerMovement player;

    Vector2 dashDirection;
    bool isDashing;
    bool canDash;

    public float currentDashDuration; 

    public Dash(PlayerMovement player)
    {
        this.player = player;
    }
    public void Enter()
    {
            currentDashDuration = player.DashDuration;

            dashDirection = new Vector2(player.x, player.y).normalized;

            if (dashDirection == Vector2.zero)
            {
                dashDirection = player.transform.forward;
            }

            isDashing = true;
    }

    public void Exit()
    {
        Debug.Log("Saliendo de Dash");
        player.rb.linearVelocity = Vector2.zero;
    }

    public void UpdateState()
    {
        if(isDashing)
        {
            currentDashDuration -= Time.deltaTime;
            player.rb.linearVelocity = dashDirection * player.DashForce;
        }

        // Si se acaba el tiempo de duración del Dash, volvemos a Walk o Idle
        if (currentDashDuration <= 0)
        {
            if (player.x == 0 && player.y == 0)
            {
                player.statemachine.ChangeState(player.statemachine.IdleState);
                isDashing = false;
            }

            else
            {
                player.statemachine.ChangeState(player.statemachine.WalkState);
                isDashing = false;
            }
        }
    }
}
