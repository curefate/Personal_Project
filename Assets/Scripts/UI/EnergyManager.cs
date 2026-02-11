using UnityEngine;

public class EnergyManager : MonoBehaviour
{
    public int maxEnergy;
    [SerializeField] private int energy;
    public int Energy
    {
        get => energy;
        set
        {
            energy = value < 0 ? 0 : value > maxEnergy ? maxEnergy : value;
        }
    }
    public bool increaseEnergy;
    public float increaseInterval;
    public int increaseAmount;

    public float EnergyPercentage => (float)Energy / maxEnergy;

    private float _timer;

    void Update()
    {
        if (increaseEnergy)
        {
            _timer += Time.deltaTime;
            if (_timer >= increaseInterval)
            {
                Energy += increaseAmount;
                _timer = 0f;
            }
        }
    }
}
