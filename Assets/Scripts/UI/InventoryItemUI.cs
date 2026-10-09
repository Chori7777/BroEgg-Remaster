using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameLabel;

    public void Setup(ObjectShopData item)
    {
        if (icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = item.icon != null;
        }
        if (nameLabel != null) nameLabel.text = item.displayName;
    }
}
