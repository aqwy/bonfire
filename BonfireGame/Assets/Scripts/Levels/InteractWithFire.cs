using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(SpriteRenderer))]
public class InteractWithFire : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SpriteRenderer interactSprite;
    public SpriteRenderer winIteractSprite;
    public SpriteSwaper spriteSwaper;
    public Inventory inventory;

    private bool _interacting;
    private bool _complitingBonefire;
    private Equippableitem _currItem;
    void Start()
    {
        interactSprite.gameObject.SetActive(false);
        winIteractSprite.gameObject.SetActive(false);
        spriteSwaper = GetComponent<SpriteSwaper>();
        _interacting = false;
        _complitingBonefire = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        checkItemType();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_interacting)
        {
            _interacting = false;
            interactSprite.gameObject.SetActive(false);
            winIteractSprite.gameObject.SetActive(false);
        }
    }

    private void checkItemType()
    {
        _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_currItem)
        {
            if (_currItem.effecttype.ToString() == "connect")
            {
                _interacting = true;
                interactSprite.gameObject.SetActive(true);
            }

            if (_currItem.effecttype.ToString() == "activator" && GameManager.Instance.getIteractIconStatus())
            {
                _interacting = true;
                winIteractSprite.gameObject.SetActive(true);
            }
        }
        else
        {
            _interacting = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("point click");
        if (_interacting && _currItem.effecttype.ToString() != "activator")
        {
            var itemtoRemove = GameManager.Instance.activeItem;
            itemtoRemove.removeItem(_currItem);
            spriteSwaper.swapSprite();
            _complitingBonefire = true;
        }

        if (_complitingBonefire)
        {
            var item = GameManager.Instance.activeItem.getCurrentItem();
            if (item)
            {
                if (item.effecttype.ToString() == "activator")
                {
                    GameManager.Instance.addWinKey();
                    GameManager.Instance.getWinReward();
                    winIteractSprite.gameObject.SetActive(false);
                    this.enabled = false;
                }
            }
        }
    }
}
