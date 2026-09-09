using ED262C;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image[] toTheList;
    [SerializeField] SimpleArrayList<UnityEngine.UI.Image> invImages = new SimpleArrayList<UnityEngine.UI.Image>();
    SimpleArrayList<IWeapon> weapons = new SimpleArrayList<IWeapon>();
    int ItemActual = 0;

    public IWeapon CurrentWeapon => weapons.Count > 0 ? weapons[ItemActual] : null;

    void Start()
    {
        for (int i = 0; i < toTheList.Length; i++)
        {
            invImages.Add(toTheList[i]);
        }
        UpdateSelection();
    }

    void Update()
    {
        if (invImages.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.Backspace)) Debug.Log("soy el slot " + ItemActual);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ItemActual--;
            if (ItemActual < 0) ItemActual = invImages.Count - 1;
            UpdateSelection();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ItemActual++;
            if (ItemActual > invImages.Count - 1) ItemActual = 0;
            UpdateSelection();
        }
    }

    public void AddWeapon(IWeapon newWeapon)
    {
        weapons.Add(newWeapon);
        ItemActual = weapons.Count - 1;

        WeaponController weaponController = newWeapon as WeaponController;
        if (weaponController != null && ItemActual < invImages.Count)
        {
            invImages[ItemActual].sprite = weaponController.WeaponData.WeaponSprite;
        }

        UpdateSelection();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < invImages.Count; i++)
        {
            if (i == ItemActual)
            {
                invImages[i].color = Color.blue;
            }
            else
            {
                invImages[i].color = Color.white;
            }
        }

        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].getTransform().gameObject.SetActive(i == ItemActual);
        }
    }
}