using UnityEngine;

public class BluePrintTray : MonoBehaviour
{
    public GameObject bluePrintPrefab;

    void Update()
    {
        if (transform.childCount == 0 && bluePrintPrefab != null)
        {
            Instantiate(bluePrintPrefab, transform.position, Quaternion.identity, transform);
        }
    }
}
