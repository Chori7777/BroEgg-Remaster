using UnityEngine;

public class WeaponController : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponData weaponData;
    private BulletPool bulletPool;

    public WeaponData WeaponData => weaponData;

    private IWeaponBehavior behavior;
    private PlayerStats stats;
    private Collider2D playerCollider;

    void Start()
    {
        //Aca se asignan los comportamientos por ahora solo hay uno
        behavior = new SingleShootBehavior();

        stats = GetComponentInParent<PlayerStats>();
        playerCollider = stats.GetComponent<Collider2D>();
    }

    public void shoot()
    {
        int damage = DamageCalculator.CalculateDamage(stats, DamageCalculator.DamageType.Normal, weaponData.BaseDamage);
        behavior.Shoot(transform, damage, playerCollider, bulletPool);
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

