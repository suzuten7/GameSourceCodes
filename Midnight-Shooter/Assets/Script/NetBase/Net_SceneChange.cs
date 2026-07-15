using System.Collections;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Net_SceneChange : NetworkSceneManagerDefault
{
    static public bool loads = false;
    [SerializeField] float waitTime = 3f;

    protected override IEnumerator LoadSceneCoroutine(SceneRef sceneRef,NetworkLoadSceneParameters sceneParams)
    {
        loads = true;
        UI_Fade.ui_Fade.ChangeScene(-1);
        yield return new WaitForSecondsRealtime(waitTime);
        yield return base.LoadSceneCoroutine(sceneRef, sceneParams);

    }
    protected override IEnumerator OnSceneLoaded(SceneRef sceneRef, Scene scene, NetworkLoadSceneParameters sceneParams)
    {
        loads = false;
        return base.OnSceneLoaded(sceneRef, scene, sceneParams);
    }
    
}
