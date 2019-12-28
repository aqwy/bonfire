using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PictureController : MonoBehaviour
{
    public GameObject[] pictures;
    public float transitionTime;

    private int _count;
    void Start()
    {
        transitionTime = 2f;
        _count = 0;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            nextPicture();
        }
    }
    public void nextPicture()
    {
        pictures[_count].transform.DOLocalMoveX(10, transitionTime).SetLoops(0).SetEase(Ease.Linear).SetDelay(0.5f);
        Destroy(pictures[_count].gameObject, transitionTime + 0.5f);
        _count++;
        if (pictures.Length > _count)
        {
            pictures[_count].transform.DOLocalMoveX(0, transitionTime).SetDelay(transitionTime + 1f);
        }
    }
}
