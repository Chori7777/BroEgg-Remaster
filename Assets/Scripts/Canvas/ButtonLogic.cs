using UnityEngine;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;

    private ShopProductData product;

    public void Setup(ShopProductData product, ShopManager manager)
    {
        this.product = product;
        shopManager = manager;

        gameObject.SetActive(product != null);

        if (product == null)
        {
            return;
        }

        Button button = GetComponent<Button>();
        button.interactable = true;
        button.image.sprite = product.icon;
    }

    public void BuyProduct()
    {
        if (shopManager == null)
        {
            return;
        }

        bool bought = shopManager.TryBuy(product);

        if (bought)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
