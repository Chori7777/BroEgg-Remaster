using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ED262C
{
    public class EnemySpawnerManager : MonoBehaviour
    {
        // Lo cambie a Vector3 por que vamos a usar los bordes de la camara para spawnear enemigos, como se hace en Vampire Survivors
        private SimpleArrayList<Vector3> spawnPoints = new SimpleArrayList<Vector3>();


       
        [SerializeField] private float groupMinRadius = 6f;    // distancia minima del centro del grupo al jugador
        [SerializeField] private float groupMaxRadius = 10f;   // distancia maxima del centro del grupo al jugador
        [SerializeField] private float groupSpread = 1.5f;     // que tan dispersos quedan los enemigos dentro del grupo

        FactoryEnemy factoryEnemy;
        private Transform player;

        [SerializeField] private GameObject spawnWarningPrefab;
        [SerializeField] private float WarningTime = 1.0f; // Cuando dura el aviso antes de que aparezca el enemigo fisico
        [SerializeField] private float MinSpawnDistance = 3.0f; // El radio alrededor

   

        void Start()
        {
            // Como ya no vamos a estar usando spawn points fijos
            // se elimino que en el start se guarde en la lista los que estaban en la escena, ahora se generan en tiempo de ejecucion
            factoryEnemy = GetComponent<FactoryEnemy>();
            player = GameObject.FindGameObjectWithTag("Player").transform;

           
        }
        
        // Update is called once per frame
        void Update()
        {
        }

        public void spawnWave(int count, List<EnemyProbability> probabilities)
        {
            Vector3 center = ChooseGroupCenter();

            for (int i = 0; i < count; i++)
            {
                Vector2 offset = Random.insideUnitCircle * groupSpread;
                Vector3 spawnPoint = center + (Vector3)offset;

                string chosenId = ChooseEnemyByProbability(probabilities);
                StartCoroutine(SpawnConAviso(chosenId, spawnPoint));
            }
        }

        Vector3 ChooseGroupCenter()
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(groupMinRadius, groupMaxRadius);

            return new Vector3(
                player.position.x + Mathf.Cos(angle) * distance,
                player.position.y + Mathf.Sin(angle) * distance,
                0);
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

        Vector3 ChooseValidSpawn(SimpleArraySet<int> pointsUsed)
        {
            int attempts = 0;
            int maxAttempts = 10;

            while(attempts < maxAttempts)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count); //Elijo un spawnPoint random
                Vector3 candidate = spawnPoints[randomIndex]; // Guardo esa posicion como posible candidato a spawnear

                if (ICanSpawnHere(candidate) && !pointsUsed.Contains(randomIndex)) // Chequea si ese indice no se a usado ya en esa tanda 
                {
                    pointsUsed.Add(randomIndex); // Guarda en el set para que el proximo enemigo de la tanda no vuelva a elegir ese mismo punto
                    return candidate;
                }
                   

                attempts++; 
            }

            // Si después de 10 intentos no encontró nada, devolvemos cualquiera igual
            // Es un caso raro peropor si pasa para que detener el while y no explote todo
            int fallback = Random.Range(0, spawnPoints.Count);
           return spawnPoints[fallback];
        }


        IEnumerator SpawnConAviso(string enemyId, Vector3 positionSpawn) // Usa una corrutina para pausar en el medio de la funcion y despues de que pase cierto tiempo sigue con la funcion 
        {
            GameObject notice = Instantiate(spawnWarningPrefab, positionSpawn, Quaternion.identity); // Instancio el aviso 

            yield return new WaitForSeconds(WarningTime); // Cuando pasa 1s sigue la funcion, destruye el aviso y ahi si spawnea el enemigo 

            Destroy(notice);

            Enemy enemy = factoryEnemy.CreateEnemy(enemyId, positionSpawn);
            Debug.Log("Enemigo creado: " + (enemy != null));

            enemy.Initialize(LevelManager.Instance.CurrentRound);
        }
    }
}
