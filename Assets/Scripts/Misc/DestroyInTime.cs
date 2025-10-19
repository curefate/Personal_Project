using System.Collections;
using UnityEngine;

public class DestroyInTime : MonoBehaviour
{
    public float lifetime;

    void Start()
    {
        StartCoroutine(DelayDestroy(lifetime));
    }

    void Update()
    {

    }

    private IEnumerator DelayDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
