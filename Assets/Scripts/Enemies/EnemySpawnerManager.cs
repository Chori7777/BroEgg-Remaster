using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED262C
{
    public class EnemySpawnerManager : MonoBehaviour
    {
        // Estructura propia permitida
        private SimpleArrayList<Vector3> spawnPoints = new SimpleArrayList<Vector3>();

        [SerializeField] private float groupMinRadius = 6f;
        [SerializeField] private float groupMaxRadius = 10f;
        [SerializeField] private float groupSpread = 1.5f;
        [SerializeField] private int candidatePointCount = 10; // Cantidad de puntos candidatos a generar por tanda

        FactoryEnemy factoryEnemy;
        private Transform player;

        [SerializeField] private GameObject spawnWarningPrefab;
        [SerializeField] private float WarningTime = 1.0f;
        [SerializeField] private float MinSpawnDistance = 3.0f;

        void Start()
        {
            factoryEnemy = GetComponent<FactoryEnemy>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void Update()
        {
        }

        public void spawnWave(int count, List<EnemyProbability> probabilities)
        {
            // 1. Generamos los puntos candidatos en tiempo de ejecución usando solo SimpleArrayList
            GenerateDynamicSpawnPoints(count);

            for (int i = 0; i < count; i++)
            {
                // Obtenemos un punto válido usando únicamente la lista
                Vector3 basePoint = ChooseValidSpawn();

                // Aplicamos dispersión alrededor del punto elegido
                Vector2 offset = Random.insideUnitCircle * groupSpread;
                Vector3 spawnPoint = basePoint + (Vector3)offset;

                string chosenId = ChooseEnemyByProbability(probabilities);
                StartCoroutine(SpawnConAviso(chosenId, spawnPoint));
            }
        }

        // Llena el SimpleArrayList con posiciones alrededor del jugador
        void GenerateDynamicSpawnPoints(int count)
        {
            spawnPoints.Clear(); // Limpiamos la lista para la nueva tanda

            int totalPointsToGenerate = Mathf.Max(count, candidatePointCount);

            for (int i = 0; i < totalPointsToGenerate; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float distance = Random.Range(groupMinRadius, groupMaxRadius);

                Vector3 point = new Vector3(
                    player.position.x + Mathf.Cos(angle) * distance,
                    player.position.y + Mathf.Sin(angle) * distance,
                    0);

                spawnPoints.Add(point);
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

            return probabilities[0].EnemyId;
        }

        bool ICanSpawnHere(Vector3 positionSpawn)
        {
            float distance = Vector2.Distance(positionSpawn, player.position);
            return distance >= MinSpawnDistance;
        }

        // Selecciona un spawn válido usando únicamente bucles y búsquedas sobre SimpleArrayList
        Vector3 ChooseValidSpawn()
        {
            int attempts = 0;
            int maxAttempts = 10;

            while (attempts < maxAttempts && spawnPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                Vector3 candidate = spawnPoints[randomIndex];

                if (ICanSpawnHere(candidate))
                {
                    return candidate;
                }

                attempts++;
            }

            // Fallback si la lista tiene elementos pero ninguno pasó el filtro a la primera
            if (spawnPoints.Count > 0)
            {
                return spawnPoints[Random.Range(0, spawnPoints.Count)];
            }

            // Fallback de emergencia absoluto si la lista estuviera vacía
            return player.position + (Vector3)(Random.insideUnitCircle * groupMinRadius);
        }

        IEnumerator SpawnConAviso(string enemyId, Vector3 positionSpawn)
        {
            GameObject notice = Instantiate(spawnWarningPrefab, positionSpawn, Quaternion.identity);

            yield return new WaitForSeconds(WarningTime);

            Destroy(notice);

            Enemy enemy = factoryEnemy.CreateEnemy(enemyId, positionSpawn);
            Debug.Log("Enemigo creado: " + (enemy != null));

            enemy.Initialize(LevelManager.Instance.CurrentRound);
        }

        public void ClearEnemies()
        {
            Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i].gameObject);
            }
        }
    }
}