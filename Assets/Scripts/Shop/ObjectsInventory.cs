using System.Collections.Generic;
using ED262C;
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

        /*// Se arma el arbol con los candidatos de esta tienda
        BST<ShopProductData> byPrice = new BST<ShopProductData>(ComparePrice);
        for (int i = 0; i < candidates.Count; i++) byPrice.Insert(candidates[i]);

        // DFS in-order: lo que el jugador puede pagar, del mas barato al mas caro
        SimpleArrayList<ShopProductData> affordable = byPrice.InOrder(p => p.price <= playerGold);

        // Primera oferta garantizada: algo que pueda pagar
        if (count > 0 && affordable.Count > 0)
        {
            ShopProductData guaranteed = affordable[Random.Range(0, affordable.Count)];
            offers.Add(guaranteed);
            candidates.Remove(guaranteed);
        }*/


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
