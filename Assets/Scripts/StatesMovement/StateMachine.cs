using UnityEngine;

public class StateMachine
{
    public IState IdleState;
    public IState WalkState;
    public IState DashState;
    public IState CurrentState;
    public StateMachine(PlayerMovement player)
    {
        IdleState = new Idle(player);
        WalkState = new Walk(player);
        DashState = new Dash(player);
        CurrentState = IdleState;

        CurrentState.Enter();
    }
    public void ChangeState(IState state)
    {
        CurrentState.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }
    public void UpdateMachine()
    {
        CurrentState.UpdateState();
    }
}
