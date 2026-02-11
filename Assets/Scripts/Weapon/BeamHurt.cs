using UnityEngine;

public class BeamHurt : MonoBehaviour
{
    public HitBox hitBox;
    public int damage = 80;

    void OnParticleCollision(GameObject other)
    {
        if (other.layer == LayerMask.NameToLayer("Enemy") && other.TryGetComponent(out BattleBase enemy))
        {
            enemy.TakeDamage(new DamageMessage(hitBox.GetOwner(), damage));
        }
    }
}
