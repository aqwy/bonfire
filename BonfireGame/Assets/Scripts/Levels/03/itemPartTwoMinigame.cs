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
    public bool isTop;
    public minigameParent minigame;

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
        if (isTop)
        {
            _paintColor = paintCurrColor.GetComponent<PaintMainColor>().getMainPaintColor();
            if (!_paintColor.Equals(_normalColor))
            {
                this.GetComponent<Image>().color = _paintColor;
                imgEffect.enabled = false;
                Debug.Log("cvet raskrasi ne beliy");
            }
        }
        else if (!isTop)
        {
            _paintColor = paintCurrColor.GetComponent<PaintMainColor>().getMainPaintColor();
            if (!_paintColor.Equals(_normalColor))
            {
                this.GetComponent<Image>().color = _paintColor;
                imgEffect.enabled = false;
                minigame.sightImg.color = getTrueColor();
            }
            else
            {
                minigame.sightImg.color = getTrueColor();
            }
        }
    }

    public Color getTrueColor()
    {
        return _trueItemColor;
    }
}
