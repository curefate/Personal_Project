using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public PrefabDictionary particleDict;

    public void SpawnParticle(string key, Vector3 position)
    {
        var prefab = particleDict.GetValue(key);
        if (prefab == null) return;
        var instance = Instantiate(prefab, position, Quaternion.identity);
        instance.GetComponent<ParticleSystem>().Play();
        Destroy(instance, 3f);
    }
}
