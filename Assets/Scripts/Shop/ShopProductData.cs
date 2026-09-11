using UnityEngine;

[CreateAssetMenu(fileName = "ShopProductData", menuName = "Scriptable Objects/ShopProductData")]
public abstract class ShopProductData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public int price;
}

