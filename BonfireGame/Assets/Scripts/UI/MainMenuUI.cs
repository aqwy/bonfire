using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject levelsMenu;

    void Start()
    {
        optionsMenu.SetActive(false);
        levelsMenu.SetActive(false);
    }

    public void openOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }
    public void openLevelsLoadingMenu()
    {
        levelsMenu.SetActive(true);
    }
    public void closeOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }
    public void closeLevelsLoadingMenu()
    {
        levelsMenu.SetActive(false);
    }
}
