using UnityEngine;

public class LevelStateMachine 
{
    public IState Shop;
    public IState Gameplay;
    public IState CurrentState;

    public LevelStateMachine(LevelManager levelManager)
    {
        Shop = new ShopState(levelManager);
        Gameplay = new GameplayState(levelManager);

        CurrentState = Gameplay;

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
