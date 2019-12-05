using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour,IPointerClickHandler
{
    public MainMenu menu;
    public Image lockLevelImg;
    public string loadingLevelName;

    private bool _lockerStats;
    void Awake()
    {
        lockLevel();
    }

    public void OnPointerClick(PointerEventData eventData)
    {      
        if (!_lockerStats)
        {
            menu.LoadingScene(loadingLevelName);
        }
    }

    public void unlocklevel()
    {
        _lockerStats = false;
        lockLevelImg.enabled = false;
    }

    public void lockLevel()
    {
        _lockerStats = true;
        lockLevelImg.enabled = true;
    }
}
