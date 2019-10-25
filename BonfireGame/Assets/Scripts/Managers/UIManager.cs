using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singltone<UIManager>
{
    public RectTransform inventoryPanel;
    public RectTransform mainMenuPanel;
    public RectTransform gamePlayPanel;
    public GameObject gameplayElements;

    public void openInventory()
    {
        Animator animator = inventoryPanel.GetComponent<Animator>();
        if (animator)
        {
            bool openInventoryUI = animator.GetBool("OpenInventory");
            animator.SetBool("OpenInventory", !openInventoryUI);
        }
    }

    public void openMainMenu()
    {
        Animator animator = mainMenuPanel.GetComponent<Animator>();
        if (animator)
        {
            bool openMainMenuUI = animator.GetBool("OpenMainMenu");
            gamePlayPanel.gameObject.SetActive(openMainMenuUI);
            gameplayElements.gameObject.SetActive(openMainMenuUI);
            animator.SetBool("OpenMainMenu", !openMainMenuUI);
        }
    }
}
