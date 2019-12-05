using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class DragonHand : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SpriteRenderer interactHandFrameSprite;
    public Animator atackAnim;
    public GameObject dragonFireParticle;
    public Transform dragFirePos;
    public Dragon dragonMain;

    void Start()
    {
        interactHandFrameSprite.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Collider2D>().isActiveAndEnabled)
        {
            dragonMain.sleepingDragon(false);
            GameManager.Instance.addWinKey();
            if (GameManager.Instance.winChecking())
            {
                if (atackAnim)
                {
                    atackAnim.SetTrigger("Burned");
                    if (dragonFireParticle)
                    {
                        GameObject fire = Instantiate
                            (dragonFireParticle, dragFirePos.position, dragonFireParticle.transform.rotation);
                        fire.transform.SetParent(dragFirePos);

                        Destroy(fire, 4f);
                    }
                }
                GameManager.Instance.getWinReward();
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

        if (gameObject.GetComponent<Collider2D>().isActiveAndEnabled)
        {
            interactHandFrameSprite.gameObject.transform.position = gameObject.transform.position;
            interactHandFrameSprite.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.GetComponent<Collider2D>().isActiveAndEnabled)
        {
            interactHandFrameSprite.enabled = false;
        }
    }
}
