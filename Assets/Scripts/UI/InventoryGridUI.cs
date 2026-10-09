using ED262C;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGridUI : MonoBehaviour
{
    [SerializeField] private InventaryManager inventory;
    [SerializeField] private InventoryItemUI[] cells;
    [SerializeField] private TMP_Text emptyMessage;
    [SerializeField] private TMP_Text pageLabel;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    private readonly SimpleArrayList<ObjectShopData> visibleItems = new SimpleArrayList<ObjectShopData>();
    private int currentPage;
    private int PageCount => cells == null || cells.Length == 0
        ? 1 : Mathf.Max(1, (visibleItems.Count + cells.Length - 1) / cells.Length);

    private void OnEnable()
    {
        if (inventory != null) inventory.OnInventoryChanged += Refresh;
        currentPage = 0;
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null) inventory.OnInventoryChanged -= Refresh;
    }

    public void Refresh()
    {
        visibleItems.Clear();
        if (inventory != null) visibleItems.AddRange(inventory.GetObjects());
        visibleItems.Sort(CompareByRarity);
        currentPage = Mathf.Clamp(currentPage, 0, PageCount - 1);
        ShowPage();
    }

    private static int CompareByRarity(ObjectShopData a, ObjectShopData b)
    {
        // Mayor rareza primero: Legendary, Epic, Uncommon, Comun.
        return b.Rarity.CompareTo(a.Rarity);
    }

    public void PreviousPage()
    {
        currentPage = Mathf.Max(0, currentPage - 1);
        ShowPage();
    }

    public void NextPage()
    {
        currentPage = Mathf.Min(PageCount - 1, currentPage + 1);
        ShowPage();
    }

    private void ShowPage()
    {
        // Las celdas existen en la escena; nunca se instancian elementos UI.
        if (cells != null)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i] == null) continue;
                int itemIndex = currentPage * cells.Length + i;
                bool hasItem = itemIndex < visibleItems.Count;
                cells[i].gameObject.SetActive(hasItem);
                if (hasItem) cells[i].Setup(visibleItems[itemIndex]);
            }
        }
        if (emptyMessage != null) emptyMessage.gameObject.SetActive(visibleItems.Count == 0);
        if (pageLabel != null) pageLabel.text = $"{currentPage + 1} / {PageCount}";
        if (previousButton != null) previousButton.interactable = currentPage > 0;
        if (nextButton != null) nextButton.interactable = currentPage < PageCount - 1;
    }
}
