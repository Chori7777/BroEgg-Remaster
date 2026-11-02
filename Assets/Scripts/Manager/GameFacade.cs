using UnityEngine;

public static class GameFacade 
{
    public static void AdvanceWave()
    {
        LevelManager level = LevelManager.Instance;
        level.AdvanceToNextWave();
        AchievementsManager.Instance.waveCount++;
        WaveData wave = level.CurrentWave;
        if (wave != null && wave.music != SoundID.None && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(wave.music);
        }
    }

}
