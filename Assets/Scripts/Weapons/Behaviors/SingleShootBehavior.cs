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

    public void ShootAtTarget(Transform weaponTransform, Vector2 targetPosition, Vector2 targetVelocity, int damage, Collider2D shooterCollider, BulletPool bulletPool)
    {
        if (bulletPool == null) return;

        // Pedimos la bala al pool
        Bullet bullet = bulletPool.Get();
        if (bullet == null) return;

        bullet.pool = bulletPool;

        // Posicionamos la bala en el arma
        bullet.transform.position = weaponTransform.position;

        // Apuntamos a donde va a estar el enemigo cuando llegue la bala, no a donde esta ahora
        Vector2 aimPoint = AimPredictor.GetInterceptPoint(weaponTransform.position, targetPosition, targetVelocity, bullet.Speed);
        Vector2 direction = (aimPoint - (Vector2)weaponTransform.position).normalized;

        // La bala mira hacia donde va
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Mandamos la direccion y el daño a la bala
        bullet.Setup(direction, damage, shooterCollider);
    }
}