using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitBox : MonoBehaviour
{
    [SerializeField]
    private GameObject Owner;
    public int Damage;

    void Start()
    {
        if (Owner == null)
        {
            Owner = transform.root.gameObject;
        }
    }

    public void SetOwner(GameObject owner)
    {
        Owner = owner;
    }

    public void SetPlayerOwner()
    {
        Owner = GameObject.FindWithTag("Player");
    }

    public GameObject GetOwner()
    {
        return Owner;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BattleBase>(out var target))
        {
            target.TakeDamage(new DamageMessage(Owner, Damage));
        }
    }
}
