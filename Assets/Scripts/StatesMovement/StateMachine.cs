using UnityEngine;

public class StateMachine
{
    public IState CurrentState;


    public void ChangeState(IState state)
    {
        CurrentState?.Exit();
        CurrentState = state;
        CurrentState.Enter();
    }
   
   public void UpdateMachine()
    {
        CurrentState?.UpdateState();
    }
    
}
