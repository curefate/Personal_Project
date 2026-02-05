using UnityEngine;

public enum TowerSizeType
{
    Single,
    Double,
    Quadruple
}

public class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    public TowerSizeType SizeType;
    public int Cost;
    public int Number;
}
