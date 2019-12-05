using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class minigameParent : MonoBehaviour
{
    public MiniGamePanel minigameMain;


    void Awake()
    {
        foreach (var list in FindObjectsOfType<ReorderableList>())
        {
            list.OnElementDropped.AddListener(tryToGetChild);
        }
    }
    void Start()
    {

    }
    public void tryToGetChild(ReorderableList.ReorderableListEventStruct droppedStruct)
    {
        int index = droppedStruct.ToIndex;
        Debug.Log("index: " + index);
        if (droppedStruct.DroppedObject.GetComponent<Image>().color.Equals(minigameMain.linkImages[index].color))
        {
            Debug.Log("Complite one!");
        }
    }
}
