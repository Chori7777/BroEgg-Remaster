using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FactoryEnemy : MonoBehaviour
{
    public List<Enemy> enemyList = new List<Enemy>();
    public SimpleArrayDictionary<string,Enemy> enemyDictionary= new SimpleArrayDictionary<string, Enemy>();
    [SerializeField] EnemyBulletPool pool;
    void Start()
    {
        for(int i =0; i< enemyList.Count; i++)
        {
            enemyDictionary.Add(enemyList[i].id, enemyList[i]);

        }
    }
    // Bueno, por lo que fui entendiendo, lo unico que le sume a Create Enemy fue el Transform spawnPoint!
   
    public Enemy CreateEnemy(string enemyType, Vector3 spawnPoint)
    {

        if (enemyDictionary.ContainsKey(enemyType))
        {
            Enemy spawnedEnemy = Instantiate(enemyDictionary[enemyType], spawnPoint, Quaternion.identity);

            TwoHeadedEnemy twoHeaded = spawnedEnemy as TwoHeadedEnemy;
            if (twoHeaded != null) twoHeaded.SetPool(pool);

            return spawnedEnemy;
        }
        else
        {
            Debug.Log("no encontre enemigo we");
            return null;
        }

    }
}
