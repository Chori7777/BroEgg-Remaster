using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] GameObject hand;
    [SerializeField] FactoryWeapon factoryWeapon;
    [SerializeField] InventoryManager inventoryManager;

    void Start()
    {
        GiveWeapon("Pistol");
        GiveWeapon("SemiAuRifle");
        GiveWeapon("AutoRifle");
        GiveWeapon("Subfusil");
        GiveWeapon("Minigun");
        GiveWeapon("Shotgun");
    }

    void GiveWeapon(string weaponId)
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
        if (Input.GetMouseButtonDown(0))
        {
            IWeapon currentWeapon = inventoryManager.CurrentWeapon;
            if (currentWeapon != null)
            {
                currentWeapon.shoot();
            }
            else
            {
                Debug.Log("no tengo arma we");
            }
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