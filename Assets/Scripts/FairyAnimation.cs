using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FairyAnimation : MonoBehaviour
{
    public static FairyAnimation Instance { get; private set; }

    [SerializeField] private bool _randomFaceAtStart;
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

        if (_randomFaceAtStart)
        {
            SetRandomFace();
        }
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

        if(DialogueManager.Instance != null)
        {
            switch (expressionKey)
            {
                case "Evil":
                    DialogueManager.Instance._mainPitch = 0.9f;
                    break;
                case "Smile":
                    DialogueManager.Instance._mainPitch = 1.00f;
                    break;
                case "Confused":
                    DialogueManager.Instance._mainPitch = 1.05f;
                    break;
                case "Angry":
                    DialogueManager.Instance._mainPitch = 0.90f;
                    break;
                case "Stare":
                    DialogueManager.Instance._mainPitch = 0.85f;
                    break;
                case "Grin":
                    DialogueManager.Instance._mainPitch = 1.05f;
                    break;
                case "Squint":
                    DialogueManager.Instance._mainPitch = 1.00f;
                    break;
                case "Cat":
                    DialogueManager.Instance._mainPitch = 1.10f;
                    break;
                case "Sad":
                    DialogueManager.Instance._mainPitch = 0.95f;
                    break;
                case "Creeper":
                    DialogueManager.Instance._mainPitch = 0.90f;
                    break;
                case "Deadpan":
                    DialogueManager.Instance._mainPitch = 0.85f;
                    break;
                case "Happy":
                    DialogueManager.Instance._mainPitch = 1.00f;
                    break;
                case "Surprised":
                    DialogueManager.Instance._mainPitch = 1.05f;
                    break;
                case "SuperStare":
                    DialogueManager.Instance._mainPitch = 0.80f;
                    break;
                case "Fresh":
                    DialogueManager.Instance._mainPitch = 0.95f;
                    break;
                case "Wink":
                    DialogueManager.Instance._mainPitch = 1.10f;
                    break;
                case "Shock":
                    DialogueManager.Instance._mainPitch = 1.10f;
                    break;
                    
            }
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

    public void SetRandomFace()
    {
        if (_faceSprites == null || _faceSprites.Count == 0)
        {
            Debug.LogWarning("Cannot set random face: Dictionary is empty.");
            return;
        }

        // Convert dictionary keys to a list to pick a random index
        List<string> keys = new List<string>(_faceSprites.Keys);
        string randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];
        
        ChangeFace(randomKey);
    }
}


