using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Castle : BattleBase
{
    private Dictionary<GameObject, float> _damagedTimers = new();
    private bool _isDead = false;

    void Start()
    {
        Health = 2000;
    }

    void Update()
    {
        foreach (var key in _damagedTimers.Keys.ToList())
        {
            _damagedTimers[key] += Time.deltaTime;
        }
    }

    public override void TakeDamage(DamageMessage msg)
    {
        if (_isDead) return;
        if (_damagedTimers.TryGetValue(msg.Source, out float timer) && timer < _damagedInterval) return;

        Health -= msg.DamageAmount;
        if (Health <= 0)
        {
            _isDead = true;
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.Lose();
        }
    }
}
