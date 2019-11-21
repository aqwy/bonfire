using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Dragon : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public HummerUpgrade newSuperHummer;
    public SpriteRenderer interactHeadFrameSprite;
    public GameObject stunParticle;
    public Collider2D handColl;
    public Animator dragonAnim;
    public Transform stunPos;

    private bool _getHandHit = false;
    private Equippableitem _currItem;

    void Start()
    {
        interactHeadFrameSprite.enabled = false;
        handColl.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_currItem)
        {
            if (newSuperHummer.statusUpgrae())
            {
                _getHandHit = true;
                handColl.enabled = true;
                if (dragonAnim)
                {
                    dragonAnim.SetTrigger("Stuned");
                    GameObject stun = Instantiate
                        (stunParticle, stunPos.position, Quaternion.Euler(new Vector3(-110f, 0f, 0f)));
                    stun.transform.DOMoveY(2f, 2f);
                    Destroy(stun, 2f);
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.winChecking())
        {
            this.gameObject.GetComponent<Collider2D>().enabled = false;
            return;
        }

        _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_currItem)
        {
            if (newSuperHummer.statusUpgrae())
            {
                interactHeadFrameSprite.gameObject.transform.position = gameObject.transform.position;
                interactHeadFrameSprite.enabled = true;
            }
        }
        Debug.Log("dragon collider enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        interactHeadFrameSprite.enabled = false;
        Debug.Log("dragon collider exit");
    }
}
