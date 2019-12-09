using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintMainColor : MonoBehaviour
{
    private Color _currCol;
    public Color getMainPaintColor()
    {
        return _currCol = GetComponent<Image>().color;
    }
}
