using System.Collections;
using UnityEngine;
using static WeaponData;

public class WeaponController : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponData weaponData;
    private BulletPool bulletPool;

    public WeaponData WeaponData => weaponData;

    private IWeaponBehavior behavior;
    private PlayerStats stats;
    private Collider2D playerCollider;

    private float nextFireTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;

    public ShootingType GetShootingType()
    {
        if (weaponData != null)
        {
            return weaponData.GetShootingType;
        }

        return ShootingType.Single; // Valor por defecto en caso de que weaponData sea null papu
    }

    void Start()
    {
        behavior = new SingleShootBehavior();

        stats = GetComponentInParent<PlayerStats>();
        TryInitStats();

        currentAmmo = weaponData.MagazineSize;
    }
    private void TryInitStats()
    {
        if (stats == null)
        {
            stats = GetComponentInParent<PlayerStats>();
            if (stats != null)
            {
                playerCollider = stats.GetComponent<Collider2D>();
            }
        }
    }
    public void shoot()
    {
        if (isReloading) return;
        if (Time.time < nextFireTime) return;
        TryInitStats();

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        nextFireTime = Time.time + weaponData.FireRate;
        currentAmmo--;

        int damage = DamageCalculator.CalculateDamage(stats, weaponData.BaseDamage);
        behavior.Shoot(transform, damage, playerCollider, bulletPool);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(weaponData.ReloadTime);
        currentAmmo = weaponData.MagazineSize;
        isReloading = false;
    }

    public Transform getTransform()
    {
        return transform;
    }

    public void SetBulletPool(BulletPool pool)
    {
        bulletPool = pool;
    }
}