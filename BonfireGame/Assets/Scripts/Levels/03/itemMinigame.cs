using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemMinigame : MonoBehaviour
{
    private Color _currCol;
   
    void Start()
    {
        _currCol = GetComponent<Image>().color;
    }

    public Color getItemColor()
    {
        return _currCol;
    }    
}
