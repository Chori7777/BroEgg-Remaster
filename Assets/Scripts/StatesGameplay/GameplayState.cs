using UnityEngine;

public class GameplayState : IState
{
    LevelManager levelManager;
    float spawnTimer;

    public GameplayState(LevelManager levelManager) { this.levelManager = levelManager; }

    public void Enter()
    {
        levelManager.AdvanceToNextWave();
        levelManager.TimeRemaining = levelManager.CurrentWave.Time;
        spawnTimer = levelManager.CurrentWave.WaveRate;
    }

    public void UpdateState()
    {
        levelManager.TimeRemaining -= Time.deltaTime;

        if (levelManager.TimeRemaining <= 0)
        {
            levelManager.levelStateMachine.ChangeState(levelManager.levelStateMachine.Shop);
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            levelManager.enemySpawnerManager.spawnWave(levelManager.CurrentWave.EnemiesPerWave, levelManager.CurrentWave.enemyProbabilities);
            spawnTimer = levelManager.CurrentWave.WaveRate;
        }
    }

    public void Exit()
    {
    }
}