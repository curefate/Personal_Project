using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(AudioSource))]
public class Cannon : BattleBase
{
    public int Damage;
    [SerializeField]
    private ParticleSystem _explosionEffect;
    [SerializeField]
    private ParticleSystem _fireEffect;
    [SerializeField]
    private float fireInterval;
    [SerializeField]
    private float detectionRadius;
    [SerializeField]
    private float explosionRadius;

    private Dictionary<GameObject, float> _damagedTimers = new();
    private bool _isDead = false;
    private float _fireTimer = 0f;

    private AudioSource _audioSource;
    public AudioClip FireSound;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
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
            var direction = (target.position - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 3f);
            if (_fireTimer >= fireInterval)
            {
                _fireTimer = 0f;
                _fireEffect.Play();
                StartCoroutine(FireExplosion(0.5f, target.position));
            }
        }

        foreach (var key in _damagedTimers.Keys.ToList())
        {
            _damagedTimers[key] += Time.deltaTime;
        }
        _fireTimer += Time.deltaTime;
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
            _explosionEffect.transform.position = transform.position;
            StartCoroutine(FireExplosion(.1f, transform.position));
            StartCoroutine(DelayDestroy(.3f));
        }
    }

    private IEnumerator DelayDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private IEnumerator FireExplosion(float delay, Vector3 position)
    {
        yield return new WaitForSeconds(delay);
        _explosionEffect.transform.position = position;
        _explosionEffect.Play();
        if (_audioSource != null && FireSound != null)
        {
            _audioSource.PlayOneShot(FireSound);
        }
        Collider[] rangeHits = Physics.OverlapSphere(position, explosionRadius, LayerMask.GetMask("Enemy"));
        foreach (var hit in rangeHits)
        {
            if (hit.CompareTag("Enemy"))
            {
                var battleBase = hit.GetComponent<BattleBase>();
                if (battleBase != null)
                {
                    battleBase.TakeDamage(new DamageMessage(gameObject, Damage));
                }
            }
        }
    }
}