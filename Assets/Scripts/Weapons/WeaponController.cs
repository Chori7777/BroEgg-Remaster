using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using static WeaponData;
[RequireComponent(typeof(WeaponFeedbackFacade))]
public class WeaponController : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponData weaponData;
    private BulletPool bulletPool;

    public event Action<int, int> OnAmmoChanged;

    private WeaponFeedbackFacade feedback;
    public WeaponData WeaponData => weaponData;

    private IWeaponBehavior behavior;
    private PlayerStats stats;
    private Collider2D playerCollider;

    private float nextFireTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;

    private const float PassiveMultiplier = 0.5f; //las armas pasivas usan la mitad de todo

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
        feedback = GetComponent<WeaponFeedbackFacade>();

    }

    void Start()
    {
        stats = GetComponentInParent<PlayerStats>();
        TryInitStats();

        currentAmmo = weaponData.MagazineSize;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) StartCoroutine(Reload());
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

        if (currentAmmo <= 0 || isReloading)
        {
            StartCoroutine(Reload());
            isReloading = false;
            return;
        }

        nextFireTime = Time.time + weaponData.FireRate;
        currentAmmo--;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX(SoundID.Shoot);
        int damage = DamageCalculator.CalculateDamage(stats, weaponData.BaseDamage);
        behavior.Shoot(transform, damage, playerCollider, bulletPool);
        if (feedback != null) feedback.OnShoot();

        //EVENTO PARA ACTUALIZAR EL HUD DE LAS BALAS
        OnAmmoChanged?.Invoke(currentAmmo, weaponData.MagazineSize);
    }

    public bool CanShoot => !isReloading && Time.time >= nextFireTime;

    public void PassiveShoot(Vector2 targetPosition, Vector2 targetVelocity)
    {
        if (LevelManager.Instance != null && LevelManager.Instance.ControlsBlocked) return;
        if (!CanShoot) return;
        TryInitStats();
        if (stats == null) return;

        //cargador a la mitad: cuando disparo esa cantidad de balas, recarga
        int passiveMagazine = Mathf.Max(1, Mathf.RoundToInt(weaponData.MagazineSize * PassiveMultiplier));
        int shotsFired = weaponData.MagazineSize - currentAmmo;

        if (shotsFired >= passiveMagazine)
        {
            StartCoroutine(Reload());
            return;
        }

        //cadencia a la mitad: tarda el doble entre disparo y disparo
        nextFireTime = Time.time + weaponData.FireRate / PassiveMultiplier;
        currentAmmo--;

        //daño a la mitad (minimo 1)
        int damage = Mathf.Max(1, Mathf.RoundToInt(DamageCalculator.CalculateDamage(stats, weaponData.BaseDamage) * PassiveMultiplier));
        behavior.ShootAtTarget(transform, targetPosition, targetVelocity, damage, playerCollider, bulletPool);
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
    public WeaponController GetWeaponController()
    {
        return this;
    }
}