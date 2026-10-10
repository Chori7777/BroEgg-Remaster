using System.Collections.Generic;
using ED262C;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private ObjectsInventory catalog;
    [SerializeField] private InventaryManager inventory;
    [SerializeField] private PlayerGold gold;
    [SerializeField] private PlayerWeaponManager weaponManager;
    [SerializeField] private ButtonLogic[] offerButtons;

    private readonly HashSet<ShopProductData> availableOffers = new HashSet<ShopProductData>();

    public InventaryManager Inventory => inventory;
    public bool IsOpen { get; private set; }

    private SimpleArrayStack<ICommand> refundProducts = new SimpleArrayStack<ICommand>();

    public void Open()
    {
        IsOpen = true;
        availableOffers.Clear();
        refundProducts.Clear();

        List<ShopProductData> products = catalog.GenerateOffers(offerButtons.Length, inventory);

        for (int i = 0; i < offerButtons.Length; i++)
        {
            ShopProductData product = null;

            if (i < products.Count)
            {
                product = products[i];
                availableOffers.Add(product);
            }

            offerButtons[i].Setup(product, this);
        }
    }

    public void Close()
    {
        IsOpen = false;
        refundProducts.Clear();
    }

    public bool TryBuy(ShopProductData product)
    {
        // Validar antes de cobrar evita gastar oro en una compra que no puede realizarse.
        if (!CanBuyProduct(product))
        {
            return false;
        }

        ICommand newProduct = new BuyCommand(product, gold, inventory, weaponManager,availableOffers);

        if (!newProduct.Execute()) return false;

        refundProducts.Push(newProduct);
        availableOffers.Remove(product);
        return true;
    }

    public bool TrySell(string id)
    {
        if (!IsOpen)
        {
            return false;
        }

        if (!inventory.TryGet(id, out ObjectShopData item))
        {
            return false;
        }

        int sellPrice = item.SellPrice;

        // Solo se entrega oro si el objeto se eliminó del inventario.
        if (!inventory.TryRemove(id))
        {
            return false;
        }

        gold.AddGold(sellPrice);
        return true;
    }

    private bool CanBuyProduct(ShopProductData product)
    {
        if (!IsOpen)
        {
            return false;
        }

        if (product == null)
        {
            return false;
        }

        if (!availableOffers.Contains(product))
        {
            return false;
        }

        if (product.price < 0)
        {
            return false;
        }

        if (product is ObjectShopData item)
        {
            return !inventory.Contains(item.id);
        }

        if (product is WeaponShopData weapon)
        {
            return CanBuyWeapon(weapon);
        }

        return false;
    }

    private bool CanBuyWeapon(WeaponShopData product)
    {
        if (weaponManager == null)
        {
            return false;
        }

        if (weaponManager.Hand == null)
        {
            return false;
        }

        if (weaponManager.factoryWeapon == null)
        {
            return false;
        }

        if (weaponManager.inventoryManager == null)
        {
            return false;
        }

        if (product.weaponPrefab == null)
        {
            return false;
        }

        if (product.weaponPrefab.WeaponData == null)
        {
            return false;
        }

        string weaponId = product.weaponPrefab.WeaponData.IdWeapon;

        if (string.IsNullOrWhiteSpace(weaponId))
        {
            return false;
        }

        if (!weaponManager.factoryWeapon.weaponDictionary.TryGetValue(weaponId, out WeaponController weaponPrefab))
        {
            return false;
        }

        return weaponPrefab != null;
    }

    public void OfferReroll()
    {
        if (!IsOpen) return;

        availableOffers.Clear();

        List<ShopProductData> products = catalog.GenerateOffers(offerButtons.Length, inventory);

        for (int i = 0; i < offerButtons.Length; i++)
        {
            ShopProductData product = null;
            if (i < products.Count)
            {
                product = products[i];
                availableOffers.Add(product);
            }
            offerButtons[i].Setup(product, this);
        }
    }

    public void Refund()
    {
        if (!IsOpen) return;

        while (!refundProducts.IsEmpty)
        {
            ICommand last = refundProducts.Pop();
            if (last.Undo()) return;
        }
    }
}
