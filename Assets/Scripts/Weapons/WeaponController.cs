using System.Collections;
using UnityEngine;

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

    void Start()
    {
        behavior = new SingleShootBehavior();

        stats = GetComponentInParent<PlayerStats>();
        playerCollider = stats.GetComponent<Collider2D>();

        currentAmmo = weaponData.MagazineSize;
    }

    public void shoot()
    {
        if (isReloading) return;
        if (Time.time < nextFireTime) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        nextFireTime = Time.time + weaponData.FireRate;
        currentAmmo--;

        int damage = DamageCalculator.CalculateDamage(stats, DamageCalculator.DamageType.Normal, weaponData.BaseDamage);
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