using UnityEngine;
using static WeaponData;

public interface IWeapon 
{

    void shoot();

    public WeaponController GetWeaponController();
    Transform getTransform();

    public ShootingType GetShootingType();
}
