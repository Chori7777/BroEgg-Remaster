using ED262C;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class InventaryWeapons : MonoBehaviour
{
    [SerializeField] Image[] toTheList; //array de imagenes
    [SerializeField] SimpleArrayList<UnityEngine.UI.Image> invImages = new SimpleArrayList<Image>(); //lista con las imagenes que s erellena con el array
    SimpleArrayList<IWeapon> weapons = new SimpleArrayList<IWeapon>(); //lista de armas
    int ItemActual = 0;
    private Image[] emptySlotSprites;

    void Start()
    {
        for (int i = 0; i < toTheList.Length; i++)
        {
            invImages.Add(toTheList[i]);
        }

        emptySlotSprites = new Image[toTheList.Length];
        for (int i = 0; i < toTheList.Length; i++)
        {
            if (toTheList[i] != null) emptySlotSprites[i] = toTheList[i];
            emptySlotSprites[i].gameObject.SetActive(false);
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
            if (ItemActual > weapons.Count - 1)
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
        invImages[ItemActual].gameObject.SetActive(true); //esto es para que se active SOLO las imagenes de las armas

        Debug.Log("Cantidad armas: " + weapons.Count);
        Debug.Log("ItemActual: " + ItemActual);

        WeaponController weaponController = newWeapon as WeaponController;
        if (weaponController != null && ItemActual < invImages.Count)
        {
            invImages[ItemActual].sprite = weaponController.WeaponData.WeaponSprite;
        }


        UpdateSelection();
    }
    public bool RemoveWeapon(IWeapon weaponToRemove)
    {
        if (weaponToRemove == null) return false;

        int index = -1;
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] == weaponToRemove) 
            { 
                index = i; 
                break; 
            }
        }
        if (index < 0) return false;

        Transform weaponTransform = weaponToRemove.getTransform();
        weapons.RemoveAt(index);

        // Reacomodamos el slot seleccionado
        if (weapons.Count == 0) ItemActual = 0;
        else if (index < ItemActual) ItemActual--;
        else if (ItemActual >= weapons.Count) ItemActual = weapons.Count - 1;

        if (weaponTransform != null) Destroy(weaponTransform.gameObject);

        RefreshSlotSprites();
        UpdateSelection();
        return true;
    }
    private void RefreshSlotSprites()
    {
        WeaponController wc;
        for (int i = 0; i < invImages.Count; i++)
        {
            if (i < weapons.Count && i > 0) //para que no tire null
            {
                wc = weapons[i] as WeaponController; //el AS es necesario porque el wachin dice que no se puede expresar porque weapons[i] es IWeapon no WeaponController
            }
            else
            {
                wc = null; //esto me lo dijo claude y no se por que, pero dejenlo ahi, aura
            }

            if(wc != null)
            {
                invImages[i].sprite = wc.WeaponData.WeaponSprite; //al sprite en el que estamos le asignamos el sprite del arma
            }
            else
            {
                invImages[i] = emptySlotSprites[i];//sino, ese sprite tiene que ser una imagen blanca
            }
        }
    }
    void UpdateSelection()
    {
        for (int i = 0; i < invImages.Count; i++)
        {
            if (i == ItemActual)
            {
                invImages[i].color = Color.red;
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