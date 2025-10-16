using UnityEngine;

public enum TowerSizeType
{
    Single,
    Double,
    Quadruple
}

public class Tower : MonoBehaviour
{
    public TowerSizeType SizeType;
    public int Cost;
    public int Number;
}
