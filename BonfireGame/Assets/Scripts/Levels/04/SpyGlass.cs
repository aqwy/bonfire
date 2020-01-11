using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpyGlass : MonoBehaviour, IPointerClickHandler
{
    public RectTransform pictureMinigamePanel;
    public Gun gun;
    public WallController wallController;
    public void OnPointerClick(PointerEventData eventData)
    {
        pictureMinigamePanel.gameObject.SetActive(true);
        gun.gunIsNotReady();
        wallController.gameObject.SetActive(false);
        this.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
