using Unity.VisualScripting;
using UnityEngine;

public class ShotgunBehavior : IWeaponBehavior
{
    private int pelletsCount = 6;
    private float spreadAngle = 20;

    public void Shoot(Transform weaponTransform, int damage, Collider2D shooterCollider, BulletPool bulletPool, BulletType bulletType)
    {
        if (bulletPool == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = weaponTransform.position.z;
        Vector2 baseDirection = (mousePosition - weaponTransform.position).normalized;
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

        int damagePerPellet = 2;

        for (int i = 0; i < pelletsCount; i++)
        {
            Bullet bullet = bulletPool.Get();
            if (bullet == null) continue;

            bullet.pool = bulletPool;
            bullet.transform.position = weaponTransform.position;

            float randomOffset = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
            float finalAngle = baseAngle + randomOffset;   // sin modificar baseAngle

            Vector2 pelletDirection = new Vector2(
                Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                Mathf.Sin(finalAngle * Mathf.Deg2Rad));

            bullet.transform.rotation = Quaternion.Euler(0, 0, finalAngle);
            bullet.Setup(pelletDirection, damagePerPellet, shooterCollider, bulletType);
        }
    }

    public void ShootAtTarget(Transform weaponTransform, Vector2 targetPosition, Vector2 targetVelocity, int damage, Collider2D shooterCollider, BulletPool bulletPool, BulletType bulletType)
    {
        if (bulletPool == null) return;

        // El daño que llega es el total del disparo, se reparte entre los perdigones (minimo 1 cada uno)
        int damagePerPellet = Mathf.Max(1, damage / pelletsCount);

        for (int i = 0; i < pelletsCount; i++)
        {
            Bullet bullet = bulletPool.Get();
            if (bullet == null) continue;

            bullet.pool = bulletPool;
            bullet.transform.position = weaponTransform.position;

            // Cada perdigon apunta al punto de intercepcion y despues le sumamos la dispersion
            Vector2 aimPoint = AimPredictor.GetInterceptPoint(weaponTransform.position, targetPosition, targetVelocity, bullet.Speed);
            Vector2 baseDirection = (aimPoint - (Vector2)weaponTransform.position).normalized;
            float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

            float randomOffset = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
            float finalAngle = baseAngle + randomOffset;

            Vector2 pelletDirection = new Vector2(
                Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                Mathf.Sin(finalAngle * Mathf.Deg2Rad));

            bullet.transform.rotation = Quaternion.Euler(0, 0, finalAngle);
            bullet.Setup(pelletDirection, damagePerPellet, shooterCollider, bulletType);
        }
    }
}