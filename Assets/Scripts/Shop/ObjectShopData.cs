using UnityEngine;

[CreateAssetMenu(fileName = "ObjectShopData", menuName = "Scriptable Objects/ObjectShopData")]
public class ObjectShopData : ShopProductData
{
    [SerializeField] private int baseHealth;
    [SerializeField] private int baseArmor;
    [SerializeField] private int baseDamage;
    [SerializeField] private int baseSpeed;
}
