using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MiniGamePanel : MonoBehaviour, IPointerClickHandler
{
    public RectTransform miniGame;
    public Image[] activeImages;
    public Image[] linkImages;

    private Color _normalColor = Color.white;
    private Color[] _colors = { Color.red, Color.blue, Color.green, Color.cyan, Color.yellow };
    private Color _currentColor;

    void Start()
    {
        miniGame.gameObject.SetActive(false);
        addColors();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        miniGame.gameObject.SetActive(true);
    }

    private void addColors()
    {
        int[] numbers = { 0, 1, 2, 3, 4 };
        numbers = ShuffleArray(numbers);

        for (int i = 0; i < activeImages.Length; i++)
        {
            activeImages[numbers[i]].color = _colors[i];
        }

        numbers = ShuffleArray(numbers);

        for (int i = 0; i < activeImages.Length; i++)
        {
            linkImages[numbers[i]].color = _colors[i];
        }
    }

    private int[] ShuffleArray(int[] numbers)
    {
        int[] newArray = numbers.Clone() as int[];
        for (int i = 0; i < newArray.Length; i++)
        {
            int tmp = newArray[i];
            int r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }

    public Image[] getArray()
    {
        return linkImages;
    }
}

