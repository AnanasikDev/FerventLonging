using System;
using UnityEngine;
using UnityEngine.UI;

public class Tip : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    public tipType tipType;
    public Func<(bool, Vector2)> stateFunction;
    public bool isActive = false;

    public void UpdateTip()
    {
        var s = stateFunction();

        if (s.Item1)
        {
            if (!isActive)
                Enable();
            transform.position = s.Item2;
        }
        else
        {
            if (isActive)
                Disable();
        }
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        isActive = false;
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        isActive = true;
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}