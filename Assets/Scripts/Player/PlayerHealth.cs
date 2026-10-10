
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStats playerStats;

   
   [SerializeField] private float regenTimer;
    // Evento para notificar cambios en la salud del jugador
    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDied;
    private bool isDead;
    // Evento para notificar cuando el jugador recibe daño
    public event Action OnDamaged;

    private void OnEnable()
    {
        playerStats.OnStatsChanged += NotifyHealthChanged;
    }

    private void OnDisable()
    {
        playerStats.OnStatsChanged -= NotifyHealthChanged;
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(playerStats.Health, playerStats.MaxHealth);
    }

    private void Start()
    {
        // Inicializar la salud del jugador al inicio del juego
        OnHealthChanged?.Invoke(playerStats.Health, playerStats.MaxHealth);
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;
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
        OnDamaged?.Invoke();
        if (playerStats.Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Murio el jugador");
        OnPlayerDied?.Invoke();
    }

    public void Revive()
    {
        isDead = false;
        regenTimer = 0f;
        OnHealthChanged?.Invoke(playerStats.Health, playerStats.MaxHealth);
    }

    [ContextMenu("DEBUG: Matar jugador")]
    private void DebugKill()
    {
        playerStats.SetHealth(0);
        OnHealthChanged?.Invoke(playerStats.Health, playerStats.MaxHealth);
        Die();
    }

    void Update()
    {
        if (isDead) return;
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
