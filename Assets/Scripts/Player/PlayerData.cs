using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{

    //Vida

    [SerializeField] private int baseHealth;
    [SerializeField] private int baseArmor;
    [SerializeField] private int baseHealthRegeneration;

    //Daño
    [SerializeField] private int baseDamage;
    [SerializeField] private int baseCritDamage;
    [SerializeField] private int baseMagicDamage;
    [SerializeField] private int baseCriticalChance;
    //Varios

    [SerializeField] private int baseSpeed;
    [SerializeField] private int baseDodgeChance;
    [SerializeField] private int baseHarvesting;
    [SerializeField] private int baseCurse;

    //Gets de Vida
    public int BaseHealth => baseHealth;
    public int BaseArmor => baseArmor;

    public int BaseHealthRegeneration => baseHealthRegeneration ;

    //Gets de daño

    public int BaseCritDamage => baseCritDamage;
    public int BaseMagicDamage => baseMagicDamage;

    public int BaseCriticalChance => baseCriticalChance;

    public int BaseDamage => baseDamage;


    //Gets de varios
    public int BaseSpeed => baseSpeed;
    public int BaseDodgeChance => baseDodgeChance;
    public int BaseHarvesting => baseHarvesting;
    public int BaseCurse => baseCurse;
}
