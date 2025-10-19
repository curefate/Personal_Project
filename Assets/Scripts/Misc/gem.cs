using UnityEngine;

public class gem : MonoBehaviour
{
    public int GoldValue = 30;

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
            player.Gold += GoldValue;
            player.audioSource.PlayOneShot(player.GetMoneySound);
            Destroy(gameObject);
        }
    }
}
