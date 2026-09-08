using System.Collections.Generic;
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
               
            }
        }

        public void spawnWave(int count, List<EnemyProbability> probabilities)
        {
            Debug.Log("Spawner currentRound: " + LevelManager.Instance.CurrentRound);

            for (int i = 0; i < count; i++)
            {
                int randomSpawnPoint = Random.Range(0, spawnPoints.Count);
                Vector3 spawnpoint = spawnPoints[randomSpawnPoint].position;

                string chosenId = ChooseEnemyByProbability(probabilities);
                Enemy enemy = factoryEnemy.CreateEnemy(chosenId, spawnpoint);
                enemy.Initialize(LevelManager.Instance.CurrentRound);
            }
        }

        string ChooseEnemyByProbability(List<EnemyProbability> probabilities)
        {
            float totalWeight = 0f;
            for (int i = 0; i < probabilities.Count; i++)
            {
                totalWeight += probabilities[i].Probability;
            }

            float randomValue = Random.Range(0f, totalWeight);

            float accumulated = 0f;
            for (int i = 0; i < probabilities.Count; i++)
            {
                accumulated += probabilities[i].Probability;
                if (randomValue <= accumulated)
                {
                    return probabilities[i].EnemyId;
                }
            }

            // Por si algo falla (lista vacía, redondeos), devolvemos el primero como fallback
            return probabilities[0].EnemyId;
        }


    }
}
