using UnityEngine;

public class GoldManager : MonoBehaviour
{
    [SerializeField] private int gold;
    public int Gold
    {
        get => gold;
        set
        {
            gold = value < 0 ? 0 : value;
        }
    }
    public bool increaseGold;
    public float increaseInterval;
    public int increaseAmount;

    private float _timer;

    void Update()
    {
        if (increaseGold)
        {
            _timer += Time.deltaTime;
            if (_timer >= increaseInterval)
            {
                Gold += increaseAmount;
                _timer = 0f;
            }
        }
    }
}
