using UnityEngine;
namespace ED262C
{
    public class EnemySpawnerManager : MonoBehaviour
    {
        private SimpleArrayList<Transform> spawnPoints = new SimpleArrayList<Transform>();


        FactoryEnemy factoryEnemy;
        void Start()
        {
            factoryEnemy=GetComponent<FactoryEnemy>();
            GameObject[] points = GameObject.FindGameObjectsWithTag("spawnPoint");

            Debug.Log("SpawnPoints encontrados: " + points.Length);

            for (int i=0; i <points.Length;i++)
            {
                spawnPoints.Add(points[i].transform);
            }
        }

        // Update is called once per frame
        void Update()
        {
           if(Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log(spawnPoints.Count);
                spawnWave();
            }
        }

        void spawnWave()
        {

            for (int i = 0; i < spawnPoints.Count; i++)
            {
                //Aca lo que hice fue primeor q nada, q saque un randomSpawnPoint de la lista de spawnPoints y cree un enemigo
                int randomSpawnPoint = Random.Range(0, spawnPoints.Count);
                // aca se hace el traspase de Int a Transform para poder darselo al Factory
                Vector3 spawnpoint = spawnPoints[randomSpawnPoint].position;
                // Y aca se crea, recorre un for y deberia funcionar!
                Enemy enemy = factoryEnemy.CreateEnemy(factoryEnemy.enemyList[Random.Range(0, factoryEnemy.enemyList.Count)].id, spawnpoint);
            }
           

        }
    }
}
