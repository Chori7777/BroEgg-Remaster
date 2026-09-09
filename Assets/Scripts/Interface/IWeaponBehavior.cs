using UnityEngine;

public interface IWeaponBehavior 
{
    void Shoot(Transform weaponTransform, int damage, Collider2D shooterCollider, BulletPool bulletPool);
    
}
