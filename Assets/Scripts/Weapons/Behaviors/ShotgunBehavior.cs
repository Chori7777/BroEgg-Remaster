using UnityEngine;

public class ShotgunBehavior : IWeaponBehavior
{
    private int pelletsCount = 6;
    private float spreadAngle = 20;


    public void Shoot(Transform weaponTransform, int damage, Collider2D shooterCollider, BulletPool bulletPool)
    {
        if (bulletPool == null) return;

        // 1. Calculamos la dirección base hacia el puntero del mouse
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = weaponTransform.position.z;
        Vector2 baseDirection = (mousePosition - weaponTransform.position).normalized;

        // 2. Calculamos el ángulo base en grados
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

        // Divide el daño total del arma entre la cantidad de perdigones
        int damagePerPellet = Mathf.Max(1, damage / pelletsCount);

        // 3. Generamos los perdigones papulince
        for (int i = 0; i < pelletsCount; i++)
        {
            Bullet bullet = bulletPool.Get();
            if (bullet == null) continue;

            bullet.pool = bulletPool;
            bullet.transform.position = weaponTransform.position;

            // Variación aleatoria de ángulo dentro del cono de dispersión
            float randomOffset = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
            float finalAngle = baseAngle + randomOffset;

            // Convertimos el ángulo final a vector de dirección 2D
            Vector2 pelletDirection = new Vector2(
                Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                Mathf.Sin(finalAngle * Mathf.Deg2Rad)
            );

            bullet.transform.rotation = Quaternion.Euler(0, 0, finalAngle);

            // Configuramos la bala con su dirección y daño individual
            bullet.Setup(pelletDirection, damagePerPellet, shooterCollider);
        }
    }
}
