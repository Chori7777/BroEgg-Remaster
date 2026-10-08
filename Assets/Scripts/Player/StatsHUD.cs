using TMPro;
using UnityEngine;

public class StatsHUD : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text armorText;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private TMP_Text critText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text dodgeText;
    [SerializeField] private TMP_Text harvestText;

    private void OnEnable()
    {
        stats.OnStatsChanged += Refresh;
        Refresh(); // Awake de PlayerStats ya disparo el evento antes de suscribirnos
    }

    private void OnDisable()
    {
        stats.OnStatsChanged -= Refresh;
    }

    private void Refresh()
    {
        healthText.text = "Vida: " +  stats.MaxHealth;
        armorText.text = "Armadura: " + stats.Armor;
        powerText.text = "Poder: " + stats.Damage;
        critText.text = "% Critico: " + stats.CriticalChance;
        speedText.text = "Velocidad: " + stats.Speed;
        dodgeText.text = "Esquiva: " + stats.DodgeChance;
        harvestText.text = "Recoleccion: " + stats.Harvesting;
    }
}