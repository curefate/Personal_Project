using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public int readonly_level;
    public float restTime;
    [HideInInspector]
    public float restOfRestTime;
    public bool isResting;
    public bool win;
    public bool lose;

    private EnemySpawner _enemySpawner;

    void Update()
    {
        if (isResting)
        {
            restOfRestTime -= Time.deltaTime;
        }
        else
        {
            restOfRestTime = restTime;
        }
    }

    public void StartLevel()
    {
        StopAllCoroutines();
        var goldmanager = FindFirstObjectByType<GoldManager>();
        goldmanager.Gold = 1000;
        goldmanager.increaseGold = true;
        StartCoroutine(Level_1());
    }

    IEnumerator Level_1()
    {
        Debug.Log("Starting Level 1");
        isResting = true;
        yield return new WaitForSeconds(restTime);
        isResting = false;
        readonly_level = 1;
        _enemySpawner.SpawnEnemy(8, "skeleton");
        yield return new WaitForSeconds(6f);
        _enemySpawner.SpawnEnemy(3, "skeleton");
        yield return new WaitForSeconds(6f);
        _enemySpawner.SpawnEnemy(3, "gold_skeleton");
        while (_enemySpawner.ActiveEnemies.Count > 0)
        {
            yield return null;
        }
        StartCoroutine(Level_2());
    }

    IEnumerator Level_2()
    {
        Debug.Log("Starting Level 2");
        isResting = true;
        yield return new WaitForSeconds(restTime);
        isResting = false;
        readonly_level = 2;
        _enemySpawner.SpawnEnemy(5, "skeleton");
        _enemySpawner.SpawnEnemy(5, "gold_skeleton");
        yield return new WaitForSeconds(15f);
        _enemySpawner.SpawnEnemy(10, "skeleton");
        _enemySpawner.SpawnEnemy(5, "gold_skeleton");
        while (_enemySpawner.ActiveEnemies.Count > 0)
        {
            yield return null;
        }
        StartCoroutine(Level_3());
    }

    IEnumerator Level_3()
    {
        Debug.Log("Starting Level 3");
        isResting = true;
        yield return new WaitForSeconds(restTime);
        isResting = false;
        readonly_level = 3;
        _enemySpawner.SpawnEnemy(8, "skeleton");
        _enemySpawner.SpawnEnemy(7, "gold_skeleton");
        yield return new WaitForSeconds(10f);
        while (_enemySpawner.ActiveEnemies.Count > 7)
        {
            yield return null;
        }
        _enemySpawner.SpawnEnemy(1, "huge_skeleton");
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
        win = true;
        yield return null;
    }

    private void Start()
    {
        _enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }
}
