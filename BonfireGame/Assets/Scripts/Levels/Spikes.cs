using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private BigBushCount bushCount;
    private int _leftSpikesCount;
    private int _rightSpikesCount;
    private int _childCount;
    
    private void Start()
    {
        _leftSpikesCount = 0;
        _rightSpikesCount = 0;
        _childCount = GetComponentsInChildren<spikesCounter>().Length;
    }

    public void leftSpikeBeveled()
    {
        _leftSpikesCount++;
        getLeftSpikeBeveled();
        Debug.Log("spkikov left srezno: " + _leftSpikesCount + " iz: " + _childCount);
    }
    public void rightSpikeBeveled()
    {
        _rightSpikesCount++;
        getRightSpikeBeveled();
        Debug.Log("spkikov right srezno: " + _rightSpikesCount + " iz: " + _childCount);
    }

    private void getLeftSpikeBeveled()
    {
        if (_leftSpikesCount == _childCount)
        {
            GameManager.Instance.addWinKey();
            bushCount.newItemCount++;
            bushCount.checkNewItem();
            Debug.Log("add win key for left");
        }
    }

    private void getRightSpikeBeveled()
    {
        if (_rightSpikesCount == _childCount)
        {
            GameManager.Instance.addWinKey();
            bushCount.newItemCount++;
            bushCount.checkNewItem();
            Debug.Log("add win key for right");
        }
    }   
}
