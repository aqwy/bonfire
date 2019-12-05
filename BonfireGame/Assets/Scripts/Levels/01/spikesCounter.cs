using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spikesCounter : MonoBehaviour
{
    private void addOne()
    {
        Spikes spike = GetComponentInParent<Spikes>();
        spike.leftSpikeBeveled();
    }
}
