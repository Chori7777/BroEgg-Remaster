using System.Collections.Generic;
using UnityEngine;

public class FactoryEnemy : MonoBehaviour
{
    public List<Enemy> enemyList = new List<Enemy>();
    public SimpleArrayDictionary<string,Enemy> enemyDictionary= new SimpleArrayDictionary<string, Enemy>();
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

            return Instantiate(enemyDictionary[enemyType], spawnPoint, Quaternion.identity);
        }
        else
        {
            Debug.Log("no encontre enemigo we");
            return null;
        }

    }
}
