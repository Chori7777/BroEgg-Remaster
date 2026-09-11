using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    private ShopProductData product;

    public void Setup(ShopProductData product)
    {
        this.product = product;
    }

    public void BuyProduct()
    {
        if (product is WeaponShopData weapon)
        {
            Debug.Log("compre arma");
        }
        else if (product is ObjectShopData obj)
        {
            Debug.Log("compre objeto");
        }
    }

}
