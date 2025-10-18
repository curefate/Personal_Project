using UnityEngine;

public class HitBox : MonoBehaviour
{
    public GameObject Owner;
    public int Damage;

    void Start()
    {
        if (Owner == null)
        {
            Owner = transform.root.gameObject;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        bool isEnemy = Owner.CompareTag("Enemy");

        if (isEnemy)
        {
            if (other.CompareTag("Player") || other.CompareTag("Ally"))
            {
                if (other.TryGetComponent<BattleBase>(out var target))
                {
                    target.TakeDamage(new DamageMessage(Owner, Damage));
                }
            }
        }
        else
        {
            if (other.CompareTag("Enemy"))
            {
                if (other.TryGetComponent<BattleBase>(out var target))
                {
                    target.TakeDamage(new DamageMessage(Owner, Damage));
                }
            }
        }
    }
}
