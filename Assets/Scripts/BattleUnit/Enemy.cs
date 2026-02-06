using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Enemy : BattleBase
{
    public List<GameObject> DropItemPrefabs;
    public AudioClip DeathSound;
    public AudioClip HitSound;

    [SerializeField]
    private GameObject CurrentTarget;
    [SerializeField]
    private Collider hitBox;
    private List<SkinnedMeshRenderer> _renderersNeedToFlash;
    [SerializeField]
    private Material flashMaterial;
    private NavMeshAgent _agent;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private AudioSource _audioSource;
    private bool _isDead = false;
    private Dictionary<GameObject, float> _damagedTimers = new();
    private bool _dominating = false;
    private Dictionary<GameObject, float> _targetPriorities = new();
    private TowerManager towerManager;
    private GameObject _castle;

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
        towerManager = FindFirstObjectByType<TowerManager>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.Play();
        _castle = FindFirstObjectByType<Castle>().gameObject;
        _targetPriorities[_castle] = 6000;
        CurrentTarget = _castle;
        _renderersNeedToFlash = GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
    }

    void Update()
    {
        var isArrive = _agent.remainingDistance <= _agent.stoppingDistance && !_agent.pathPending;
        _animator.SetBool("IsArrive", isArrive);
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
            if (DropItemPrefabs != null && DropItemPrefabs.Count > 0 && Random.value < 0.5f)
            {
                var randomIndex = Random.Range(0, DropItemPrefabs.Count);
                var item = DropItemPrefabs[randomIndex];
                Instantiate(item, transform.position + item.transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
            return;
        }
        if (info.IsName("slash01") && info.normalizedTime >= 0.2f && info.normalizedTime <= 0.8f)
        {
            hitBox.enabled = true;
            _dominating = true;
            if (_audioSource != null && HitSound != null && !_audioSource.isPlaying)
            {
                _audioSource.PlayOneShot(HitSound);
            }
        }
        else if (info.IsName("slash 02") && info.normalizedTime >= 0.5f && info.normalizedTime <= 0.8f)
        {
            hitBox.enabled = true;
            if (_audioSource != null && HitSound != null && !_audioSource.isPlaying)
            {
                _audioSource.PlayOneShot(HitSound);
            }
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

        HandlePriorityTargeting();
    }

    public override void TakeDamage(DamageMessage msg)
    {
        if (_isDead) return;
        if (_damagedTimers.TryGetValue(msg.Source, out float timer) && timer < _damagedInterval) return;

        _damagedTimers[msg.Source] = 0f;

        if (_targetPriorities.ContainsKey(msg.Source))
        {
            _targetPriorities[msg.Source] += 3000f;
        }
        else
        {
            _targetPriorities[msg.Source] = 3000f;
        }

        StartCoroutine(FlashEffect());

        Health -= msg.DamageAmount;
        if (Health <= 0)
        {
            _isDead = true;
            _animator.SetBool("IsDead", true);
            if (_audioSource != null && DeathSound != null)
            {
                _audioSource.PlayOneShot(DeathSound);
            }
        }

        if (!_dominating)
        {
            _animator.SetTrigger("Hited");
        }
    }

    private void HandlePriorityTargeting()
    {
        float highestPriority = 0f;
        foreach (var target in _targetPriorities.Keys.ToList())
        {
            if (target == null)
            {
                _targetPriorities.Remove(target);
                continue;
            }

            if (_targetPriorities[target] > highestPriority && CheckTargetValidity(target))
            {
                highestPriority = _targetPriorities[target];
                CurrentTarget = target;
            }
        }

        if (highestPriority == 0f || !CheckTargetValidity(CurrentTarget))
        {
            var nearestTower = towerManager.TowerList
                .Where(tower => tower != null)
                .OrderBy(tower => Vector3.Distance(tower.transform.position, _castle.transform.position));
            foreach (var tower in nearestTower)
            {
                if (CheckTargetValidity(tower.gameObject))
                {
                    CurrentTarget = tower;
                    return;
                }
            }
        }

        if (CurrentTarget == null) CurrentTarget = _castle;
    }

    private bool CheckTargetValidity(GameObject target)
    {
        if (target == null)
        {
            return false;
        }

        Vector3 targetPosition = target.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, _agent.stoppingDistance, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            if (_agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {

                float distToTarget = Vector3.Distance(hit.position, targetPosition);
                return distToTarget <= _agent.stoppingDistance;

            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private IEnumerator FlashEffect()
    {
        var originalMaterials = new List<Material>();
        foreach (var renderer in _renderersNeedToFlash)
        {
            originalMaterials.Add(renderer.material);
            renderer.material = flashMaterial;
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < _renderersNeedToFlash.Count; i++)
        {
            _renderersNeedToFlash[i].material = originalMaterials[i];
        }
    }
}
