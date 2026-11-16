using TMPro;
using UnityEngine;

public class StatsHUD : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private PlayerGold gold;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text armorText;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private TMP_Text critText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text dodgeText;
    [SerializeField] private TMP_Text harvestText;
    [SerializeField] private TMP_Text playerGold;

    private void OnEnable()
    {
        // Los prefabs no pueden guardar referencias a objetos de una escena.
        // Resolvemos estos datos al activarse, si no fueron asignados en el Inspector.
        if (stats == null) stats = FindFirstObjectByType<PlayerStats>();
        if (gold == null) gold = FindFirstObjectByType<PlayerGold>();

        if (stats != null) stats.OnStatsChanged += Refresh;
        if (gold != null) gold.OnGoldChanged += RefreshGold;   

        Refresh();
        if (gold != null) RefreshGold(gold.CurrentGold);
    }

    private void OnDisable()
    {
        if (stats != null) stats.OnStatsChanged -= Refresh;
        if (gold != null) gold.OnGoldChanged -= RefreshGold;
    }

    private void Refresh()
    {
        if (stats == null) return;

        if (healthText != null) healthText.text = "Vida: " + stats.MaxHealth;
        if (armorText != null) armorText.text = "Armadura: " + stats.Armor;
        if (powerText != null) powerText.text = "Poder: " + stats.Damage;
        if (critText != null) critText.text = "% Critico: " + stats.CriticalChance;
        if (speedText != null) speedText.text = "Velocidad: " + stats.Speed;
        if (dodgeText != null) dodgeText.text = "Esquiva: " + stats.DodgeChance;
        if (harvestText != null) harvestText.text = "Recoleccion: " + stats.Harvesting;
    }
 
    private void RefreshGold(int nuevoValorDeOro) 
    {
        if (playerGold != null)
            playerGold.text = "" + nuevoValorDeOro;
    }
}
