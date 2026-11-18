using System.Collections.Generic;
using UnityEngine;

public class FactoryWeapon : MonoBehaviour
{
    public List<WeaponController> weaponList = new List<WeaponController>();
    public Dictionary<string, WeaponController> weaponDictionary = new Dictionary<string, WeaponController>();

    [Header("Pools de balas (uno por tipo)")]
    [SerializeField] private BulletPool bulletPool;           // Normal (el que ya tenias)
    [SerializeField] private BulletPool heavyBulletPool;      // Heavy
    [SerializeField] private BulletPool piercingBulletPool;   // Piercing
    [SerializeField] private BulletPool knockbackBulletPool;  // Knockback

    private void Awake()
    {
        for (int i = 0; i < weaponList.Count; i++)
            weaponDictionary.Add(weaponList[i].WeaponData.IdWeapon, weaponList[i]);
    }

    public WeaponController CreateWeapon(string weaponId, Vector3 spawnPoint)
    {
        if (weaponDictionary.ContainsKey(weaponId))
        {
            WeaponController weapon = Instantiate(weaponDictionary[weaponId], spawnPoint, Quaternion.identity);

            //cada arma recibe el pool del tipo de bala que dispara (WeaponData.BulletType)
            weapon.SetBulletPool(GetPool(weapon.WeaponData.BulletType));
            return weapon;
        }
        else
        {
            Debug.LogWarning($"No se encontro un arma con id '{weaponId}' en el diccionario.");
            return null;
        }
    }

    private BulletPool GetPool(BulletType type)
    {
        BulletPool pool = null;

        switch (type)
        {
            case BulletType.Heavy: pool = heavyBulletPool; break;
            case BulletType.Piercing: pool = piercingBulletPool; break;
            case BulletType.Knockback: pool = knockbackBulletPool; break;
        }

        //si todavia no hay un pool asignado para ese tipo, se usa la bala normal (asi nada se rompe mientras configuras)
        if (pool == null) pool = bulletPool;
        return pool;
    }
}
