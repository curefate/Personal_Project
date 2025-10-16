using UnityEngine;
using UnityEngine.AI;

public class Enemy : BattleBase
{
    [SerializeField]
    private GameObject CurrentTarget;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (CurrentTarget != null && agent != null)
        {
            agent.SetDestination(CurrentTarget.transform.position);
        }
    }

    public override void TakeDamage(DamageMessage msg)
    {
        Health -= msg.DamageAmount;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
