using UnityEngine;
using UnityEngine.Rendering;

public class GameplayState : IState
{

    LevelManager levelManager;
    public GameplayState( LevelManager levelManager ) {  this.levelManager = levelManager; }

   

    public void Enter()
    {
        LevelManager.Instance.TimeRemaining = LevelManager.Instance.CurrentWave.Time;
    }

    public void UpdateState()
    {
        LevelManager.Instance.TimeRemaining -= Time.deltaTime;

        if(LevelManager.Instance.TimeRemaining == 0) LevelManager.Instance.TimeRemaining = LevelManager.Instance.CurrentWave.Time;
    }

    public void Exit()
    {

    }
}
