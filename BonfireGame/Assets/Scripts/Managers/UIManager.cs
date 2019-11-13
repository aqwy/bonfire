using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIManager : Singltone<UIManager>
{
    public RectTransform inventoryPanel;
    public RectTransform mainMenuPanel;
    public RectTransform gamePlayPanel;
    public Image fadeImg;
    public RectTransform newItemSignPanel;
    public GameObject gameplayElements;


    private float _inventoryHeightSize;
    private float _slidesTime;
    private bool _inventoryOpen = false;
    private bool _optionsOpen = false;

    private void Start()
    {
        _inventoryHeightSize = 150f;
        _slidesTime = 0.8f;

        inventoryPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        inventoryPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inventoryHeightSize);
        newItemSignPanel.gameObject.SetActive(false);
    }


    public void openMainMenu()
    {
        if (!_optionsOpen)
        {
            mainMenuPanel.DOAnchorPos(Vector2.zero, _slidesTime);
            gamePlayPanel.gameObject.SetActive(false);
            _optionsOpen = true;
            fadeImg.DOFade(0.6f, 1f);
        }
        else if (_optionsOpen)
        {
            mainMenuPanel.DOAnchorPos(new Vector2(-700f,0f), _slidesTime);
            gamePlayPanel.gameObject.SetActive(true);
            _optionsOpen = false;
            fadeImg.DOFade(0f, 1f);
        }
    }

    public void openInventory()
    {
        if (!_inventoryOpen)
        {
            inventoryPanel.DOSizeDelta(new Vector2(500f, _inventoryHeightSize), _slidesTime, true);
            _inventoryOpen = true;
            gameplayElements.gameObject.SetActive(false);
            fadeImg.DOFade(0.6f, 1f);
        }
        else if (_inventoryOpen)
        {
            inventoryPanel.DOSizeDelta(new Vector2(0f, _inventoryHeightSize), _slidesTime);
            _inventoryOpen = false;
            gameplayElements.gameObject.SetActive(true);
            fadeImg.DOFade(0f, 1f);
        }


        if (newItemSignPanel.gameObject.activeInHierarchy)
        {
            newItemSignPanel.gameObject.SetActive(false);
        }
    }
}
