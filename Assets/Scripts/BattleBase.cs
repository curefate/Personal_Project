using UnityEngine;

public abstract class BattleBase : MonoBehaviour
{
    public int Health;

    public abstract void TakeDamage(DamageMessage msg);
}

public class DamageMessage
{
    public GameObject Source;
    public int DamageAmount;

    public DamageMessage(GameObject source, int damage)
    {
        Source = source;
        DamageAmount = damage;
    }
}