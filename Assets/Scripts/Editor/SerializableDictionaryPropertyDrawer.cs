using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
public class SerializableDictionaryPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the foldout
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            SerializedProperty keys = property.FindPropertyRelative("keys");
            SerializedProperty values = property.FindPropertyRelative("values");

            if (keys != null && values != null)
            {
                int count = Mathf.Max(keys.arraySize, values.arraySize);

                // Ensure keys and values have the same size
                keys.arraySize = count;
                values.arraySize = count;

                for (int i = 0; i < count; i++)
                {
                    Rect keyRect = new Rect(position.x, position.y + (i + 1) * EditorGUIUtility.singleLineHeight, position.width / 2 - 5, EditorGUIUtility.singleLineHeight);
                    Rect valueRect = new Rect(position.x + position.width / 2 + 5, position.y + (i + 1) * EditorGUIUtility.singleLineHeight, position.width / 2 - 5, EditorGUIUtility.singleLineHeight);

                    EditorGUI.PropertyField(keyRect, keys.GetArrayElementAtIndex(i), GUIContent.none);
                    EditorGUI.PropertyField(valueRect, values.GetArrayElementAtIndex(i), GUIContent.none);
                }

                // Add buttons to add or remove entries
                Rect buttonRect = new Rect(position.x, position.y + (count + 1) * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(new Rect(buttonRect.x, buttonRect.y, buttonRect.width / 2 - 5, buttonRect.height), "Add"))
                {
                    // Add a new entry
                    keys.arraySize++;
                    values.arraySize++;

                    // Initialize the new key with a unique value
                    SerializedProperty newKey = keys.GetArrayElementAtIndex(keys.arraySize - 1);
                    if (newKey.propertyType == SerializedPropertyType.String)
                    {
                        newKey.stringValue = GetUniqueKey(keys);
                    }
                }
                if (GUI.Button(new Rect(buttonRect.x + buttonRect.width / 2 + 5, buttonRect.y, buttonRect.width / 2 - 5, buttonRect.height), "Remove"))
                {
                    // Remove the last entry
                    if (keys.arraySize > 0)
                    {
                        keys.arraySize--;
                        values.arraySize--;
                    }
                }
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

        SerializedProperty keys = property.FindPropertyRelative("keys");
        int count = keys != null ? keys.arraySize : 0;

        return EditorGUIUtility.singleLineHeight * (count + 3); // +3 for foldout, buttons, and spacing
    }

    private string GetUniqueKey(SerializedProperty keys)
    {
        HashSet<string> existingKeys = new HashSet<string>();
        for (int i = 0; i < keys.arraySize; i++)
        {
            SerializedProperty key = keys.GetArrayElementAtIndex(i);
            if (key.propertyType == SerializedPropertyType.String)
            {
                existingKeys.Add(key.stringValue);
            }
        }

        // Generate a unique key
        string newKey = "New Key";
        int counter = 1;
        while (existingKeys.Contains(newKey))
        {
            newKey = $"New Key {counter}";
            counter++;
        }

        return newKey;
    }
}