using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioDictionary audioAsset;
    public AudioSource audioSource;
    public bool useCameraSource;

    void Start()
    {
        if (audioSource != null) return;
        if (useCameraSource)
        {
            audioSource = Camera.main.gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = TryGetComponent<AudioSource>(out var source) ? source : gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayOneShot(string key)
    {
        var clip = audioAsset.GetValue(key);
        if (clip == null)
            Debug.LogWarning($"Playing audio: {key} is null");
        else
            audioSource.PlayOneShot(clip);
    }
}
