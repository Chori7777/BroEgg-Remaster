
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStats playerStats;

   
    private float regenTimer;

    
    public void TakeDamage(int damage)
    {
        bool dodged=Random.Range(0,100)<playerStats.DodgeChance;
        if(dodged)
        {
            Debug.Log("GG Ez no tuve ni que prender el monitor");
            return;
        }
        float armorReduction = Mathf.Clamp(playerStats.Armor, 0, 90);

        int finalDamage = damage;
        finalDamage -= Mathf.RoundToInt(damage * (armorReduction / 100f));
      playerStats.SetHealth(playerStats.Health - finalDamage);
        if (playerStats.Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Murio el jugador");
    }

    void Update()
    {
        regenTimer += Time.deltaTime;
        if (regenTimer >= 1f)
        {
            playerStats.SetHealth(playerStats.Health + playerStats.HealthRegeneration);
            regenTimer = 0f;
        }
    }

}
