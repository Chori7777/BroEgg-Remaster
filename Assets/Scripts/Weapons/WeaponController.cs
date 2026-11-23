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

    [Header("Giro de recarga")]
    [Tooltip("Velocidad del giro en grados por segundo para que todas las armas giren lo mismo")]
    [SerializeField] private float degreesPerSecond = 720f;

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

    // La tecla R ya no se escucha aca: ahora la maneja PlayerWeaponManager y llama a StartReload()
    // del arma de la mano, para que las armas pasivas no recarguen (ni giren) cuando apretas R

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

        // Sin balas: arranca la recarga (con giro) y listo.
        // Ya no hace falta preguntar isReloading aca porque arriba ya cortamos si estaba recargando
        if (currentAmmo <= 0)
        {
            StartReload();
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
            StartCoroutine(Reload(false)); //las pasivas recargan SIN girar (PassiveWeaponFire controla su rotacion)
            return;
        }

        //cadencia a la mitad: tarda el doble entre disparo y disparo
        nextFireTime = Time.time + weaponData.FireRate / PassiveMultiplier;
        currentAmmo--;

        //daño a la mitad (minimo 1)
        int damage = Mathf.Max(1, Mathf.RoundToInt(DamageCalculator.CalculateDamage(stats, weaponData.BaseDamage) * PassiveMultiplier));
        behavior.ShootAtTarget(transform, targetPosition, targetVelocity, damage, playerCollider, bulletPool);
    }

    // Recarga del arma de la mano (la llaman shoot() sin balas y PlayerWeaponManager con la tecla R)
    public void StartReload()
    {
        if (isReloading || currentAmmo >= weaponData.MagazineSize) return;
        StartCoroutine(Reload(true));
    }

    private IEnumerator Reload(bool spin) //Funcion para recargar las armas. Si spin es true, el arma gira mientras recarga
    {
        isReloading = true;

        float duration = weaponData.ReloadTime;
        float elapsed = 0f;

        // vueltas completas segun la duracion (minimo 1): asi siempre termina derecha justo cuando termina la recarga
        float turns = Mathf.Max(1f, Mathf.Round(duration * degreesPerSecond / 360f));

        // Si el padre esta espejado (WeaponAim da vuelta la escala al apuntar a la izquierda),
        // invertimos el giro para que en pantalla siempre gire para el mismo lado
        float direction = (transform.parent != null && transform.parent.lossyScale.y < 0f) ? -1f : 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (spin)
            {
                float progress = Mathf.Clamp01(elapsed / duration);
                transform.localRotation = Quaternion.Euler(0f, 0f, 360f * turns * progress * direction);
            }

            yield return null;
        }

        if (spin) transform.localRotation = Quaternion.identity; // vuelve a su posicion normal

        currentAmmo = weaponData.MagazineSize;
        isReloading = false;

        // para que el HUD muestre el cargador lleno apenas termina de recargar
        OnAmmoChanged?.Invoke(currentAmmo, weaponData.MagazineSize);
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