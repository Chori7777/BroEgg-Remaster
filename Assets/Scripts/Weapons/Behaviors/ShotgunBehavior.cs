using Unity.VisualScripting;
using UnityEngine;

public class ShotgunBehavior : IWeaponBehavior
{
    private int pelletsCount = 6;
    private float spreadAngle = 20; 

    public void Shoot(Transform weaponTransform, int damage, Collider2D shooterCollider, BulletPool bulletPool)
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
            bullet.Setup(pelletDirection, damagePerPellet, shooterCollider);
        }
    }
}
