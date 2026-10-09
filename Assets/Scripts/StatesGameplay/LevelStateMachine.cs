using UnityEngine;

public class LevelStateMachine 
{
    public IState Shop;
    public IState Gameplay;
    public IState CurrentState;
    public IState Paused { get; private set; }
    private IState suspendedState;
    public bool IsPaused => CurrentState == Paused;

    public LevelStateMachine(LevelManager levelManager)
    {
        Shop = new ShopState(levelManager);
        Gameplay = new GameplayState(levelManager);
        Paused = new PausedState(levelManager);

        CurrentState = Gameplay;

        CurrentState.Enter();
    }

    public void ChangeState(IState state)
    {
        if (IsPaused) return;
        CurrentState.Exit();
        CurrentState = state;
        CurrentState.Enter();

    }

    public void UpdateMachine()
    {
        CurrentState.UpdateState();
    }

    public void Pause()
    {
        if (IsPaused) return;
        // Suspender no ejecuta Exit: conserva la tienda y la oleada.
        suspendedState = CurrentState;
        CurrentState = Paused;
        CurrentState.Enter();
    }

    public void Resume()
    {
        if (!IsPaused) return;
        CurrentState.Exit();
        // No ejecutar Enter: Gameplay.Enter inicia otra oleada.
        CurrentState = suspendedState;
        suspendedState = null;
    }
}
