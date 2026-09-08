
using UnityEngine;

public class PlayerStats : MonoBehaviour
{


    [SerializeField] private PlayerData playerData;

    [SerializeField] private int maxHealth;
    [SerializeField] private int health;
    [SerializeField] private int armor;
    [SerializeField] private int healthRegeneration;

    //Daño
    [SerializeField] private int damage;
    [SerializeField] private int critDamage;
    [SerializeField] private int magicDamage;
    [SerializeField] private int criticalChance;
    //Varios

    [SerializeField] private int speed;
    [SerializeField] private int dodgeChance;
    [SerializeField] private int harvesting;
    [SerializeField] private int curse;

  

    //Gets de Vida
    public int Health => health;
    public int Armor => armor;

    public int HealthRegeneration => healthRegeneration;

    //Gets de daño

    public int CritDamage => critDamage;
    public int MagicDamage => magicDamage;

    public int CriticalChance => criticalChance;

    public int Damage => damage;



    //Gets de varios
    public int Speed => speed;
    public int DodgeChance => dodgeChance;
    public int Harvesting => harvesting;
    public int Curse => curse;

    private void Awake()
    {
        Initialize();
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
        damage = playerData.BaseDamage;
        speed = playerData.BaseSpeed ;
        healthRegeneration = playerData.BaseHealthRegeneration;
        critDamage = playerData.BaseCritDamage;
        magicDamage = playerData.BaseMagicDamage;
        criticalChance = playerData.BaseCriticalChance;
        dodgeChance = playerData.BaseDodgeChance;
        curse= playerData.BaseCurse;
        harvesting=playerData.BaseHarvesting;

    }

    public void SetHealth(int value)
    {
        // para q la vida no se ponga en negativo, si eso llega a pasar se pone en 0, metodos de seguridad amigo!
        health = Mathf.Clamp(value, 0, maxHealth);
    }


    public void AddDamage(int amount)
    {
        damage += amount;
    }
    public void AddArmor(int amount)
    {
        armor += amount;
    }
    public void AddSpeed(int amount)
    {
        speed += amount;
    }
    public void AddCriticalChance(int amount)
    {
        criticalChance += amount;
    }
    public void AddDodgeChance(int amount)
    {
        dodgeChance += amount;
    }

    public void AddMaxHealth(int amount)
    {
        maxHealth += amount;
        health = Mathf.Clamp(health + amount, 0, maxHealth);
    }

    public void AddHarvesting(int amount)
    {
        harvesting += amount;
    }

    public void Addcurse(int amount)
    {
        curse += amount;
    }
    //AMIGO LO SIENTO ESTE ES MI UNICO ERROR
}



