using UnityEngine;

[CreateAssetMenu(fileName = "ShopProductData", menuName = "Scriptable Objects/ShopProductData")]
public abstract class ShopProductData : ScriptableObject
{
    public string id;
    public string displayName;
    // Ahora tiene una descripcion...
    public string description;
    public Sprite icon;
    public int price;
}

