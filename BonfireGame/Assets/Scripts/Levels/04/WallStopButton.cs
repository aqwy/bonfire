using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallStopButton : MonoBehaviour
{
    public WallController wallMain;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("shoot"))
        {
            wallMain.stopAllWalls();
            Debug.Log("enter trigger");
        }
    }
}
