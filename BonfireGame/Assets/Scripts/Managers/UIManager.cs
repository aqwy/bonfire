using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : Singltone<UIManager>
{
    public RectTransform inventoryPanel;
    public RectTransform mainMenuPanel;
    public RectTransform gamePlayPanel;
    public GameObject gameplayElements;

    public Animator inventoryAnim;
    public Animator settingsUIAnim;

    private bool _openInventoryUI = true;


    public void openInventory()
    {
        if (inventoryAnim)
        {
            _openInventoryUI = inventoryAnim.GetBool("OpenInventory");
            inventoryAnim.SetBool("OpenInventory", !_openInventoryUI);
        }
    }

    public void openMainMenu()
    {
        if (settingsUIAnim)
        {
            bool openMainMenuUI = settingsUIAnim.GetBool("OpenMainMenu");
            gamePlayPanel.gameObject.SetActive(openMainMenuUI);

            if (!_openInventoryUI)
            {
                Debug.Log("need close inventory");
                StartCoroutine(closeInventory());
            }

            gameplayElements.gameObject.SetActive(openMainMenuUI);
            settingsUIAnim.SetBool("OpenMainMenu", !openMainMenuUI);
        }
    }

    IEnumerator closeInventory()
    {
        openInventory();
        yield return new WaitForSeconds(0.2f);
    }
}
