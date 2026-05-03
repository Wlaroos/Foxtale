using System;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    private int _clicksToBreak = 3;
    private ParticleSystem _breakEffect;
    private ParticleSystem _boneEffect;

    public event Action<BreakableObject> Broken;

    private int _clicks;

    public void Initialize(int clicksToBreak, ParticleSystem breakEffect, ParticleSystem boneEffect)
    {
        _clicksToBreak = clicksToBreak;
        _breakEffect = breakEffect;
        _boneEffect = boneEffect;
    }

    private void OnMouseDown()
    {
        HandleClick((Vector2)transform.position);
    }

    public void HandleClick(Vector2 hitPosition)
    {
        _clicks++;

        if (_boneEffect != null)
        {
            Instantiate(_boneEffect, hitPosition, Quaternion.identity);
            SFXManager.Instance.PlayBoneClick();
        }

        if (_clicks >= _clicksToBreak)
        {
            if (_breakEffect != null)
                Instantiate(_breakEffect, hitPosition, Quaternion.identity);

            Broken?.Invoke(this);
            SFXManager.Instance.PlayBoneBreak();
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Broken = null;
    }
}
