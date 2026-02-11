using UnityEngine;

public class ButtonFuncs : MonoBehaviour
{
    private AudioPlayer _audioPlayer;

    private void Start()
    {
        _audioPlayer = GetComponent<AudioPlayer>();
    }

    public void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    public void BuySword(GameObject sword)
    {
        var goldManager = FindFirstObjectByType<GoldManager>();
        if (goldManager.Gold < 10)
        {
            _audioPlayer.PlayOneShotFromAsset("error");
            return;
        }
        goldManager.Gold -= 10;
        _audioPlayer.PlayOneShotFromAsset("build");
        var pos = Camera.main.transform.position + new Vector3(0, 1, 0) + Camera.main.transform.forward * 1f;
        Instantiate(sword, pos, new Quaternion(Random.value, Random.value, Random.value, Random.value).normalized);
    }
}
