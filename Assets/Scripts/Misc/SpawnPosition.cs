using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        AlignPlayerWithSpawnPoint();
    }

    public void AlignPlayerWithSpawnPoint()
    {
        if (player != null)
        {
            player.transform.position = transform.position;
        }
    }
}
