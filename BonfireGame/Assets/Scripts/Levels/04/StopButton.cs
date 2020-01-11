using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopButton : MonoBehaviour
{
    public WallController wallMain;
    public SpriteSwaper swaper;
    public SpriteRenderer lightSprite;

    private void Start()
    {
        lightSprite.color = Color.red;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("shoot"))
        {
            wallMain.stopAllWalls();
            swaper.swapSprite();
            lightSprite.color = Color.green;
            Debug.Log("enter trigger");
        }
    }
}
