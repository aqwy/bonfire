using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WallMover : MonoBehaviour
{
    public float startPosY;
    public float endPos;
    public float duration;

    private Vector3 _currentPos;
    private Tweener _tweener;
    void Start()
    {
        _currentPos = transform.position;
        startPosY = _currentPos.y;
        startMoving();
    }

    public void wallMoving(bool status)
    {
        if (status)
        {
            _tweener = transform.DOLocalMoveY(endPos, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
        else if (!status)
        {
            _tweener.Pause();
            _tweener = transform.DOLocalMoveY(startPosY, 2f);
            Debug.Log("false moving");
        }
    }

    public void startMoving()
    {
        _tweener.Pause();
        _tweener = transform.DOLocalMoveY(endPos, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        _tweener.Play();
    }

    public void stopMoving()
    {
        _tweener.Pause();
        _tweener = transform.DOLocalMoveY(startPosY, 2f);
        _tweener.Play();
    }

    public void resetWall()
    {
        this.transform.position = _currentPos;
    }
}
