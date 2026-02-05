using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    public AudioDictionary audioAsset;
    public AudioSource audioSource;

    void Start()
    {
        audioSource = TryGetComponent<AudioSource>(out var source) ? source : gameObject.AddComponent<AudioSource>();
    }

    public void Play(string key)
    {
        var clip = audioAsset.GetValue(key);
        if (clip == null)
            Debug.LogWarning($"Playing audio: {key} is null");
        else
            audioSource.PlayOneShot(clip);
    }
}
