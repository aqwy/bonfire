using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MiniGamePanel : MonoBehaviour
{
    public Image[] activeImages;
    public Image[] linkImages;
    public Image[] paintsColors;
    public Image[] wireTopColors;
    public Image[] wireBotColors;
    public Image currentPaintColor;

    private Color _normalColor = Color.white;
    private Color[] _colors = { Color.red, Color.blue, Color.green, Color.cyan, Color.yellow };
    private Color _currentColor;

    void Start()
    {
        addColors();
        currentPaintColor.color = _normalColor;
    }

    private void addColors()
    {
        for (int i = 0; i < paintsColors.Length; i++)
        {
            paintsColors[i].color = _colors[i];
        }

        int[] numbers = { 0, 1, 2, 3, 4 };
        numbers = ShuffleArray(numbers);

        for (int i = 0; i < activeImages.Length; i++)
        {
            activeImages[numbers[i]].color = _colors[i];
        }

        numbers = ShuffleArray(numbers);

        for (int i = 0; i < linkImages.Length; i++)
        {
            linkImages[numbers[i]].color = _colors[i];
        }

        numbers = ShuffleArray(numbers);

        for (int i = 0; i < wireTopColors.Length; i++)
        {
            wireTopColors[numbers[i]].color = _colors[i];
        }

        numbers = ShuffleArray(numbers);

        for (int i = 0; i < wireBotColors.Length; i++)
        {
            wireBotColors[numbers[i]].color = _colors[i];
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
}

