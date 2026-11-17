public class VictoryState : IState
{
    private LevelManager levelManager;

    public VictoryState(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    public void Enter()
    {
        if (levelManager.VictoryPanel != null)
        {
            levelManager.VictoryPanel.SetActive(true);
        }
        levelManager.ChangeTime(0f); // Congela el juego

        UnityEngine.Cursor.visible = true;
    }

    public void Exit()
    {
        if (levelManager.VictoryPanel != null)
        {
            levelManager.VictoryPanel.SetActive(false);
        }
        levelManager.ChangeTime(1f); // ¡OBLIGATORIO: Restaura el tiempo al salir!
    }

    public void UpdateState()
    {
        // Inputs de la pantalla de victoria (ej: reiniciar nivel)
    }
}