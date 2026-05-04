using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
public class SerializableDictionaryPropertyDrawer : PropertyDrawer
{
    private bool _showGrid = false;
    private const float SpriteSize = 64f;
    private const float SpritePadding = 10f;
    private const float LabelHeight = 18f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 1. Draw the Foldout and a Grid Toggle on the same line
        Rect foldoutRect = new Rect(position.x, position.y, position.width - 60, EditorGUIUtility.singleLineHeight);
        Rect toggleRect = new Rect(position.x + position.width - 55, position.y, 55, EditorGUIUtility.singleLineHeight);

        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
        
        // Only show grid toggle if the value type is a Sprite
        bool isSpriteType = property.type.Contains("Sprite");
        if (isSpriteType)
        {
            _showGrid = GUI.Toggle(toggleRect, _showGrid, "Grid", "Button");
        }

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            SerializedProperty keys = property.FindPropertyRelative("keys");
            SerializedProperty values = property.FindPropertyRelative("values");

            if (keys != null && values != null)
            {
                int count = Mathf.Max(keys.arraySize, values.arraySize);
                keys.arraySize = count;
                values.arraySize = count;

                if (_showGrid && isSpriteType)
                {
                    DrawSpriteGrid(position, keys, values, count);
                }
                else
                {
                    DrawStandardList(position, keys, values, count);
                }

                // Buttons: Add / Remove
                float buttonsY = _showGrid ? GetGridHeight(count, position.width) : (count + 1) * EditorGUIUtility.singleLineHeight;
                Rect buttonRect = new Rect(position.x, position.y + buttonsY, position.width, EditorGUIUtility.singleLineHeight);
                
                if (GUI.Button(new Rect(buttonRect.x, buttonRect.y, buttonRect.width / 2 - 5, buttonRect.height), "Add"))
                {
                    keys.arraySize++;
                    values.arraySize++;
                    SerializedProperty newKey = keys.GetArrayElementAtIndex(keys.arraySize - 1);
                    if (newKey.propertyType == SerializedPropertyType.String)
                        newKey.stringValue = GetUniqueKey(keys);
                }
                if (GUI.Button(new Rect(buttonRect.x + buttonRect.width / 2 + 5, buttonRect.y, buttonRect.width / 2 - 5, buttonRect.height), "Remove"))
                {
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

    private void DrawStandardList(Rect position, SerializedProperty keys, SerializedProperty values, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Rect keyRect = new Rect(position.x, position.y + (i + 1) * EditorGUIUtility.singleLineHeight, position.width / 2 - 5, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(position.x + position.width / 2 + 5, position.y + (i + 1) * EditorGUIUtility.singleLineHeight, position.width / 2 - 5, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(keyRect, keys.GetArrayElementAtIndex(i), GUIContent.none);
            EditorGUI.PropertyField(valueRect, values.GetArrayElementAtIndex(i), GUIContent.none);
        }
    }

    private void DrawSpriteGrid(Rect position, SerializedProperty keys, SerializedProperty values, int count)
    {
        // Important: Reset indent for the grid so calculations aren't offset by the foldout margin
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float startY = position.y + EditorGUIUtility.singleLineHeight + 5;
        // Account for the fact that we are inside an inspector which might have padding
        float availableWidth = position.width;
        int columns = Mathf.FloorToInt(availableWidth / (SpriteSize + SpritePadding));
        if (columns < 1) columns = 1;

        for (int i = 0; i < count; i++)
        {
            int row = i / columns;
            int col = i % columns;

            Rect elementRect = new Rect(
                position.x + col * (SpriteSize + SpritePadding),
                startY + row * (SpriteSize + LabelHeight + SpritePadding),
                SpriteSize,
                SpriteSize + LabelHeight
            );

            // 1. Draw Key (The string field) - Forced to SpriteSize width
            Rect labelRect = new Rect(elementRect.x, elementRect.y, SpriteSize, LabelHeight);
            SerializedProperty keyProp = keys.GetArrayElementAtIndex(i);
            GUIStyle centeredStyle = new GUIStyle(EditorStyles.miniTextField) { alignment = TextAnchor.MiddleCenter };
            keyProp.stringValue = EditorGUI.TextField(labelRect, keyProp.stringValue, centeredStyle);

            // 2. Draw Value (The Sprite Object slot) - Forced to SpriteSize width/height
            Rect spriteRect = new Rect(elementRect.x, elementRect.y + LabelHeight, SpriteSize, SpriteSize);
            SerializedProperty valueProp = values.GetArrayElementAtIndex(i);
            
            // Use a background box to visualize the slot area
            GUI.Box(spriteRect, GUIContent.none, EditorStyles.helpBox);
            
            // Draw the property field. We use GUIContent.none to ensure no text label is drawn
            EditorGUI.PropertyField(spriteRect, valueProp, GUIContent.none);

            // 3. Overlay Preview
            if (valueProp.objectReferenceValue is Sprite sprite)
            {
                Texture2D texture = AssetPreview.GetAssetPreview(sprite);
                if (texture != null)
                {
                    // We draw the texture slightly smaller or with ScaleToFit to keep it neat
                    GUI.DrawTexture(spriteRect, texture, ScaleMode.ScaleToFit);
                }
            }
        }

        // Restore indent
        EditorGUI.indentLevel = indent;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

        SerializedProperty keys = property.FindPropertyRelative("keys");
        int count = keys != null ? keys.arraySize : 0;

        if (_showGrid && property.type.Contains("Sprite"))
        {
            // Calculate grid height + header + buttons
            return GetGridHeight(count, EditorGUIUtility.currentViewWidth) + EditorGUIUtility.singleLineHeight + 25;
        }

        return EditorGUIUtility.singleLineHeight * (count + 3);
    }

    private float GetGridHeight(int count, float width)
    {
        int columns = Mathf.FloorToInt(width / (SpriteSize + SpritePadding));
        if (columns < 1) columns = 1;
        int rows = Mathf.CeilToInt((float)count / columns);
        return rows * (SpriteSize + LabelHeight + SpritePadding) + EditorGUIUtility.singleLineHeight;
    }

    private string GetUniqueKey(SerializedProperty keys)
    {
        HashSet<string> existingKeys = new HashSet<string>();
        for (int i = 0; i < keys.arraySize; i++)
        {
            SerializedProperty key = keys.GetArrayElementAtIndex(i);
            if (key.propertyType == SerializedPropertyType.String)
                existingKeys.Add(key.stringValue);
        }

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