using ED262C;
using UnityEngine;

public class InventaryWeapons : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image[] toTheList; //array de imagenes
    [SerializeField] SimpleArrayList<UnityEngine.UI.Image> invImages = new SimpleArrayList<UnityEngine.UI.Image>(); //lista con las imagenes que s erellena con el array
    SimpleArrayList<IWeapon> weapons = new SimpleArrayList<IWeapon>(); //lista de armas
    int ItemActual = 0;

    
    void Start()
    {
        for (int i = 0; i < toTheList.Length; i++)
        {
            invImages.Add(toTheList[i]);
        }
        UpdateSelection();
    }
    public IWeapon CurrentWeapon() //esto te dice el comportamiento del arma dependiendo de cual tenes en la mano en ese momento
    {
        if(weapons.Count > 0 && ItemActual < weapons.Count)
        {
            return weapons[ItemActual];
        }
        else
        {
            return null;
        }
    }
    

    void Update()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.ControlsBlocked) return;
        if (invImages.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.Backspace)) Debug.Log("soy el slot " + ItemActual);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ItemActual--;
            if (ItemActual < 0)
            {
                ItemActual = invImages.Count - 1;
                ItemActual = weapons.Count - 1;
            }

           
            UpdateSelection();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ItemActual++;
            if (ItemActual > invImages.Count - 1)
            {
                ItemActual = invImages.Count - 1;
                ItemActual = 0;
            }
            UpdateSelection();
        }
    }

    public void AddWeapon(IWeapon newWeapon)
    {
        if (newWeapon == null)
        {
            Debug.LogError("AddWeapon recibió un IWeapon NULL");
            return;
        }
        Debug.Log("Agregando arma: " + newWeapon);

        weapons.Add(newWeapon);
        ItemActual = weapons.Count - 1;

        Debug.Log("Cantidad armas: " + weapons.Count);
        Debug.Log("ItemActual: " + ItemActual);

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
            if (weapons[i] == null)
            {
                Debug.LogError("El arma " + i + " es NULL");
                continue;
            }

            Transform weaponTransform = weapons[i].getTransform();

            if (weaponTransform == null)
            {
                Debug.LogError("getTransform() devolvió NULL en arma " + i);
                continue;
            }

            weaponTransform.gameObject.SetActive(i == ItemActual);

            
        }
    }
    public bool HasWeapon(string weaponId)
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            // es algo similar a un cast, pero con seguridad de tipo. Si weapons[i] es un WeaponController, lo asigna a wc; si no, wc será null.
            WeaponController wc = weapons[i] as WeaponController;
            if (wc != null && wc.WeaponData.IdWeapon == weaponId)
            {
                return true;
            }
        }
        return false;
    }

  
}