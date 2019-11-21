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
    /*public RectTransform particlePanel;*/
    public GameObject gameplayElements;
    public List<GameObject> particles;

    private float _inventoryHeightSize;
    private float _slidesTime;
    private bool _inventoryOpen = false;
    private bool _optionsOpen = false;
    private Equippableitem _equpedItem;
    private int _particleCount;

    private void Start()
    {
        _inventoryHeightSize = 150f;
        _slidesTime = 0.8f;
        _particleCount = 0;

        inventoryPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
        inventoryPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inventoryHeightSize);
        newItemSignPanel.gameObject.SetActive(false);
        /*particlePanel.gameObject.SetActive(false);*/

        closeAllParticles();
    }

    public void openMainMenu()
    {
        if (!_optionsOpen)
        {
            mainMenuPanel.DOAnchorPos(Vector2.zero, _slidesTime);
            gamePlayPanel.gameObject.SetActive(false);
            _optionsOpen = true;

            if (!_inventoryOpen)
            {
                fadeIn();
            }
        }
        else if (_optionsOpen)
        {
            mainMenuPanel.DOAnchorPos(new Vector2(-700f, 0f), _slidesTime);
            gamePlayPanel.gameObject.SetActive(true);
            _optionsOpen = false;

            if (!_inventoryOpen)
            {
                fadeOut();
                checkEquipedItemUI();
            }
        }
    }

    public void openInventory()
    {
        if (!_inventoryOpen)
        {
            gameplayElements.gameObject.SetActive(false);
            inventoryPanel.DOSizeDelta(new Vector2(500f, _inventoryHeightSize), _slidesTime, true);
            fadeIn();
            _inventoryOpen = true;
        }
        else if (_inventoryOpen)
        {

            inventoryPanel.DOSizeDelta(new Vector2(0f, _inventoryHeightSize), _slidesTime);
            fadeOut();
            _inventoryOpen = false;

            checkEquipedItemUI();
        }

        if (newItemSignPanel.gameObject.activeInHierarchy)
        {
            newItemSignPanel.gameObject.SetActive(false);
        }
    }

    public void interactivElemsControlEnter()
    {
        gameplayElements.SetActive(false);
    }

    public void interactivElemsControlExit()
    {
        if (_inventoryOpen || _optionsOpen)
        {
            gameplayElements.SetActive(false);
            return;
        }

        checkEquipedItemUI();
    }

    private void checkEquipedItemUI()
    {
        _equpedItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_equpedItem)
        {
            gameplayElements.SetActive(true);
        }
    }

    private void fadeIn()
    {
        fadeImg.DOFade(0.6f, 1f);
        fadeImg.raycastTarget = true;
    }

    private void fadeOut()
    {
        fadeImg.DOFade(0f, 1f);
        fadeImg.raycastTarget = false;
    }

    public void equipUiEffect()
    {
        particles[_particleCount++].SetActive(true);
        /*particlePanel.gameObject.SetActive(true);*/
    }

    private void closeAllParticles()
    {
        foreach (GameObject particle in particles)
        {
            particle.SetActive(false);
        }
    }
}
