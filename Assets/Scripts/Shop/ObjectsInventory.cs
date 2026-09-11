using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectsInventory : MonoBehaviour
{
    public List<ShopProductData> shopList = new List<ShopProductData>();
    public Dictionary<string, ShopProductData> shopDictionary = new Dictionary<string, ShopProductData>();


    //Se aplica toda la lista de objetos en el diccionario
    void Start()
    {
        for (int i = 0; i < shopList.Count; i++)
        {
            shopDictionary.Add(shopList[i].id, shopList[i]);

        }
    }
    public ShopProductData ChooseObject()
    {
        if (shopDictionary.Count == 0)
        { return null; }

        int randomIndex = Random.Range(0, shopDictionary.Count);
        return shopDictionary.Values.ElementAt(randomIndex);
        //esta linea agarra todos los valores del diccionario en el "Values" y con elementAt en randomindex agarra un objeto random de los valores q reconocio antes en Values
    }


    void Update()
    {

    }
}
