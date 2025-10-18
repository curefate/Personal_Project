using System.Collections;
using UnityEngine;

public class FallZone : MonoBehaviour
{
    public int FallDamage = 20;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BattleBase>(out var battleBase))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                DamageMessage dmgMsg = new(transform.gameObject, FallDamage);
                battleBase.TakeDamage(dmgMsg);
                PlayerController player = (PlayerController)battleBase;
                player.Transport(player.LastGroundedPosition);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}
