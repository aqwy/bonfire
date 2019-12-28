using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PictureminigameController : MonoBehaviour
{
    public RectTransform minigamePanel;
    public PictureController pictureControll;
    public RectTransform[] botPanels;
    public RectTransform[] topPanels;
    public Sprite frontImg;

    private string[] _pictureNames = { "MONNA LISA", "krik", "mishkivlesu" };
    private char[] _lettersPicture;
    private int _pictureCount;
    void Start()
    {
        _pictureCount = 0;
        fillBotPanels();
    }

    public void fillBotPanels()
    {
        closeAllPicPanels();

        _lettersPicture = _pictureNames[_pictureCount].ToCharArray();
        _lettersPicture = RemoveDublicateLetters(_lettersPicture);
        _lettersPicture = ShuffleArray(_lettersPicture);

        int topPanelSize = _pictureNames[_pictureCount].Length;
        int botPanelSize = _lettersPicture.Length;

        for (int i = 0; i < botPanelSize; i++)
        {
            botPanels[i].gameObject.SetActive(true);
            botPanels[i].GetComponentsInChildren<Image>()[1].sprite = frontImg;
            TextMeshProUGUI panelTXt = botPanels[i].GetComponentInChildren<TextMeshProUGUI>();
            panelTXt.fontSize = 78;
            panelTXt.fontStyle = FontStyles.Bold;
            panelTXt.text = _lettersPicture[i].ToString();
        }

        for (int i = 0; i < topPanelSize; i++)
        {
            topPanels[i].gameObject.SetActive(true);
            topPanels[i].GetComponent<Image>().sprite = frontImg;
        }
    }

    private char[] ShuffleArray(char[] letters)
    {
        char[] newArray = letters.Clone() as char[];
        for (int i = 0; i < newArray.Length; i++)
        {
            char tmp = newArray[i];
            int r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }

    private char[] RemoveDublicateLetters(char[] letter)
    {
        char[] noDuplicates = letter.Clone() as char[];
        string clearName = string.Empty;

        char previousLetter = noDuplicates[0];
        for (int i = 1; i < noDuplicates.Length; i++)
        {
            if (noDuplicates[i] == previousLetter)
            {
                continue;
            }
            else
            {
                clearName += previousLetter;
                previousLetter = noDuplicates[i];
            }
        }

        noDuplicates = clearName.ToCharArray();
        return noDuplicates;
    }

    private void closeAllPicPanels()
    {
        for (int i = 0; i < botPanels.Length; i++)
        {
            botPanels[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < topPanels.Length; i++)
        {
            topPanels[i].gameObject.SetActive(false);
            /*topPanels[i].GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;*/
        }
    }

    public void GetPictureNameFromTop()
    {
        string topPickName = string.Empty;

        for (int i = 0; i < topPanels.Length; i++)
        {
            if (topPanels[i].GetComponentInChildren<TextMeshProUGUI>())
            {
                topPickName += topPanels[i].GetComponentInChildren<TextMeshProUGUI>().text;
            }
        }

        if (topPickName.Equals(_pictureNames[_pictureCount]))
        {
            pictureControll.nextPicture();
            _pictureCount++;
            minigamePanel.gameObject.SetActive(false);
            Debug.Log("complt popadanie");
        }
        else
        {
            Debug.Log("polnaya hueta " + "nada :" + _pictureNames[_pictureCount] + "a u nas " + topPickName);
        }
    }
}
