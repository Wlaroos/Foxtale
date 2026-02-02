using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FairyAnimation : MonoBehaviour
{
    public static FairyAnimation Instance { get; private set; }

    [Serializable] public class StringSpriteDictionary : SerializableDictionary<string, Sprite> { }
    [SerializeField] private StringSpriteDictionary _faceSprites = new StringSpriteDictionary();
    [SerializeField] GameObject _face;
    [SerializeField] GameObject _leftArm;
    [SerializeField] GameObject _rightArm;
    [SerializeField] GameObject _body;
    [SerializeField] GameObject _tail;
    [SerializeField] SpriteRenderer _collar;
    private string _defaultFaceKey = "Grin"; // Default face key
    private Color _defaultCollarColor = Color.white; // Default collar color
    private Vector3 _defaultLeftArmPosition = new Vector3(-0.62f, -1f, 0); // Default left arm position
    private Quaternion _defaultLeftArmRotation = Quaternion.Euler(0, 0, 0); // Default left arm rotation
    private Vector3 _defaultRightArmPosition = new Vector3(0.62f, -1, 0); // Default right arm position
    private Quaternion _defaultRightArmRotation = Quaternion.Euler(0, 0, 0); // Default right arm rotation

    private SpriteRenderer _faceRenderer;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of FairyAnimation detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _faceRenderer = _face.GetComponent<SpriteRenderer>();

        // Set default values
        ResetToDefault();
    }

    public void ChangeFace(string expressionKey)
    {
        if (_faceSprites.TryGetValue(expressionKey, out Sprite newSprite))
        {
            _faceRenderer.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning($"Expression key '{expressionKey}' not found in face sprites dictionary.");
        }
    }

    public void ArmDefault()
    {
        SetArmPositionAndRotation(_leftArm, _defaultLeftArmPosition, _defaultLeftArmRotation);
        SetArmPositionAndRotation(_rightArm, _defaultRightArmPosition, _defaultRightArmRotation);
    }

    public void ArmsUp()
    {
        ArmPositionAndRotationOffset(_leftArm, new Vector3(-0.4f, 0.5f, 0), Quaternion.Euler(0, 0, -90));
        ArmPositionAndRotationOffset(_rightArm, new Vector3(0.4f, 0.5f, 0), Quaternion.Euler(0, 0, 90));
    }

    private void ArmPositionAndRotationOffset(GameObject arm, Vector3 positionOffset, Quaternion rotation)
    {
        arm.transform.localRotation = rotation;
        arm.transform.position = new Vector3(
            arm.transform.position.x + positionOffset.x,
            arm.transform.position.y + positionOffset.y,
            arm.transform.position.z + positionOffset.z
        );

        arm.GetComponent<RotateHoverUtil>().NewPosistionOffset();
    }

    private void SetArmPositionAndRotation(GameObject arm, Vector3 position, Quaternion rotation)
    {
        arm.transform.localPosition = position;
        arm.transform.localRotation = rotation;

        arm.GetComponent<RotateHoverUtil>().NewPosistionOffset();
    }

    public void CollarColor(Color newColor)
    {
        _collar.color = newColor;
    }

    public void ResetToDefault()
    {
        // Reset face to default
        ChangeFace(_defaultFaceKey);

        // Reset collar color to default
        CollarColor(_defaultCollarColor);

        // Reset arms to default positions and rotations
        ArmDefault();
    }
}

