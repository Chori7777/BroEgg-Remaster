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
    private IWeaponBehavior CreateBehavior(ShootingType type) //esto soluciona lo de la escopeta, antes estaba seteado por defecto el singleshot, LPM
    {
        switch (type) //no hace falta el break por el return
        {
            case ShootingType.Shotgun: return new ShotgunBehavior(); 
            case ShootingType.Automatic: return new AutoShoot();
            default: return new SingleShootBehavior();
        }
    }

    private void Awake()
    {
        behavior = CreateBehavior(GetShootingType());

    }

    void Start()
    {
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
        if (LevelManager.Instance != null && LevelManager.Instance.ControlsBlocked) return;
        if (isReloading) return;
        if (Time.time < nextFireTime) return; //cadencia de tiro
        TryInitStats(); //para saber las stats del player

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

    private IEnumerator Reload() //Funcion para recargar las armas
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