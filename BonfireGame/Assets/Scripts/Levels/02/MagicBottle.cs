using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


public class MagicBottle : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler,IPointerDownHandler
{
    public GameObject egg;
    public Equippableitem magicPotion;

    private bool _canUse = false;
    private void Start()
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!egg)
        {
            UIManager.Instance.gameplayElements.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!egg)
        {               
            StartCoroutine(moveToInventory());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_currItem)
        {
            UIManager.Instance.gameplayElements.SetActive(true);
        }
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
        GameManager.Instance.addWinKey();
        _canUse = true;
        InventoryManager.Instance.inventory.addItem(magicPotion);
        UIManager.Instance.newItemSignPanel.gameObject.SetActive(true);
        UIManager.Instance.gameplayElements.SetActive(true);

        Destroy(gameObject, 2f);        
    }

    public bool canBottleUse()
    {
        return _canUse;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!egg)
        {
            StartCoroutine(keepItemDelay(5f));
            UIManager.Instance.gameplayElements.SetActive(false);
        }
    }
}
