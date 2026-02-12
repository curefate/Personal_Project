using UnityEngine;

public class gem : MonoBehaviour
{
    public int GoldValue = 30;
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
            var goldmanager = FindFirstObjectByType<GoldManager>();
            goldmanager.Gold += GoldValue;
            var mainAudioSource = Camera.main.GetComponent<AudioSource>();
            mainAudioSource.PlayOneShot(CollectSound);
            Destroy(gameObject);
        }
    }
}
