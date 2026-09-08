using UnityEngine;
using UnityEngine.UI;

public class ShopState : IState
{
    
    [SerializeField] private float timershop = 2;
    LevelManager levelManager;
    public ShopState(LevelManager levelManager) { this.levelManager = levelManager; }

    public void Enter()
    {
        levelManager.shopPanel.gameObject.SetActive(true);
        levelManager.Objeto1.sprite = levelManager.inventory.ChooseObject().SpriteShop;
        levelManager.Objeto2.sprite = levelManager.inventory.ChooseObject().SpriteShop;
        levelManager.Objeto3.sprite = levelManager.inventory.ChooseObject().SpriteShop;
        levelManager.Objeto4.sprite = levelManager.inventory.ChooseObject().SpriteShop;
    }

    public void UpdateState()
    {
        timershop -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.O) && timershop <= 0)
        {
            levelManager.shopPanel.gameObject.SetActive(false);
            timershop = 2;
            levelManager.levelStateMachine.ChangeState(levelManager.levelStateMachine.Gameplay);
            return;
        }
    }

    public void Exit()
    {
    }
}
