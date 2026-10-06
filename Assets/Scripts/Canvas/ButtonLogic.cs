using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    private ShopProductData product;
    [SerializeField] private PlayerWeaponManager weaponManager;

    public void Setup(ShopProductData product)
    {
        this.product = product;
    }

    public void BuyProduct()
    {
        if (product is WeaponShopData weapon)
        {
            Debug.Log("compre arma");
            weaponManager.GiveWeapon(weapon.weaponPrefab.WeaponData.IdWeapon);
        }
        else if (product is ObjectShopData obj)
        {
            Debug.Log("compre objeto");
        }
    }

}
