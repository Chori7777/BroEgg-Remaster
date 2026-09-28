using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    private ShopProductData product;
    [SerializeField] private PlayerWeaponManager weaponmanager;

    public void Setup(ShopProductData product)
    {
        this.product = product;
    }

    public void BuyProduct()
    {
        if (product is WeaponShopData weapon)
        {
            Debug.Log("compre arma");
            weaponmanager.GiveWeapon(weapon.weaponPrefab.WeaponData.IdWeapon);
        }
        else if (product is ObjectShopData obj)
        {
            Debug.Log("compre objeto");
        }
    }

}
