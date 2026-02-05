using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> EnemyPrefabs;
    public Transform[] SpawnPoints;
    
    private float _spawnOffset = 1f;

    public void SpawnEnemy(int number)
    {
        if (EnemyPrefabs.Count == 0 || SpawnPoints.Length == 0) return;

        var enemyPrefab = EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)];

        for (int i = 0; i < number; i++)
        {
            var spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
            var position = spawnPoint.position + new Vector3(Random.Range(-_spawnOffset, _spawnOffset), 0, Random.Range(-_spawnOffset, _spawnOffset));
            var rotation = spawnPoint.rotation;
            StartCoroutine(DelaySpawnEnemy(i * 0.5f, enemyPrefab, position, rotation));
        }
    }

    private IEnumerator DelaySpawnEnemy(float delay, GameObject enemyPrefab, Vector3 position, Quaternion rotation)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(enemyPrefab, position, rotation);
    }
}
