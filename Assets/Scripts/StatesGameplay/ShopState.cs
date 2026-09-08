using UnityEngine;

public class ShopState : IState
{

    LevelManager levelManager;
    public ShopState(LevelManager levelManager) { this.levelManager = levelManager; }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
}
