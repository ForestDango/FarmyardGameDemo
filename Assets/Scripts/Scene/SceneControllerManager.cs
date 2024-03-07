using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SceneControllerManager : Singleton<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup fadeCanvasGroup = null;
    [SerializeField] private Image fadeImage = null;
    public SceneName startingSceneName;

    private IEnumerator Start()
    {
        fadeImage.color = new Color(0f, 0f, 0f, 1f);
        fadeCanvasGroup.alpha = 1f;

        yield return StartCoroutine(LoadSceneAndSetActiveCoroutine(startingSceneName.ToString()));

        EventHandler.CallAfterSceneLoadEvent();

        SaveLoadManager.Instance.RestoreCurrentSceneData();

        StartCoroutine(FadeCoroutine(0f));
    }

    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenesCoroutine(sceneName, spawnPosition));
        }
    }

    private IEnumerator FadeAndSwitchScenesCoroutine(string sceneName,Vector3 spawnPosition)
    {
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        yield return StartCoroutine(FadeCoroutine(1f));

        SaveLoadManager.Instance.StoreCurrentSceneData();

        PlayerController.Instance.gameObject.transform.position = spawnPosition;

        EventHandler.CallBeforeSceneUnloadEvent();

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        yield return StartCoroutine(LoadSceneAndSetActiveCoroutine(sceneName));

        EventHandler.CallAfterSceneLoadEvent();

        SaveLoadManager.Instance.RestoreCurrentSceneData();

        yield return StartCoroutine(FadeCoroutine(0f));

        EventHandler.CallAfterSceneLoadFadeInEvent();
    }

    private IEnumerator FadeCoroutine(float finalAlpha)
    {
        isFading = true;

        fadeCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(fadeCanvasGroup.alpha - finalAlpha) / fadeDuration;

        while (!Mathf.Approximately(fadeCanvasGroup.alpha, finalAlpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            yield return null;
        }


        isFading = false;

        fadeCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator LoadSceneAndSetActiveCoroutine(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        SceneManager.SetActiveScene(newlyLoadedScene);
    }
}
