using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [SerializeField] private WeaponTier tier;

    [SerializeField] private string idWeapon;

    [SerializeField] private int baseDamage;
    [SerializeField] private int magazineSize;

    [SerializeField] private float fireRate;
    [SerializeField] private float reloadTime;

    [SerializeField] private Sprite weaponSprite;

    public WeaponTier Tier => tier;
    public string IdWeapon => idWeapon;
    public int BaseDamage => baseDamage;

    public int MagazineSize => magazineSize; 
    
    public float FireRate => fireRate; 

    public float ReloadTime => reloadTime;

    public Sprite WeaponSprite => weaponSprite;



}
