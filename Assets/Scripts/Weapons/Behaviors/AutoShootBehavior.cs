using UnityEngine;

public class AutoShoot : IWeaponBehavior
{
    public void Shoot(Transform weaponTransform, int damage, Collider2D shooterCollider, BulletPool bulletPool)
    {
        if (bulletPool == null) return;

        // Pedimos la bala al pool
        Bullet bullet = bulletPool.Get();
        if (bullet == null) return;

        bullet.pool = bulletPool;

        // Posicionamos la bala en el arma
        bullet.transform.position = weaponTransform.position;
        bullet.transform.rotation = weaponTransform.rotation;

        // Calculamos la dirección del mouse desde el arma papu
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = weaponTransform.position.z;
        Vector2 direction = (mousePosition - weaponTransform.position).normalized;

        // Mandamos la dirección y el daño a la bala
        bullet.Setup(direction, damage, shooterCollider);
    }
}
