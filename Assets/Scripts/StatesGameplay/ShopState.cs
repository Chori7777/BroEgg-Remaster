using UnityEngine;

public class ShopState : IState
{
    private readonly LevelManager levelManager;

    public ShopState(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    public void Enter()
    {
        levelManager.shopPanel.gameObject.SetActive(true);
        levelManager.shopManager.Open();
        levelManager.ChangeTime(0);
    }

    public void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            levelManager.shopPanel.gameObject.SetActive(false);
            levelManager.levelStateMachine.ChangeState(levelManager.levelStateMachine.Gameplay);
        }
    }

    public void Exit()
    {
        levelManager.shopManager.Close();
        levelManager.shopPanel.gameObject.SetActive(false);
        levelManager.ChangeTime(1f);
    }
}
