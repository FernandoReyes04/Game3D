using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject giantEnemyPrefab;
    public GameObject oneEyeEnemyPrefab;

    [Header("Spawn Settings")]
    public List<Transform> spawnPoints = new List<Transform>();
    public float spawnDelay = 1f;

    private GameObject currentEnemy;
    private bool isGiantTurn = true;

    void Start()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No hay puntos de spawn definidos!");
        }

        SpawnEnemy();
    }

    void Update()
    {
        if (currentEnemy == null)
        {
            Invoke("SpawnEnemy", spawnDelay);
        }
    }

    void SpawnEnemy()
    {
        if (currentEnemy != null) return;

        Vector3 spawnPosition = GetRandomSpawnPosition();

        if (isGiantTurn)
        {
            currentEnemy = Instantiate(giantEnemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemy.name = "Enemy_Giant";
            Debug.Log("Spawned Enemy_Giant");
        }
        else
        {
            currentEnemy = Instantiate(oneEyeEnemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemy.name = "Enemy_OneEye";
            Debug.Log("Spawned Enemy_OneEye");
        }

        isGiantTurn = !isGiantTurn;

        EnemyHealth enemyHealth = currentEnemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.onDeath += OnEnemyDied;
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Count);
            return spawnPoints[randomIndex].position;
        }
        else
        {
            float randomX = Random.Range(-10f, 10f);
            float randomZ = Random.Range(-10f, 10f);
            return new Vector3(randomX, 1f, randomZ);
        }
    }

    void OnEnemyDied()
    {
        currentEnemy = null;
    }
}
