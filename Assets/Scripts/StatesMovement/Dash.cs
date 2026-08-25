using UnityEngine;

public class Dash : IState
{
    PlayerMovement player;

    Vector2 direction;

    public Dash(PlayerMovement player)
    {
        this.player = player;
    }
    public void Enter()
    {
        Debug.Log("Dash");

        direction = new Vector2(player.x, player.y).normalized;

        Vector2 move = direction * player.dashForce * Time.fixedDeltaTime;

        player.rb.AddForce(move, ForceMode2D.Impulse);
    }

    public void Exit()
    {

    }

    public void UpdateState()
    {

    }
}
