﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singltone<GameManager>
{
    public GameObject[] levelInteractiveElements;
    public EquipmentPanel activeItem;
    public int winKeysCount;
    public GameObject winRewardMenu;

    private bool[] _winConditionsKeys;
    private int _winConCounter;


    void Start()
    {
        setupStarting();
    }

    void Update()
    {
        
    }
    private void setupStarting()
    {
        _winConditionsKeys = new bool[winKeysCount];
        closeInteractive();
        resetWinCon();
        _winConCounter = 0;

        if (winRewardMenu)
        {
            winRewardMenu.SetActive(false);
        }
    }
    public void closeInteractive()
    {
        foreach (GameObject element in levelInteractiveElements)
        {
            element.gameObject.SetActive(false);
        }
    }
    private int GetCurrentLevel()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }
    private void LoadLevel(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }

    public void restartLevel()
    {
        LoadLevel(GetCurrentLevel());
    }

    public void setInteractive()
    {
        Equippableitem item = activeItem.getCurrentItem();
        if (!item)
        {
            return;
        }
        else
        {
            bool findIntractive = false;
            for (int i = 0; i < levelInteractiveElements.Length; i++)
            {
                if (item.effecttype.ToString() == levelInteractiveElements[i].tag)
                {
                    closeInteractive();
                    levelInteractiveElements[i].SetActive(true);
                    findIntractive = true;
                    break;
                }
            }
            if (!findIntractive)
            {
                closeInteractive();
            }
        }
    }

    private void resetWinCon()
    {
        for (int i = 0; i < _winConditionsKeys.Length; i++)
        {
            _winConditionsKeys[i] = false;
        }
    }
    public void addWinKey()
    {
        _winConditionsKeys[_winConCounter++] = true;
    }
    public bool winChecking()
    {
        for (int i = 0; i < _winConditionsKeys.Length; i++)
        {
            if (!_winConditionsKeys[i])
                return false;
        }
        return true;
    }

    public void getWinReward()
    {
        if (winChecking())
        {
            winRewardMenu.SetActive(true);
        }
    }
    public bool getIteractIconStatus()
    {
        for (int i = 0; i < _winConditionsKeys.Length - 1; i++)
        {
            if(_winConditionsKeys[i] == false)
            {
                return false;
            }
        }
        return true;
    }
}
