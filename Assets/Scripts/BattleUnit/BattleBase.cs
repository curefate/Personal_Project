using System.Collections.Generic;
using UnityEngine;

public abstract class BattleBase : MonoBehaviour
{
    [Header("Battle Unit Stats")]
    public int Health;
    protected float _damagedInterval = 0.4f;

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