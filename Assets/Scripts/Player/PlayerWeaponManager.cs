using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    IWeapon weapon;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetMouseButtonDown(0))
        {
            if(weapon!= null)
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
        IWeapon weaponComponent= other.GetComponent<IWeapon>();

        if (weaponComponent != null)
        {
            SetWeapon(weaponComponent);
            Debug.Log("Arma recogida");

        }

    }
}
