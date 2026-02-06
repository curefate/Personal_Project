using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public float countDownTimer;
    public bool ifCountdownActive;
    public int readonly_level;

    private EnemySpawner _enemySpawner;

    public void StartLevel()
    {
        StopAllCoroutines();
        StartCoroutine(Level_1());
    }

    IEnumerator Level_1()
    {
        readonly_level = 1;
        yield return new WaitForSeconds(3f);
        _enemySpawner.SpawnEnemy(8, "skeleton");
        yield return new WaitForSeconds(6f);
        _enemySpawner.SpawnEnemy(3, "skeleton");
        yield return new WaitForSeconds(6f);
        _enemySpawner.SpawnEnemy(3, "skeleton");
        while (_enemySpawner.ActiveEnemies.Count > 0)
        {
            yield return null;
        }
        StartCoroutine(Level_2());
    }

    IEnumerator Level_2()
    {
        readonly_level = 2;
        _enemySpawner.SpawnEnemy(10, "skeleton");
        yield return new WaitForSeconds(10f);
        _enemySpawner.SpawnEnemy(10, "skeleton");
        while (_enemySpawner.ActiveEnemies.Count > 0)
        {
            yield return null;
        }
        StartCoroutine(Level_3());
    }

    IEnumerator Level_3()
    {
        readonly_level = 3;
        _enemySpawner.SpawnEnemy(15, "skeleton");
        yield return new WaitForSeconds(5f);
        while (_enemySpawner.ActiveEnemies.Count > 7)
        {
            yield return null;
        }
        _enemySpawner.SpawnEnemy(10, "skeleton");
        while (_enemySpawner.ActiveEnemies.Count > 0)
        {
            yield return null;
        }
        StartCoroutine(Level_4());
    }

    IEnumerator Level_4()
    {
        readonly_level = 4;
        Debug.Log("All levels completed!");
        yield return null;
    }

    private void Start()
    {
        _enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }

    private void Update()
    {
        if (ifCountdownActive)
        {
            countDownTimer -= Time.deltaTime;
            if (countDownTimer <= 0)
            {
                countDownTimer = 0;
                StartLevel();
            }
        }
    }
}
