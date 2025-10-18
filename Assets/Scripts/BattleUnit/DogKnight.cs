using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DogKnight : BattleBase
{
    [SerializeField]
    private Collider hitBox;
    [SerializeField]
    private float detectionRadius = 3.6f;
    private float _attackRange = 2.8f;
    private bool _dominating = false;
    private Animator _animator;
    private Dictionary<GameObject, float> _damagedTimers = new();
    private bool _isDead = false;


    void Start()
    {
        _animator = GetComponent<Animator>();
        hitBox.enabled = false;
        hitBox.isTrigger = true;
    }

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, LayerMask.GetMask("Enemy"));
        List<Collider> sortedHits = hits
            .Where(c => c.CompareTag("Enemy"))
            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
            .ToList();
        if (sortedHits.Count > 0)
        {
            var target = sortedHits[0].transform;
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
            if (Vector3.Distance(transform.position, target.position) <= _attackRange)
            {
                _animator.SetBool("IsAttacking", true);
            }
        }
        else
        {
            _animator.SetBool("IsAttacking", false);
        }


        var info = _animator.GetCurrentAnimatorStateInfo(0);

        if (info.IsName("Die") && info.normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }

        if (info.IsName("Attack01") && info.normalizedTime >= 0.3f && info.normalizedTime <= 0.8f)
        {
            hitBox.enabled = true;
        }
        else if (info.IsName("Attack02") && info.normalizedTime >= 0.5f && info.normalizedTime <= 0.8f)
        {
            hitBox.enabled = true;
            _dominating = true;
        }
        else
        {
            hitBox.enabled = false;
            _dominating = false;
        }

        foreach (var key in _damagedTimers.Keys.ToList())
        {
            _damagedTimers[key] += Time.deltaTime;
        }
    }

    public override void TakeDamage(DamageMessage msg)
    {
        if (_isDead) return;
        if (_damagedTimers.TryGetValue(msg.Source, out float timer) && timer < _damagedInterval) return;

        _damagedTimers[msg.Source] = 0f;

        Health -= msg.DamageAmount;
        if (Health <= 0)
        {
            _isDead = true;
            _animator.SetBool("IsDead", true);
        }

        if (!_dominating)
        {
            _animator.SetTrigger("GetHit");
        }
    }
}