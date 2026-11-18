using UnityEngine;

public static class GameFacade 
{
    public static void AdvanceWave()
    {
        LevelManager level = LevelManager.Instance;
        level.AdvanceToNextWave();
        AchievementsManager.Instance.waveCount++;
        WaveData wave = level.CurrentWave;
        
    }

}
