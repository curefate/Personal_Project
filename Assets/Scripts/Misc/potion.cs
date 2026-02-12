using UnityEngine;

public class potion : MonoBehaviour
{
    public int EnergyAmount;
    public AudioClip CollectSound;

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
            var energyManager = FindFirstObjectByType<EnergyManager>();
            energyManager.Energy += EnergyAmount;
            var mainAudioSource = Camera.main.GetComponent<AudioSource>();
            mainAudioSource.PlayOneShot(CollectSound);
            Destroy(gameObject);
        }
    }
}
