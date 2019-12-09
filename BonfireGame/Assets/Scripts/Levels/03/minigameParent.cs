using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class minigameParent : MonoBehaviour
{
    public MiniGamePanel minigameMain;
    public Image[] centrLinkPanel;
    public RectTransform winFirstPartPanel;
    public GameObject enginePartPanel;
    public GameObject paintsPartPanel;
    public Image sightImg;

    private int _currIndex;
    private bool _miniPartOneComplite;
    private Color _currCol;

    void Awake()
    {
        foreach (var list in FindObjectsOfType<ReorderableList>())
        {
            list.OnElementDropped.AddListener(tryToGetChild);
            list.OnElementGrabbed.AddListener(grabbToTrueSight);
        }
    }
    void Start()
    {
        _currIndex = 0;
        _miniPartOneComplite = false;
        winFirstPartPanel.gameObject.SetActive(false);
        enginePartPanel.SetActive(true);
        paintsPartPanel.SetActive(false);
        closeLinks();
    }
    public void tryToGetChild(ReorderableList.ReorderableListEventStruct droppedStruct)
    {
        int index = droppedStruct.ToIndex;
        Debug.Log("drop index: " + index);
        if (!_miniPartOneComplite)
        {
            if (droppedStruct.DroppedObject.GetComponent<Image>().color.Equals(minigameMain.linkImages[index].color))
            {
                _currIndex = index;
                centrLinkPanel[index].enabled = true;
                droppedStruct.DroppedObject.GetComponent<ReorderableListElement>().IsGrabbable = false;
                Debug.Log("Complite one!");

                if (checkFullLink())
                {
                    winFirstPartPanel.gameObject.SetActive(true);
                    _miniPartOneComplite = true;
                }
            }
        }
    }

    private void closeLinks()
    {
        for (int i = 0; i < centrLinkPanel.Length; i++)
        {
            centrLinkPanel[i].enabled = false;
        }
    }

    private bool checkFullLink()
    {
        for (int i = 0; i < centrLinkPanel.Length; i++)
        {
            if (!centrLinkPanel[i].enabled)
            {
                return false;
            }
        }
        return true;
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
}
