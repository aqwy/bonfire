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
    public RectTransform fadePanel;
    public RectTransform newItemSignPanel;
    public GameObject gameplayElements;

    public Animator inventoryAnim;
    public Animator settingsUIAnim;
    public Animator fadeAnim;

    private bool _openInventoryUI = true;

    private void Start()
    {
        newItemSignPanel.gameObject.SetActive(false);
    }

    public void fadeInUI(bool state)
    {
        if (fadeAnim)
        {
            fadeAnim.SetBool("FadeIn", state);
        }
    }

    public void openInventory()
    {
        if (inventoryAnim)
        {
            _openInventoryUI = inventoryAnim.GetBool("OpenInventory");
            gameplayElements.gameObject.SetActive(_openInventoryUI);
            fadeInUI(!_openInventoryUI);

            inventoryAnim.SetBool("OpenInventory", !_openInventoryUI);          
        }

        if(newItemSignPanel.gameObject.activeInHierarchy)
        {
            newItemSignPanel.gameObject.SetActive(false);
        }
    }

    public void openMainMenu()
    {
        if (settingsUIAnim)
        {
            bool openMainMenuUI = settingsUIAnim.GetBool("OpenMainMenu");
            fadeInUI(!openMainMenuUI);
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


    private IEnumerator closeInventory()
    {
        openInventory();
        yield return new WaitForSeconds(0.2f);
    }
}
