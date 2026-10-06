using UnityEngine;

[CreateAssetMenu(fileName = "ObjectShopData", menuName = "Scriptable Objects/ObjectShopData")]
public class ObjectShopData : ShopProductData
{
    [SerializeField] private int baseHealth;
    [SerializeField] private int baseArmor;
    [SerializeField] private int baseDamage;
    [SerializeField] private int baseSpeed;

    [SerializeField, Range(0f, 1f)] private float sellPriceRatio = 0.5f;

    public int BonusMaxHealth => baseHealth;
    public int BonusArmor => baseArmor;
    public int BonusDamage => baseDamage;
    public int BonusSpeed => baseSpeed;

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
