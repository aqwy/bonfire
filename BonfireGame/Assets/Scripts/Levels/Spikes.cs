using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private int _leftSpikesCount = 0;
    private int _rightSpikesCount = 0;

    public void leftSpikeBeveled()
    {
        _leftSpikesCount++;
        getLeftSpikeBeveled();
        Debug.Log("spkikov left srezno: " + _leftSpikesCount);
        gameObject.SetActive(false);
    }
    public void rightSpikeBeveled()
    {
        _rightSpikesCount++;
        getRightSpikeBeveled();
        Debug.Log("spkikov right srezno: " + _rightSpikesCount);
    }

    private void getLeftSpikeBeveled()
    {
        if (_leftSpikesCount == 3)
        {
            GameManager.Instance.addWinKey();
            
        }
    }

    private void getRightSpikeBeveled()
    {
        if (_rightSpikesCount == 3)
        {
            GameManager.Instance.addWinKey();
        }
    }
}
