using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class minigameParent : MonoBehaviour
{
    public MiniGamePanel minigameMain;
    public ReorderableList engineReordable;
    public ReorderableList paintReordable;
    public RectTransform winFirstPartPanel;
    public RectTransform topPanel;
    public GameObject enginePartPanel;
    public GameObject paintsPartPanel;
    public GameObject boltsPanel;
    public GameObject colorPanel;
    public Image sightImg;
    public Rocket rocket;

    private int _currIndex;
    private bool _miniPartOneComplite;
    private Color _currCol;
    private Image[] _dropsColors;
    private Equippableitem _currItem;

    void Awake()
    {
        engineReordable.OnElementDropped.AddListener(tryToGetChild);
        paintReordable.OnElementGrabbed.AddListener(grabbToTrueSight);
    }
    void Start()
    {
        _dropsColors = new Image[minigameMain.wireBotColors.Length];
        _currIndex = 0;
        _miniPartOneComplite = false;
        winFirstPartPanel.gameObject.SetActive(false);
        enginePartPanel.SetActive(true);
        paintsPartPanel.SetActive(false);
        boltsPanel.SetActive(false);
        colorPanel.SetActive(false);
    }
    private void Update()
    {
        _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_currItem)
        {
            if (_currItem.effecttype.ToString() == "activator")
            {
                boltsPanel.SetActive(true);
            }
            else
            {
                boltsPanel.SetActive(false);
            }

            if (_currItem.effecttype.ToString() == "nothing")
            {
                colorPanel.SetActive(true);
            }
            else
            {
                colorPanel.SetActive(false);
            }
        }
    }
    public void tryToGetChild(ReorderableList.ReorderableListEventStruct droppedStruct)
    {
        int index = droppedStruct.ToIndex;
        if (!_miniPartOneComplite)
        {
            if (droppedStruct.DroppedObject.GetComponent<Image>().color.Equals(minigameMain.linkImages[index].color))
            {
                _currIndex = index;
                minigameMain.centrLinkPanel[index].enabled = true;
                droppedStruct.DroppedObject.GetComponent<ReorderableListElement>().IsGrabbable = false;

                if (minigameMain.checkFullLink())
                {
                    winFirstPartPanel.gameObject.SetActive(true);
                    _miniPartOneComplite = true;
                }
            }
        }
    }

    public void openPaintPart()
    {
        enginePartPanel.SetActive(false);
        paintsPartPanel.SetActive(true);
    }

    public void grabbToTrueSight(ReorderableList.ReorderableListEventStruct dragStruct)
    {
        if (_miniPartOneComplite)
        {
            sightImg.color = dragStruct.SourceObject.GetComponent<itemPartTwoMinigame>().getTrueColor();
        }
    }

    private bool minigameWinCheking()
    {
        Image[] botColors = minigameMain.wireBotColors;
        _dropsColors = topPanel.GetComponentsInChildren<Image>();
        for (int i = 0; i < botColors.Length; i++)
        {
            if (!botColors[i].color.Equals(_dropsColors[i].GetComponent<itemPartTwoMinigame>().getTrueColor())
                || !botColors[i].color.Equals(_dropsColors[i].color))
            {
                return false;
            }
        }
        return true;
    }

    public void getWinMinigame()
    {
        if (minigameWinCheking())
        {
            minigameMain.gameObject.SetActive(false);
            rocket.enabled = true;
        }
    }
}
