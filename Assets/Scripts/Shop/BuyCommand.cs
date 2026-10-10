using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    public bool Undo();
    public bool Execute();
}

public class BuyCommand : ICommand
{
    private readonly ShopProductData product;
    private readonly PlayerGold gold;
    private readonly InventaryManager inventory;
    private readonly PlayerWeaponManager weaponManager;
    private readonly HashSet<ShopProductData> availableOffers;

    // Estado que se guarda al ejecutar para poder deshacer
    private int paidPrice;
    private WeaponController deliveredWeapon;
    private bool executed;

    public BuyCommand(ShopProductData product,PlayerGold gold,InventaryManager inventory,PlayerWeaponManager weaponManager,HashSet<ShopProductData> availableOffers)
    {
        this.gold = gold;
        this.product = product;
        this.inventory = inventory;
        this.weaponManager = weaponManager;
        this.availableOffers = availableOffers;
    }

    public bool Execute() //esta funcion guarda el oro de lo que se compro y lo remueve de las ofertas
    {
        if (executed) return false;
        if (product == null) return false;

        if (!gold.SpendGold(product.price))
        {
            return false;
        }

        // Guardamos lo que realmente se pago
        paidPrice = product.price;

        if (!Deliver())
        {
            gold.AddGold(paidPrice);
            return false;
        }

        availableOffers.Remove(product);
        executed = true;
        return true;
    }

    public bool Undo() //paso inverso al Execute(), en vez de Remove() de offers, hace Add(), y en vez de SpendGold(), hace AddGold()
    {
        if (!executed) return false;

        // Primero se quita el producto: si falla (por ejemplo, el objeto ya se vendio)
        // no se devuelve el oro, asi no se puede explotar vender y despues reembolsar.
        if (!Remove())
        {
            return false;
        }

        availableOffers.Add(product);

        // El oro se devuelve al final: OnGoldChanged refresca los botones de la tienda
        // y en ese momento el inventario ya tiene que estar actualizado.
        gold.AddGold(paidPrice);

        executed = false;
        return true;
    }

    private bool Deliver()
    {
        if (product is ObjectShopData item)
        {
            Debug.Log("objeto añadido al jugador");
            return inventory.TryAdd(item);
            
        }

        if (product is WeaponShopData weapon)
        {
            string weaponId = weapon.weaponPrefab.WeaponData.IdWeapon;
            deliveredWeapon = weaponManager.GiveWeapon(weaponId);
            Debug.Log("arma añadido al jugador");
            return deliveredWeapon != null;
        }

        return false;
    }

    private bool Remove()
    {
        if (product is ObjectShopData item)
        {
            return inventory.TryRemove(item.id);
        }

        if (product is WeaponShopData)
        {
            return deliveredWeapon != null && weaponManager.RemoveWeapon(deliveredWeapon);
        }

        return false;
    }
}
