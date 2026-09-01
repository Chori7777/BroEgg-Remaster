using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    IWeapon weapon;
    [SerializeField] Bullet bullet;
    [SerializeField] GameObject hand;//empty que sostiene el arma
    //Bullet bulletScript;


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
            Transform weaponTransform = weaponComponent.getTransform();

            // Se creo un metodo de getTransform en IWeapon para obtener la transform del arma y poder setearla como hija de la mano del jugador
            weaponTransform.SetParent(hand.transform);

            //lo coloco en donde esta la manito del jugadorsillo y ahi queda
            weaponTransform.localPosition = Vector3.zero;
            weaponTransform.localRotation = Quaternion.identity;
        }

    }
}
