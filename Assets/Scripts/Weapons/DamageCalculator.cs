using UnityEngine;

public static class DamageCalculator
{
    // Me van a querer matar, pero dejenme explicarme
    // creo una variable static para usarla en cada arma,es mas reutilizable y nos inpide tener que crear una instancia de DamageCalculator cada vez que queramos calcular el daño
    // asi que es mas automatico, y nada es estatica para poder tener un estado global, y no tener que pasarle el playerstats a cada arma
    // y asi poder calcular el daño de cada arma sin tener que crear una instancia de DamageCalculator cada vez que queramos calcular el daño
    public enum DamageType { Normal, Magic }

    public static int CalculateDamage(PlayerStats stats, DamageType type, int weaponDamage)
    {
        int baseDamage = (type == DamageType.Normal ? stats.Damage : stats.MagicDamage) + weaponDamage;
        bool isCritical = Random.Range(0, 100) < stats.CriticalChance;
        return isCritical ? baseDamage + stats.CritDamage : baseDamage;
    }
}