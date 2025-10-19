using UnityEngine;

public class potion : MonoBehaviour
{
    public int HealAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Heal(HealAmount);
                player.audioSource.PlayOneShot(player.HealSound);
                Destroy(gameObject);
            }
        }
    }
}
