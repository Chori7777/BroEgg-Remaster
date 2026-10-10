using System.Collections.Generic;

public sealed class GameMemento // Guarda ronda, oro stats y los ids de las armas
{
    public readonly int Round; // readonly significa solo lectura
    public readonly int Gold;
    public readonly PlayerStatsSnapshot Stats;
    private readonly string[] weaponIds; // Si guardara la referencia al objeto se destruiran al morir por eso guardo las ids para al volver crear las armas 

    public IReadOnlyList<string> WeaponIds => weaponIds; 

    public GameMemento(int round, int gold, PlayerStatsSnapshot stats, string[] weaponIds)
    {
        Round = round;
        Gold = gold;
        Stats = stats;
        this.weaponIds = (string[])weaponIds.Clone();  // Copia del array, si guardo el array original y despues se modifica, el guarado cambiaria 
    }
}

public readonly struct PlayerStatsSnapshot  // Guardo los datos en un struct 
{
    public readonly int MaxHealth, Health, Armor, HealthRegeneration;
    public readonly int Damage, CritDamage, MagicDamage, CriticalChance;
    public readonly int Speed, DodgeChance, Harvesting, Curse;

    //public PlayerStatsSnapshot(int maxHealth, int health, int armor, int healthRegeneration,
    //                           int damage, int critDamage, int criticalChance,
    //                           int speed, int dodgeChance, int harvesting, int curse)
    //{
    //    MaxHealth = maxHealth; Health = health; Armor = armor; HealthRegeneration = healthRegeneration;
    //    Damage = damage; CritDamage = critDamage; CriticalChance = criticalChance;
    //    Speed = speed; DodgeChance = dodgeChance; Harvesting = harvesting; Curse = curse;
    //}
}