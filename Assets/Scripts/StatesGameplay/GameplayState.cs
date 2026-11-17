using UnityEngine;

public class GameplayState : IState
{
    LevelManager levelManager;
    float spawnTimer;

    public GameplayState(LevelManager levelManager) { this.levelManager = levelManager; }

    public void Enter()
    {
        GameFacade.AdvanceWave();
        levelManager.TimeRemaining = levelManager.CurrentWave.Time;
        spawnTimer = levelManager.CurrentWave.WaveRate;

        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SaveCheckpoint();
        }
    }

    public void UpdateState()
    {
        levelManager.TimeRemaining -= Time.deltaTime;

        if (levelManager.TimeRemaining <= 0)
        {
            // AQUÍ ESTÁ EL CAMBIO CLAVE:
            if (levelManager.HasMoreWaves())
            {
                // Todavía hay juego por delante: ir a la tienda
                levelManager.shopPanel.gameObject.SetActive(true);
                levelManager.levelStateMachine.ChangeState(levelManager.levelStateMachine.Shop);
            }
            else
            {
                // Se acabaron las oleadas: Pantalla de Victoria
                levelManager.levelStateMachine.ChangeState(levelManager.levelStateMachine.Victory);
            }
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            levelManager.enemySpawnerManager.spawnWave(levelManager.CurrentWave.EnemiesPerWave, levelManager.CurrentWave.enemyProbabilities);
            spawnTimer = levelManager.CurrentWave.WaveRate;
        }
    }

    public void Exit() { }
}