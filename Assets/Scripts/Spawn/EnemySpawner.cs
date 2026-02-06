using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public PrefabDictionary EnemyPrefabs;
    public Transform[] SpawnPoints;
    public List<GameObject> ActiveEnemies = new();

    private float _spawnOffset = 1f;

    public void SpawnEnemy(int number, string enemyKey)
    {
        if (SpawnPoints.Length == 0 || EnemyPrefabs == null || EnemyPrefabs.entries.Count == 0) return;

        GameObject enemyPrefab = EnemyPrefabs.GetValue(enemyKey);
        if (enemyPrefab == null)
        {
            enemyPrefab = EnemyPrefabs.entries[0].value;
        }

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
        var enemy = Instantiate(enemyPrefab, position, rotation);
        ActiveEnemies.Add(enemy);
    }
}
