using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singltone<GameManager>
{
    void Start()
    {

    }

    void Update()
    {

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
}
