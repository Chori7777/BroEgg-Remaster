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
        GiveWeapon("SemiAuRifle");
    }

    public void GiveWeapon(string weaponId)
    {
        WeaponController newWeapon = factoryWeapon.CreateWeapon(weaponId, hand.transform.position);

        Transform weaponTransform = newWeapon.getTransform();
        weaponTransform.SetParent(hand.transform);
        weaponTransform.localPosition = Vector3.zero;
        weaponTransform.localRotation = Quaternion.identity;

        inventoryManager.AddWeapon(newWeapon);
    }

    void Update()
    {
        IWeapon currentWeapon = inventoryManager.CurrentWeapon();
        if (currentWeapon == null) Debug.Log("No tengo arma");

        
        bool isTriggering = false;

        if (currentWeapon.GetShootingType() == ShootingType.Automatic)
        {
            // Automática
            isTriggering = Input.GetMouseButton(0);
        }
        else if(currentWeapon.GetShootingType() == ShootingType.Single)
        {
            // Semiautomática
            isTriggering = Input.GetMouseButtonDown(0);
        }
        else
        {
            // Escopeta
            isTriggering = Input.GetMouseButtonDown(0);
        }

        if (isTriggering)
        {
            currentWeapon.shoot();
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        IWeapon weaponComponent = other.GetComponent<IWeapon>();
        if (weaponComponent != null)
        {
            Debug.Log("Arma recogida");
            Transform weaponTransform = weaponComponent.getTransform();
            weaponTransform.SetParent(hand.transform);
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;

            inventoryManager.AddWeapon(weaponComponent);
        }
    }
}