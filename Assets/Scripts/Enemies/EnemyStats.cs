using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;

    [SerializeField] private int health;
    [SerializeField] private int armor;
    [SerializeField] private int damage;
    [SerializeField] private int speed;
    [SerializeField] private int enemyGoldDrop;
    [SerializeField] private bool isBoss;

   

    //Gets

    public int Health => health;
    public int Armor => armor;
    public int Speed => speed; 
    public int EnemyGoldDrop => enemyGoldDrop;
    public int Damage=> damage;

    // Initialize se usa en dos scripts (en este y en Enemy) para inicializar las stats del enemigo, y se le pasa el round para poder escalar las stats segun el round
    // Y mas q nada tambien para separar responsabilidades, ya que el script Enemy se encarga de manejar el comportamiento del enemigo
    // mientras que este script se encarga de manejar las stats del enemigo
    public void Initialize(int round)
    {
        if(enemyData==null)
        {
            Debug.LogError("EnemyData no asignado en " + gameObject.name);
            return;
        }

        health = Mathf.RoundToInt(enemyData.BaseHealth*(1+ (enemyData.HealthScaling/100f)*(round-1)));
        armor = Mathf.RoundToInt(enemyData.BaseArmor*(1+ (enemyData.ArmorScaling/100f)*(round-1)));
        damage = Mathf.RoundToInt(enemyData.BaseDamage*(1+ (enemyData.DamageScaling/100f)*(round-1)));
        speed = Mathf.RoundToInt(enemyData.BaseSpeed*(1+ (enemyData.SpeedScaling/100f)*(round-1)));
        enemyGoldDrop= Mathf.RoundToInt(enemyData.GoldDrop*(1+ (enemyData.GoldDropScaling/100f)*(round-1)));

        isBoss = enemyData.IsBoss;
    }
    public void TakeDamage(int damage)
    {
        float armorReduction = Mathf.Clamp(armor, 0, 90);

       int finalDamage = damage;
        finalDamage-= Mathf.RoundToInt(damage * (armorReduction / 100f));
        health -= finalDamage;

    }

}

