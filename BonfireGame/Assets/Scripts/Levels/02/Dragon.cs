using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Dragon : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public HummerUpgrade newSuperHummer;
    public SpriteRenderer interactHeadFrameSprite;
    public SpriteRenderer sleepIconSprite;
    public GameObject stunParticle;
    public Collider2D handColl;
    public Animator dragonAnim;
    public Transform stunPos;
    public Transform sleepPos;

    private bool _getHandHit = false;
    private Equippableitem _currItem;
    private Sequence _sleepingDragon;


    void Start()
    {
        sleepIconSprite.enabled = false;
        sleepIconSprite.transform.SetParent(sleepPos, true);
        interactHeadFrameSprite.enabled = false;
        handColl.enabled = false;
        sleepingDragon(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _currItem = GameManager.Instance.activeItem.getCurrentItem();
        if (_currItem)
        {
            if (newSuperHummer.statusUpgrae())
            {
                sleepingDragon(false);

                _getHandHit = true;
                handColl.enabled = true;
                if (dragonAnim)
                {
                    dragonAnim.SetTrigger("Stuned");
                    GameObject stun = Instantiate
                        (stunParticle, stunPos.position, Quaternion.Euler(new Vector3(-110f, 0f, 0f)));
                    stun.transform.DOMoveY(2f, 2f);

                    Destroy(stun, 2f);
                    StartCoroutine(sleepindDelay());
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

    public void sleepingDragon(bool sleep)
    {
        if (sleep)
        {
            sleepIconSprite.transform.localScale = Vector3.zero;
            sleepIconSprite.transform.position = sleepPos.position;
            sleepIconSprite.enabled = true;

            _sleepingDragon = makeDragonSleep();
        }
        else if (!sleep)
        {
            sleepIconSprite.transform.localScale = Vector3.zero;
            sleepIconSprite.transform.position = sleepPos.position;
            _sleepingDragon.Pause();
            sleepIconSprite.enabled = false;
        }
    }

    private Sequence makeDragonSleep()
    {
        float needSpriteSize = 0.17f;
        Sequence gonaSleep = DOTween.Sequence();

        needSpriteSize = Mathf.Round(needSpriteSize * 100.0f) / 100.0f;

        gonaSleep.Append(sleepIconSprite.transform.DOMove(new Vector2(-1f,0f), 2f))
            .Join(sleepIconSprite.transform.DOScale(needSpriteSize, 2f));
        gonaSleep.SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        return gonaSleep;
    }

    private IEnumerator sleepindDelay()
    {
        yield return new WaitForSeconds(2.3f);
        sleepingDragon(true);
    }
}
