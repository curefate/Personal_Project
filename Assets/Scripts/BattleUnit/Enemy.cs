using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Enemy : BattleBase
{
    [SerializeField]
    private GameObject CurrentTarget;
    [SerializeField]
    private Collider hitBox;
    private NavMeshAgent _agent;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private bool _isDead = false;
    private Dictionary<GameObject, float> _damagedTimers = new();
    private bool _dominating = false;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
        }
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError("Rigidbody component not found on " + gameObject.name);
        }
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        _animator.SetBool("IsArrive", _agent.velocity.magnitude < 0.1f && CurrentTarget != null);
        if (CurrentTarget == null)
        {
            _animator.SetBool("NoTarget", true);
        }
        else
        {
            _animator.SetBool("NoTarget", false);
        }

        var info = _animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("death") && info.normalizedTime >= 1f)
        {
            Destroy(gameObject);
            return;
        }
        if (info.IsName("slash01") && info.normalizedTime >= 0.4f && info.normalizedTime <= 0.8f)
        {
            hitBox.enabled = true;
            _dominating = true;
        }
        else if (info.IsName("slash02") && info.normalizedTime >= 0.5f && info.normalizedTime <= 0.8f)
        {
            hitBox.enabled = true;
        }
        else
        {
            hitBox.enabled = false;
            _dominating = false;
        }

        if (CurrentTarget != null && _agent != null)
        {
            _agent.SetDestination(CurrentTarget.transform.position);
        }

        if (_agent.remainingDistance <= _agent.stoppingDistance && CurrentTarget != null && _agent.hasPath && !_agent.pathPending)
        {
            transform.LookAt(new Vector3(CurrentTarget.transform.position.x, transform.position.y, CurrentTarget.transform.position.z));
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

        // var stunDir = (transform.position - msg.Source.transform.position).normalized;

        Health -= msg.DamageAmount;
        if (Health <= 0)
        {
            _isDead = true;
            _animator.SetBool("IsDead", true);
        }

        if (!_dominating)
        {
            _animator.SetTrigger("Hited");
        }
    }
}
