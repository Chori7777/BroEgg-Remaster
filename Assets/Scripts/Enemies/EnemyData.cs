using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private int baseHealth;
    [SerializeField] private int baseArmor;
    [SerializeField] private int baseDamage;
    [SerializeField] private int baseSpeed;
    [SerializeField] private int goldDrop;

    [SerializeField] private bool isBoss;

    [SerializeField] private int healthScaling;
    [SerializeField] private int armorScaling;
    [SerializeField] private int damageScaling;
    [SerializeField] private int speedScaling;
    [SerializeField] private int goldDropScaling;



    //Gets


    public int BaseHealth => baseHealth;
    public int BaseArmor => baseArmor;
    public int BaseDamage => baseDamage;
    public int BaseSpeed => baseSpeed;

    public int GoldDrop  => goldDrop;

    public int HealthScaling => healthScaling;
    public int ArmorScaling => armorScaling;
    public int DamageScaling => damageScaling;
    public int SpeedScaling => speedScaling;

    public int GoldDropScaling => goldDropScaling;
    public bool IsBoss => isBoss;

}
