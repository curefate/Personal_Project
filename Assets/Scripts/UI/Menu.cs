using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Menu : MonoBehaviour
{
    public Button StartButton;
    public Button GuideButton;
    public Button BackButton;
    public GameObject GuidePanel;

    private AudioSource _audioSource;
    public AudioClip ButtonClickSound;

    void Start()
    {
        StartButton.onClick.AddListener(OnStartButtonClick);
        GuideButton.onClick.AddListener(OnGuideButtonClick);
        BackButton.onClick.AddListener(OnBackButtonClick);
        GuidePanel.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnStartButtonClick()
    {
        _audioSource.PlayOneShot(ButtonClickSound);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private void OnGuideButtonClick()
    {
        _audioSource.PlayOneShot(ButtonClickSound);
        // Show the guide panel
        GuidePanel.SetActive(true);
    }

    private void OnBackButtonClick()
    {
        _audioSource.PlayOneShot(ButtonClickSound);
        // Hide the guide panel
        GuidePanel.SetActive(false);
    }
}
