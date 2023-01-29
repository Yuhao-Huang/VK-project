using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : Singleton<TransitionManager>
{
    [Header("Fade")]
    [SerializeField] CanvasGroup fadeCanvasGroup;
    [SerializeField] float fadeDuration;
    bool isTransition;





    //转换场景
    public void Transition(string from, string to) => StartCoroutine(TransitionToScene(from, to));
    IEnumerator TransitionToScene(string from, string to)
    {
        //游戏刚开始
        if(from=="Persistent")
        {
            if (SceneManager.sceneCount == 1)
            {
                yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);
                EventCenter.Notify_Transition(from, to);
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
            yield break;
        }
        if (isTransition) yield break;
        isTransition = true;
        yield return Fade(1);
        yield return SceneManager.UnloadSceneAsync(from);
        yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
        EventCenter.Notify_Transition(from, to);
        yield return Fade(0);
        isTransition = false;
    }
    IEnumerator Fade(float targetAlpha)
    {
        fadeCanvasGroup.blocksRaycasts = true;
        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / fadeDuration;
        while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }
        fadeCanvasGroup.blocksRaycasts = false;
    }
}
