using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Coffee.UIExtensions;

public class itemPartTwoMinigame : MonoBehaviour, IPointerClickHandler
{
    public Image paintCurrColor;
    public UIEffect imgEffect;

    private Color _trueItemColor;
    private Color _paintColor;
    private Color _normalColor;

    void Start()
    {
        _normalColor = Color.white;
    }

    private void OnEnable()
    {
        _trueItemColor = GetComponent<Image>().color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _paintColor = paintCurrColor.GetComponent<PaintMainColor>().getMainPaintColor();
        if (!_paintColor.Equals(_normalColor))
        {
            this.GetComponent<Image>().color = _paintColor;
            imgEffect.enabled = false;
            Debug.Log("cvet raskrasi ne beliy");
        }
    }

    public Color getTrueColor()
    {
        return _trueItemColor;
    }
}
