using UnityEngine;

public class SingleShootBehavior : IWeaponBehavior
{
    public void Shoot(Transform weaponTransform, int damage, Collider2D shooterCollider, BulletPool bulletPool)
    {
        // Pedimos la bala al pool
        Bullet bullet = bulletPool.Get();

        // Le asignamos la referencia del pool a la bala
        bullet.pool = bulletPool;

        // Posicionamos la bala en el arma
        bullet.transform.position = weaponTransform.position;
        bullet.transform.rotation = weaponTransform.rotation;

        // Calculamos la dirrecion del mouse desde el arma
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = weaponTransform.position.z; 
        Vector2 direction = (mousePosition - weaponTransform.position).normalized;

        // Mandamos la direccion y el daño a la bala
        bullet.Setup(direction, damage, shooterCollider);
    }
}
