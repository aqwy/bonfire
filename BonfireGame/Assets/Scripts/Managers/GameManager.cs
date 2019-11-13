using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singltone<GameManager>
{
    public GameObject[] levelInteractiveElements;
    public EquipmentPanel activeItem;
    public int winKeysCount;
    public GameObject winRewardMenu;
    public CinemachineVirtualCamera cmCam;
    public GameObject firePrefab;
    public Transform firePrefabPosition;


    private bool[] _winConditionsKeys;
    private int _winConCounter;


    void Start()
    {
        setupStarting();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (cmCam.LookAt != null)
            {
                StartCoroutine(cameraZoom());

            }
            else
            {
                Debug.Log("dont have follow");
            }
        }
    }
    private void setupStarting()
    {
        _winConditionsKeys = new bool[winKeysCount];
        closeInteractive();
        resetWinCon();
        _winConCounter = 0;

        if (winRewardMenu)
        {
            winRewardMenu.SetActive(false);
        }
    }
    public void closeInteractive()
    {
        foreach (GameObject element in levelInteractiveElements)
        {
            element.gameObject.SetActive(false);
        }
    }
    private int GetCurrentLevel()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }
    private void LoadLevel(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }

    public void restartLevel()
    {
        LoadLevel(GetCurrentLevel());
    }

    public void setInteractive()
    {
        Equippableitem item = activeItem.getCurrentItem();
        if (!item)
        {
            return;
        }
        else
        {
            bool findIntractive = false;
            for (int i = 0; i < levelInteractiveElements.Length; i++)
            {
                if (item.effecttype.ToString() == levelInteractiveElements[i].tag)
                {
                    closeInteractive();
                    levelInteractiveElements[i].SetActive(true);
                    findIntractive = true;
                    break;
                }
            }
            if (!findIntractive)
            {
                closeInteractive();
            }
        }
    }

    private void resetWinCon()
    {
        for (int i = 0; i < _winConditionsKeys.Length; i++)
        {
            _winConditionsKeys[i] = false;
        }
    }
    public void addWinKey()
    {
        _winConditionsKeys[_winConCounter++] = true;
    }
    public bool winChecking()
    {
        for (int i = 0; i < _winConditionsKeys.Length; i++)
        {
            if (!_winConditionsKeys[i])
                return false;
        }
        return true;
    }

    public void getWinReward()
    {
        if (winChecking())
        {
            StartCoroutine(cameraZoom());
        }
    }
    public bool getIteractIconStatus()
    {
        for (int i = 0; i < _winConditionsKeys.Length - 1; i++)
        {
            if (_winConditionsKeys[i] == false)
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator cameraZoom()
    {
        float currOrtSize = cmCam.m_Lens.OrthographicSize;
        float needOrtSzie = 3.5f;
        float velocity = 0.2f;
        float smoothTime = 1f;
        bool active = true;
        UIManager.Instance.gamePlayPanel.gameObject.SetActive(false);
        UIManager.Instance.gameplayElements.gameObject.SetActive(false);

        while (active)
        {
            currOrtSize = Mathf.Round(cmCam.m_Lens.OrthographicSize * 100.0f) / 100.0f;
            if (currOrtSize == needOrtSzie)
                active = false;

            cmCam.m_Lens.OrthographicSize =
                Mathf.SmoothDamp(cmCam.m_Lens.OrthographicSize, needOrtSzie, ref velocity, smoothTime);
            Debug.Log("time " + Time.time);
            yield return null;
        }

        GameObject fire = Instantiate(firePrefab, firePrefabPosition.position, firePrefab.transform.rotation);

        yield return new WaitForSeconds(3f);
        winRewardMenu.SetActive(true);

        if (winRewardMenu.activeInHierarchy)
        {
            WinReward winReward = winRewardMenu.GetComponent<WinReward>();
            winReward.playStarFest();
        }
    }

}
