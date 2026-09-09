using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    IWeapon weapon;
    [SerializeField] GameObject hand;
    [SerializeField] FactoryWeapon factoryWeapon;

    void Start()
    {
        WeaponController defaultWeapon = factoryWeapon.CreateWeapon("Pistol", hand.transform.position);
        SetWeapon(defaultWeapon);

        Transform weaponTransform = defaultWeapon.getTransform();
        weaponTransform.SetParent(hand.transform);
        weaponTransform.localPosition = Vector3.zero;
        weaponTransform.localRotation = Quaternion.identity;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (weapon != null)
            {
                weapon.shoot();
            }
            else
            {
                Debug.Log("no tengo arma we");
            }
        }
    }

    public void SetWeapon(IWeapon weapon)
    {
        this.weapon = weapon;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IWeapon weaponComponent = other.GetComponent<IWeapon>();
        if (weaponComponent != null)
        {
            SetWeapon(weaponComponent);
            Debug.Log("Arma recogida");
            Transform weaponTransform = weaponComponent.getTransform();
            weaponTransform.SetParent(hand.transform);
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;
        }
    }
}