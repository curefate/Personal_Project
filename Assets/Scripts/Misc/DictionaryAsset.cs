
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "TextureDictionary", menuName = "Custom/Texture Dictionary Asset")]
public class TextureDictionary : DictionaryAsset<Texture> { }

[CreateAssetMenu(fileName = "AudioDictionary", menuName = "Custom/Audio Dictionary Asset")]
public class AudioDictionary : DictionaryAsset<AudioClip> { }

public abstract class DictionaryAsset<T> : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public string key = "";
        public T value = default;
    }

    public List<Entry> entries = new();

    private Dictionary<string, T> dict = null;

    public T GetValue(string key)
    {
        if (dict == null)
        {
            dict = new Dictionary<string, T>();
            foreach (var e in entries)
            {
                if (!dict.ContainsKey(e.key))
                    dict.Add(e.key, e.value);
            }
        }

        if (dict.TryGetValue(key, out T value))
            return value;

        Debug.LogWarning($"DictionaryAsset: Don't have value for key = {key}");
        return default;
    }
}