using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HummerUpgrade : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SpriteRenderer interactFrameSprite;
    public Equippableitem newSuperHummer;
    public Transform particlePos;
    public GameObject poofParticle;

    private Equippableitem _currItem;
    private bool _upgraded = false;
    private void Start()
    {
        interactFrameSprite.enabled = false;
    }
    public void startHummUpgrade()
    {
        InventoryManager.Instance.inventory.addItem(newSuperHummer);
        UIManager.Instance.newItemSignPanel.gameObject.SetActive(true);
        GameManager.Instance.addWinKey();
        GameObject hammerPoof = Instantiate(poofParticle, particlePos.position, Quaternion.Euler(-75f, 0f, 0f));
        Destroy(hammerPoof, 2f);
        _upgraded = true;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_currItem)
        {
            if (_currItem.effecttype.ToString() == "activator")
            {
                var itemtoRemove = GameManager.Instance.activeItem;
                itemtoRemove.removeItem(_currItem);
                UIManager.Instance.equipUiEffect();
                startHummUpgrade();
                Destroy(gameObject);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_currItem)
        {
            if (_currItem.effecttype.ToString() == "activator")
            {
                interactFrameSprite.enabled = true;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        interactFrameSprite.enabled = false;
    }

    public bool statusUpgrae()
    {
        return _upgraded;
    }
}
