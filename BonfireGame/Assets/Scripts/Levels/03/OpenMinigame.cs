using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenMinigame : MonoBehaviour, IPointerClickHandler
{
    public RectTransform miniGame;
    void Start()
    {
        miniGame.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        miniGame.gameObject.SetActive(true);
    }
}
