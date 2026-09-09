using System.Collections.Generic;
using UnityEngine;

public class FactoryWeapon : MonoBehaviour
{
    public List<WeaponController> weaponList = new List<WeaponController>();
    public Dictionary<string, WeaponController> weaponDictionary = new Dictionary<string, WeaponController>();

    [SerializeField] private BulletPool bulletPool;

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
            weapon.SetBulletPool(bulletPool);
            return weapon;
        }
        else
        {
            Debug.LogWarning($"No se encontro un arma con id '{weaponId}' en el diccionario.");
            return null;

        }

    }
}