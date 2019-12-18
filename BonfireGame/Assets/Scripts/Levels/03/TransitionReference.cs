using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionReference : MonoBehaviour
{
    public minigameParent minigameRef;

    public void makeTransition()
    {
        minigameRef.openPaintPart();
    }
}
