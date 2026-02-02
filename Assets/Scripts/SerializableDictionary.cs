using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    // Save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            if (pair.Key != null && pair.Value != null) // Skip null entries
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
    }

    // Load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError($"SerializableDictionary: Mismatch between keys ({keys.Count}) and values ({values.Count}) during deserialization.");
            return; // Prevent further processing
        }

        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i] != null && values[i] != null) // Skip null entries
            {
                this.Add(keys[i], values[i]);
            }
        }
    }
}