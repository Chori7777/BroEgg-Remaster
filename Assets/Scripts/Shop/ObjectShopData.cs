using UnityEngine;

[CreateAssetMenu(fileName = "ObjectShopData", menuName = "Scriptable Objects/ObjectShopData")]
public class ObjectShopData : ShopProductData
{ 
    [SerializeField] private int baseHealth;
    [SerializeField] private int baseArmor;
    [SerializeField] private int baseHealthRegeneration;


    [SerializeField] private int baseDamage;
    [SerializeField] private int baseCritDamage;
    [SerializeField] private int baseCriticalChance;


    [SerializeField] private int baseSpeed;
    [SerializeField] private int baseDodgeChance;
    [SerializeField] private int baseHarvesting;
    [SerializeField] private int baseCurse;

    [SerializeField, Range(0f, 1f)] private float sellPriceRatio = 0.5f;

    public int BonusMaxHealth => baseHealth;
    public int BonusArmor => baseArmor;
    public int BonusDamage => baseDamage;
    public int BonusSpeed => baseSpeed;

    public int BonusHealthRegeneration => baseHealthRegeneration;

    public int BonusCritDamage => baseCritDamage;

    public int BonusCriticalChance => baseCriticalChance;

    public int BonusDodgeChance => baseDodgeChance;

    public int BonusHarvesting => baseHarvesting;

    public int BonusCurse => baseCurse;


    public int SellPrice
    {
        get
        {
            int purchasePrice = Mathf.Max(0, price);
            float salePercentage = Mathf.Clamp01(sellPriceRatio);

            return Mathf.FloorToInt(purchasePrice * salePercentage);
        }
    }
}
