using Unity.VisualScripting;
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
            audioSource = Camera.main.TryGetComponent<AudioSource>(out var source) ? source : Camera.main.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = TryGetComponent<AudioSource>(out var source) ? source : gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayOneShotFromAsset(string key)
    {
        var clip = audioAsset.GetValue(key);
        if (clip == null)
            Debug.LogWarning($"Playing audio: {key} is null");
        else
            audioSource.PlayOneShot(clip);
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip == null)
            Debug.LogWarning($"Playing audio: clip is null");
        else
            audioSource.PlayOneShot(clip);
    }
}
