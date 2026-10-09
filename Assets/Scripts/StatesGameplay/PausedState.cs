using UnityEngine;

public class PausedState : IState
{
    private readonly LevelManager levelManager;
    private float previousTimeScale;
    private bool previousCursorVisible;
    private CursorLockMode previousCursorLock;

    public PausedState(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    public void Enter()
    {
        previousTimeScale = Time.timeScale;
        previousCursorVisible = Cursor.visible;
        previousCursorLock = Cursor.lockState;
        levelManager.ChangeTime(0f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (levelManager.InventoryPanel != null)
            levelManager.InventoryPanel.SetActive(true);
    }

    public void UpdateState() { }

    public void Exit()
    {
        if (levelManager.InventoryPanel != null)
            levelManager.InventoryPanel.SetActive(false);
        levelManager.ChangeTime(previousTimeScale);
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;
    }
}
