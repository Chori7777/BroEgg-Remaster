using UnityEngine;
using static WeaponData;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] GameObject hand;
    public GameObject Hand => hand;
    public FactoryWeapon factoryWeapon;
    public InventaryWeapons inventoryManager;

    void Start()
    {
        GiveWeapon("Pistol");
        GiveWeapon("AutoRifle");
        GiveWeapon("Shotgun");
    }   

    public WeaponController GiveWeapon(string weaponId)
    {
        WeaponController newWeapon = factoryWeapon.CreateWeapon(weaponId, hand.transform.position);

        Transform weaponTransform = newWeapon.getTransform();
        weaponTransform.SetParent(hand.transform);
        weaponTransform.localPosition = Vector3.zero;
        weaponTransform.localRotation = Quaternion.identity;

        inventoryManager.AddWeapon(newWeapon);
        return newWeapon;
    }
    public bool RemoveWeapon(WeaponController weapon)
    {
       return inventoryManager.RemoveWeapon(weapon);
    }
    void Update()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.ControlsBlocked) return;
        IWeapon currentWeapon = inventoryManager.CurrentWeapon();
        if (currentWeapon == null) Debug.Log("No tengo arma");

        
        bool isTriggering = false;

        if (currentWeapon != null)
        {
            switch(currentWeapon.GetShootingType())
            {
                case ShootingType.Automatic:

                    // Automática
                    isTriggering = Input.GetMouseButton(0);
                    break;

                case ShootingType.Single:

                    // Semiautomática
                    isTriggering = Input.GetMouseButtonDown(0);
                    break;

                case ShootingType.Shotgun:

                    // Escopeta
                    isTriggering = Input.GetMouseButtonDown(0);
                    Debug.Log("shotgun mode");
                    break;
            }
        }
            

        if (isTriggering)
        {
            currentWeapon.shoot();
        }
    }
    // Chequeamos si esta el arma, el inventoryManager se encarga de eso buscando el id q le damos desde el ButtonLogic
    public bool HasWeapon(string weaponId)
    {
        return inventoryManager.HasWeapon(weaponId);
    }
}