using UnityEngine;
using static WeaponData;

public interface IWeapon 
{

    void shoot();

    Transform getTransform();

    public ShootingType GetShootingType();
}
