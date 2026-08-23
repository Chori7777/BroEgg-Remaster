using System.Collections.Generic;
using UnityEngine;

public class FactoryEnemy : MonoBehaviour
{
    public List<Enemy> enemyList = new List<Enemy>();
    public Dictionary<string,Enemy> enemyDictionary= new Dictionary<string, Enemy>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        Debug.Log("Enemigo:" + enemyType);
        Debug.Log("esta en el diccionario?" + enemyDictionary.ContainsKey(enemyType));

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
