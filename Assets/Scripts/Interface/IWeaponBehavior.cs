using UnityEngine;

public interface IWeaponBehavior
{
    // Disparo manual: apunta al mouse
    void Shoot(Transform weaponTransform, int damage, Collider2D shooterCollider, BulletPool bulletPool);

    // Disparo automatico: apunta a un objetivo prediciendo hacia donde se va a mover
    void ShootAtTarget(Transform weaponTransform, Vector2 targetPosition, Vector2 targetVelocity, int damage, Collider2D shooterCollider, BulletPool bulletPool);
}