using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> EnemyPrefabs;
    public Transform[] SpawnPoints;
    public float StartCountDownTime;
    public int SpawnNumber;
    public int MaxWave = 10;
    public int WaveTime = 30;

    public TextMeshProUGUI CountdownText;
    public Button BackToMenuButton;
    public TextMeshProUGUI NextWaveText;
    public Camera GameOverCamera;

    private bool _isGameStarted = false;
    private bool _isGameOver = false;
    private float _spawnOffset = 1f;
    private float _waveTimer = 0f;
    private int _currentWave = 0;

    void Start()
    {
    }

    void Update()
    {
        if (!_isGameStarted)
        {
            CountdownText.gameObject.SetActive(true);
            StartCountDownTime -= Time.deltaTime;
            CountdownText.text = "Enemy is coming in: " + Mathf.CeilToInt(StartCountDownTime).ToString();
            if (StartCountDownTime <= 0f)
            {
                _isGameStarted = true;
                CountdownText.text = "Start!";
                StartCoroutine(DelayDisable(CountdownText.gameObject, 1f));
                NextWaveText.gameObject.SetActive(true);
                SpawnNumber = 10;
                SpawnEnemy();
            }
            return;
        }

        if (_isGameOver) return;

        NextWaveText.text = "Wave " + (_currentWave + 1) + "/" + MaxWave + "\nNext wave: " + Mathf.CeilToInt(WaveTime - _waveTimer) + " seconds";

        _waveTimer += Time.deltaTime;
        if (_waveTimer >= WaveTime)
        {
            _waveTimer = 0f;
            _currentWave++;
            SpawnNumber += 8;
            if (_currentWave > MaxWave)
            {
                Win();
                return;
            }
            SpawnEnemy();
        }

        if (_currentWave > MaxWave)
        {
            Win();
            return;
        }
    }

    private IEnumerator DelayDisable(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    private void SpawnEnemy()
    {
        if (EnemyPrefabs.Count == 0 || SpawnPoints.Length == 0) return;

        var enemyPrefab = EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)];

        for (int i = 0; i < SpawnNumber; i++)
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

    public void Win()
    {
        _isGameOver = true;
        CountdownText.gameObject.SetActive(true);
        CountdownText.text = "You Win! Congratulations!\nThe evil was repelled, you defended the castle";
        BackToMenuButton.gameObject.SetActive(true);
        NextWaveText.gameObject.SetActive(false);
        GameOverCamera.gameObject.SetActive(true);
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        playerController.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Lose()
    {
        _isGameOver = true;
        CountdownText.gameObject.SetActive(true);
        CountdownText.text = "Game Over!";
        BackToMenuButton.gameObject.SetActive(true);
        NextWaveText.gameObject.SetActive(false);
        GameOverCamera.gameObject.SetActive(true);
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        playerController.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
