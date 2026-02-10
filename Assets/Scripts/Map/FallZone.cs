using System.Collections;
using UnityEngine;

public class FallZone : MonoBehaviour
{
    public Transform spawnPoint;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.position = spawnPoint.position;
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
