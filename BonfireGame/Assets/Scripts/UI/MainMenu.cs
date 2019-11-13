﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string sceneToLoad;
    public RectTransform loadingOverlay;

    private AsyncOperation sceneLoadingOperation;

    public void Start()
    {
        loadingOverlay.gameObject.SetActive(false);
        sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        sceneLoadingOperation.allowSceneActivation = false;
    }

    public void LoadScene()
    {
        loadingOverlay.gameObject.SetActive(true);
        sceneLoadingOperation.allowSceneActivation = true;
    }
}

