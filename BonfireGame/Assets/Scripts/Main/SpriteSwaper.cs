using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSwaper : MonoBehaviour
{
    public Sprite spriteToUse;
    public SpriteRenderer spriteRenderer;

    private Sprite _originalSprite;
    public void swapSprite()
    {
        if (spriteToUse != spriteRenderer.sprite)
        {
            _originalSprite = spriteRenderer.sprite;
            spriteRenderer.sprite = spriteToUse;
        }
    }

    public void resetSprite()
    {
        if (_originalSprite)
        {
            spriteRenderer.sprite = _originalSprite;
        }
    }
}
