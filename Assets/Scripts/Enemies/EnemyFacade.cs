using UnityEngine;


public class EnemyFacade : MonoBehaviour
{
    private HitFlash hitFlash;

    private void Awake()
    {
        hitFlash = GetComponent<HitFlash>();
    }

    public void OnHit()
    {
        if (hitFlash != null) hitFlash.Flash();
        //PlaySound(SoundID.EnemyHit);
    }

    public void OnDeath()
    {
        AchievementsManager.Instance.killCount++;
        //PlaySound(SoundID.EnemyDeath);
    }

    private void PlaySound(SoundID id)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(id);
    }
}