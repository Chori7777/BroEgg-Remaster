using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ED262C
{
    public class EnemySpawnerManager : MonoBehaviour
    {
        private SimpleArrayList<Transform> spawnPoints = new SimpleArrayList<Transform>();

       

        FactoryEnemy factoryEnemy;
        private Transform player;

        [SerializeField] private GameObject spawnWarningPrefab;
        [SerializeField] private float WarningTime = 1.0f; // Cuando dura el aviso antes de que aparezca el enemigo fisico
        [SerializeField] private float MinSpawnDistance = 3.0f; // El radio alrededor

       

        void Start()
        {
           
             
            factoryEnemy = GetComponent<FactoryEnemy>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
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
        }

        public void spawnWave(int count, List<EnemyProbability> probabilities)
        {
            //Debug.Log("Spawner currentRound: " + LevelManager.Instance.CurrentRound);

            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPoint = ChooseValidSpawn();

                string chosenId = ChooseEnemyByProbability(probabilities);

                StartCoroutine(SpawnConAviso(chosenId, spawnPoint)); // Como es una corrutina no se llama como una funcion normal necesita que comience la corrutina 
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

        bool ICanSpawnHere(Vector3 positionSpawn)
        {
            float distance = Vector2.Distance(positionSpawn, player.position); 
            return distance >= MinSpawnDistance; // Si el jugador esta a 5 unidades es mayor osea que es true osea puedo spawnear aqui
        }

        Vector3 ChooseValidSpawn()
        {
            int attempts = 0;
            int maxAttempts = 10;

            while(attempts < maxAttempts)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count); //Elijo un spawnPoint random
                Vector3 candidate = spawnPoints[randomIndex].position; // Guardo esa posicion como posible candidato a spawnear

                if(ICanSpawnHere(candidate))
                    return candidate;

                attempts++; 
            }

            // Si después de 10 intentos no encontró nada, devolvemos cualquiera igual
            // Es un caso raro peropor si pasa para que detener el while y no explote todo
            int fallback = Random.Range(0, spawnPoints.Count);
            return spawnPoints[fallback].position;
        }


        IEnumerator SpawnConAviso(string enemyId, Vector3 positionSpawn) // Usa una corrutina para pausar en el medio de la funcion y despues de que pase cierto tiempo sigue con la funcion 
        {
            GameObject notice = Instantiate(spawnWarningPrefab, positionSpawn, Quaternion.identity); // Instancio el aviso 

            yield return new WaitForSeconds(WarningTime); // Cuando pasa 1s sigue la funcion, destruye el aviso y ahi si spawnea el enemigo 

            Destroy(notice);

            Enemy enemy = factoryEnemy.CreateEnemy(enemyId, positionSpawn);
            enemy.Initialize(LevelManager.Instance.CurrentRound);
        }
    }
}
