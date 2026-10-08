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
        if (stats != null) stats.OnStatsChanged += Refresh;
        if (gold != null) gold.OnGoldChanged += RefreshGold;   

        Refresh(); // lo llamamos para que se refresque al iniciar el juego, y no esperar a que cambien los stats
        if (gold != null) RefreshGold(gold.CurrentGold);
    }

    private void OnDisable()
    {
        if (stats != null) stats.OnStatsChanged -= Refresh;
        if (gold != null) gold.OnGoldChanged -= RefreshGold;
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
 
    private void RefreshGold(int nuevoValorDeOro) 
    {
        if (playerGold != null)
            playerGold.text = "" + nuevoValorDeOro;
    }
}