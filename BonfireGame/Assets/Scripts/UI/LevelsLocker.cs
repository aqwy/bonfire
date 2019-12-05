using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelsLocker : MonoBehaviour
{
    [SerializeField] private RectTransform[] levelPanels;
    public string levelStringKey = "levelReached";

    private int _currentUnlockLevels = 1;

    void Start()
    {
        /*closeAllLevels();*/
        int levelReached = PlayerPrefs.GetInt(levelStringKey, _currentUnlockLevels);
        for (int i = 0; i < levelPanels.Length; i++)
        {
            if (i + 1 <= levelReached)
            {
                levelPanels[i].GetComponent<LevelButton>().unlocklevel();
            }
        }
    }

    public void closeAllLevels()
    {
        for (int i = 0; i < levelPanels.Length; i++)
        {
            levelPanels[i].GetComponent<LevelButton>().lockLevel();
        }
        _currentUnlockLevels = 1;
        PlayerPrefs.SetInt(levelStringKey, _currentUnlockLevels);
    }

    /*private void createLevels()
    {
        if (levelPrefab)
        {
            for (int i = 0; i < levelPanels.Length; i++)
            {              
                RectTransform lvl = Instantiate(levelPrefab);
                string prefabName = lvl.name;
                lvl.name = prefabName + (i).ToString();
                levelPanels[i] = lvl;
                Text levelTxt = levelPanels[i].GetComponentInChildren<Text>();
                levelTxt.text = (i + 1).ToString();

                if (i % 2 == 0)
                {
                    Image lockImg = levelPanels[i].GetComponentsInChildren<Image>()[1];
                    lockImg.enabled = false;
                }

                lvl.transform.SetParent(levelButtonsParent);
            }
        }
    }*/
}
