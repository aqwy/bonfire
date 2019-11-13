using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


public class MagicBottle : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public GameObject egg;
    public GameObject interactiveElements;
    public Equippableitem magicPotion;

    private void Start()
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!egg)
        {
            Debug.Log("pointer enter");
            interactiveElements.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!egg)
        {
            StartCoroutine(keepItemDelay(5f));
            Debug.Log("clicker magic position");

            StartCoroutine(moveToInventory());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("clicker magic exit");
        interactiveElements.SetActive(true);
    }

    private IEnumerator keepItemDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    private IEnumerator moveToInventory()
    {
        transform.DOMove(new Vector2(7f, 3.7f), 1.8f);
        transform.DOScale(0f, 1.8f);
        transform.DORotate
            (new Vector3(0f, 0f, -45f), 0.4f).SetLoops(5, LoopType.Incremental).SetEase(Ease.Linear);

        yield return new WaitForSeconds(2f);
        InventoryManager.Instance.inventory.addItem(magicPotion);
        UIManager.Instance.newItemSignPanel.gameObject.SetActive(true);

        Destroy(gameObject,2f);
        interactiveElements.SetActive(true);
    }
}
