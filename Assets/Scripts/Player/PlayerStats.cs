using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private InventaryManager objectInventory;

    // Vida
    [SerializeField] private int maxHealth;
    [SerializeField] private int health;
    [SerializeField] private int armor;
    [SerializeField] private int healthRegeneration;

    // Daño
    [SerializeField] private int damage;
    [SerializeField] private int critDamage;
    [SerializeField] private int criticalChance;

    // Otros atributos
    [SerializeField] private int speed;
    [SerializeField] private int dodgeChance;
    [SerializeField] private int harvesting;
    [SerializeField] private int curse;

    // Bonificaciones del inventario que ya están incluidas en las stats actuales.
    private int appliedMaxHealthBonus;
    private int appliedArmorBonus;
    private int appliedDamageBonus;
    private int appliedSpeedBonus;
    private int appliedHealthRegenBonus;
    private int appliedCritDamageBonus;
    private int appliedCritChanceBonus;
    private int appliedDodgeBonus;
    private int appliedHarvestingBonus;
    private int appliedCurseBonus;

    public int Health => health;
    public int MaxHealth => Mathf.Max(1, maxHealth);
    public int Armor => armor;
    public int HealthRegeneration => healthRegeneration;

    public int Damage => damage;
    public int CritDamage => critDamage;
    public int CriticalChance => criticalChance;

    public int Speed => Mathf.Max(0, speed);
    public int DodgeChance => dodgeChance;
    public int Harvesting => harvesting;
    public int Curse => curse;

    public event Action OnStatsChanged;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        if (objectInventory != null)
        {
            objectInventory.OnInventoryChanged += RecalculateObjectBonuses;
        }

        RecalculateObjectBonuses();
    }

    private void OnDisable()
    {
        if (objectInventory != null)
        {
            objectInventory.OnInventoryChanged -= RecalculateObjectBonuses;
        }
    }

    public void Initialize()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData no asignado en " + gameObject.name);
            return;
        }

        maxHealth = playerData.BaseHealth;
        health = playerData.BaseHealth;
        armor = playerData.BaseArmor;
        healthRegeneration = playerData.BaseHealthRegeneration;

        damage = playerData.BaseDamage;
        critDamage = playerData.BaseCritDamage;
        criticalChance = playerData.BaseCriticalChance;

        speed = playerData.BaseSpeed;
        dodgeChance = playerData.BaseDodgeChance;
        harvesting = playerData.BaseHarvesting;
        curse = playerData.BaseCurse;

        appliedMaxHealthBonus = 0;
        appliedArmorBonus = 0;
        appliedDamageBonus = 0;
        appliedSpeedBonus = 0;

        RecalculateObjectBonuses();
    }

    public void RecalculateObjectBonuses()
    {
        if (playerData == null) return;
        if (objectInventory == null) return;

        int maxHealthBonus = 0;
        int armorBonus = 0;
        int damageBonus = 0;
        int speedBonus = 0;

        int healthRegenBonus = 0;
        int critDamageBonus = 0;
        int critChanceBonus = 0;
        int dodgeBonus = 0;
        int harvestingBonus = 0;
        int curseBonus = 0;

        foreach (ObjectShopData item in objectInventory.GetObjects())
        {
            maxHealthBonus += item.BonusMaxHealth;
            armorBonus += item.BonusArmor;
            damageBonus += item.BonusDamage;
            speedBonus += item.BonusSpeed;

            healthRegenBonus += item.BonusHealthRegeneration;
            critDamageBonus += item.BonusCritDamage;
            critChanceBonus += item.BonusCriticalChance;
            dodgeBonus += item.BonusDodgeChance;
            harvestingBonus += item.BonusHarvesting;
            curseBonus += item.BonusCurse;
        }
        // Reemplaza la contribucion anterior del inventario por la nueva.
        // Asi se conservan las mejoras obtenidas por otros medios.
        maxHealth = maxHealth - appliedMaxHealthBonus + maxHealthBonus;
        armor = armor - appliedArmorBonus + armorBonus;
        damage = damage - appliedDamageBonus + damageBonus;
        speed = speed - appliedSpeedBonus + speedBonus;

        // Todos los nuevos cambios de stats que se aplican por el inventario se suman a los valores actuales.
        healthRegeneration = healthRegeneration - appliedHealthRegenBonus + healthRegenBonus;
        critDamage = critDamage - appliedCritDamageBonus + critDamageBonus;
        criticalChance = criticalChance - appliedCritChanceBonus + critChanceBonus;
        dodgeChance = dodgeChance - appliedDodgeBonus + dodgeBonus;
        harvesting = harvesting - appliedHarvestingBonus + harvestingBonus;
        curse = curse - appliedCurseBonus + curseBonus;

        appliedMaxHealthBonus = maxHealthBonus;
        appliedArmorBonus = armorBonus;
        appliedDamageBonus = damageBonus;
        appliedSpeedBonus = speedBonus;

        appliedHealthRegenBonus = healthRegenBonus;
        appliedCritDamageBonus = critDamageBonus;
        appliedCritChanceBonus = critChanceBonus;
        appliedDodgeBonus = dodgeBonus;
        appliedHarvestingBonus = harvestingBonus;
        appliedCurseBonus = curseBonus;

        // Cambiar la vida maxima no cura, limita la vida actual al nuevo maximo.
        health = Mathf.Clamp(health, 0, MaxHealth);

        OnStatsChanged?.Invoke();
    }

    //public PlayerStatsSnapshot CreateSnapshot()
    //{
    //    return new PlayerStatsSnapshot(maxHealth, health, armor, healthRegeneration,
    //                                   damage, critDamage, criticalChance,
    //                                   speed, dodgeChance, harvesting, curse);
    //}

    public void RestoreSnapshot(PlayerStatsSnapshot s)
    {
        maxHealth = s.MaxHealth; health = s.Health; armor = s.Armor; healthRegeneration = s.HealthRegeneration;
        damage = s.Damage; critDamage = s.CritDamage;  criticalChance = s.CriticalChance;
        speed = s.Speed; dodgeChance = s.DodgeChance; harvesting = s.Harvesting; curse = s.Curse;
    }

    public void SetHealth(int value)
    {
        health = Mathf.Clamp(value, 0, MaxHealth);
    }

    public void AddDamage(int amount)
    {
        damage += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddArmor(int amount)
    {
        armor += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddSpeed(int amount)
    {
        speed += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddCriticalChance(int amount)
    {
        criticalChance += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddDodgeChance(int amount)
    {
        dodgeChance += amount;
        OnStatsChanged?.Invoke();
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        health = Mathf.Clamp(health + amount, 0, MaxHealth);
        OnStatsChanged?.Invoke();
    }

    public void AddHarvesting(int amount)
    {
        harvesting += amount;
        
        OnStatsChanged?.Invoke();
    }

    public void Addcurse(int amount)
    {
        curse += amount;
    }
}
