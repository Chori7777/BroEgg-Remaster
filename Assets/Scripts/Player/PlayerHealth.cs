
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStats playerStats;

   
   [SerializeField] private float regenTimer;
    // Evento para notificar cambios en la salud del jugador
    public event Action<int, int> OnHealthChanged;

    private void Start()
    {
        // Inicializar la salud del jugador al inicio del juego
        OnHealthChanged?.Invoke(playerStats.Health, playerStats.MaxHealth);
    }
    public void TakeDamage(int damage)
    {
        bool dodged= UnityEngine.Random.Range(0,100)<playerStats.DodgeChance;
        if(dodged)
        {
            Debug.Log("GG Ez no tuve ni que prender el monitor");
            return;
        }
        float armorReduction = Mathf.Clamp(playerStats.Armor, 0, 90);


        int finalDamage = damage;
        finalDamage -= Mathf.RoundToInt(damage * (armorReduction / 100f));
      playerStats.SetHealth(playerStats.Health - finalDamage);

        OnHealthChanged?.Invoke(playerStats.Health, playerStats.MaxHealth);

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
        if (regenTimer >= 5f)
        {
            playerStats.SetHealth(playerStats.Health + playerStats.HealthRegeneration);
            //Aca tambien deberia avisar al HUD que la salud cambio, asi q hago lo mismo q en TakeDamage
            OnHealthChanged?.Invoke(playerStats.Health, playerStats.MaxHealth);
            regenTimer = 0f;
        }
    }

}
