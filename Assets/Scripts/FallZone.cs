using System.Collections;
using UnityEngine;

public class FallZone : MonoBehaviour
{
    public int FallDamage = 20;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            CharacterController controller = other.GetComponent<CharacterController>();
            if (player != null)
            {
                player.TakeDamage(new DamageMessage(null, FallDamage));
                player.ResetVelocity();
                controller.enabled = false;
                other.transform.position = player.LastGroundedPosition;
                controller.enabled = true;
            }
        }

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(new DamageMessage(null, 9999));
            }
        }
    }
}
