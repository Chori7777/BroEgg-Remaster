using UnityEngine;
using UnityEngine.UI;

public class ShopState : IState
{
    
    LevelManager levelManager;
    public ShopState(LevelManager levelManager) { this.levelManager = levelManager; }

    public void Enter()
    {
        levelManager.shopPanel.gameObject.SetActive(true);

        for (int i = 0; i < levelManager.botones.Length; i++)
        {
            ShopProductData objeto = levelManager.inventory.ChooseObject();
            levelManager.botones[i].image.sprite = objeto.icon;
            levelManager.botones[i].GetComponent<ButtonLogic>().Setup(objeto);
        }
        levelManager.ChangeTime(0);
    }

    public void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            levelManager.shopPanel.gameObject.SetActive(false);
            levelManager.levelStateMachine.ChangeState(levelManager.levelStateMachine.Gameplay);
            return;
        }
    }

    public void Exit()
    {
        levelManager.ChangeTime(1f);
    }
}
