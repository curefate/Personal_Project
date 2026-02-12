using TMPro;
using UnityEngine;

public class LevelMessageText : MonoBehaviour
{
    private LevelManager _levelManager;
    private EnemySpawner _enemySpawner;
    public TextMeshProUGUI text;

    void Start()
    {
        _levelManager = FindFirstObjectByType<LevelManager>();
        _enemySpawner = FindFirstObjectByType<EnemySpawner>();
    }

    void Update()
    {
        if (_levelManager.lose)
        {
            text.text = "You lost! Try again.";
        }
        else if (_levelManager.win)
        {
            text.text = "All levels completed! You saved the kingdom!";
        }
        else if (_levelManager.isResting)
        {
            text.text = $"They are coming in {_levelManager.restOfRestTime:F1} seconds...";
        }
        else
        {

            text.text = $"Level {_levelManager.readonly_level}\nRest of Enemies: {_enemySpawner.ActiveEnemies.Count}";

        }
    }
}
