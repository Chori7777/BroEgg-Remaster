using UnityEngine;

public class Idle : IState
{
    PlayerMovement player;

    public Idle(PlayerMovement player)
    { 
        this.player = player; 
    }
    public void Enter()
    {
        Debug.Log("IDLE");
    }

    public void Exit()
    {
        
    }

    public void UpdateState()
    {
        if (player.x != 0 || player.y != 0)
        {
            player.statemachine.ChangeState(player.statemachine.WalkState);
        }
    }
}
