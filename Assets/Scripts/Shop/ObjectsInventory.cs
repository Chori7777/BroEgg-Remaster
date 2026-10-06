using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Este catálogo contiene los productos de la tienda, no los objetos del jugador.
public class ObjectsInventory : MonoBehaviour
{
    [FormerlySerializedAs("objectList")]
    public List<ShopProductData> shopList = new List<ShopProductData>();

    public List<ShopProductData> GenerateOffers(int count, InventaryManager inventory)
    {
        List<ShopProductData> candidates = GetAvailableProducts(inventory);
        List<ShopProductData> offers = new List<ShopProductData>();

        while (offers.Count < count && candidates.Count > 0)
        {
            int randomIndex = Random.Range(0, candidates.Count);
            ShopProductData selectedProduct = candidates[randomIndex];

            offers.Add(selectedProduct);

            // Se retira de la lista temporal para que no aparezca en otra oferta.
            candidates.RemoveAt(randomIndex);
        }

        return offers;
    }

    private List<ShopProductData> GetAvailableProducts(InventaryManager inventory)
    {
        List<ShopProductData> candidates = new List<ShopProductData>();
        HashSet<string> includedIds = new HashSet<string>();

        foreach (ShopProductData product in shopList)
        {
            if (product == null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(product.id))
            {
                continue;
            }

            if (product.price < 0)
            {
                continue;
            }

            if (product is ObjectShopData && inventory.Contains(product.id))
            {
                continue;
            }

            if (!includedIds.Add(product.id))
            {
                continue;
            }

            candidates.Add(product);
        }

        return candidates;
    }
}
