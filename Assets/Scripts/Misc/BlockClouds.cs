using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockClouds : MonoBehaviour
{
    public float cloudSpeed = 5f;

    private bool _ifFlying;

    public void AllCloudsFly()
    {
        if (_ifFlying) return;

        _ifFlying = true;

        List<Transform> clouds = new List<Transform>();
        GetAllChildren(ref clouds, transform);

        foreach (Transform cloud in clouds)
        {
            StartCoroutine(PushCloud(cloud));
        }

        var colliders = GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        levelManager.StartLevel();
        Destroy(gameObject, 5f);
    }

    private void GetAllChildren(ref List<Transform> children, Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.childCount > 0)
            {
                GetAllChildren(ref children, child);
            }
            else
            {
                children.Add(child);
            }
        }
    }

    IEnumerator PushCloud(Transform cloud)
    {
        var direction = (cloud.position - transform.position).normalized;
        while (true)
        {
            cloud.position += direction * Time.deltaTime * cloudSpeed;
            yield return null;
        }
    }
}