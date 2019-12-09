using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class itemMinigame : MonoBehaviour, IPointerClickHandler
{
    public Image currentColor;

    private Color _currCol;

    /*void Start()
    {
        _currCol = GetComponent<Image>().color;
    }*/

    public Color getItemColor()
    {
        return _currCol = GetComponent<Image>().color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("paint butt click " + _currCol + " color");
        currentColor.color = getItemColor();
    }
}
