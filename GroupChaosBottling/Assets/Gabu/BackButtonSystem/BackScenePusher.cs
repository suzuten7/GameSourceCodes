using UnityEngine;
using static BackSceneStack;

public class BackScenePusher : MonoBehaviour
{
    [SerializeField]
    private InstructionsSpaceAnimation targetScene;
    public void Push()
    {
        if (targetScene == null)
        {
            Debug.LogWarning("開くシーンのインターフェイスが設定されていません");
            return;
        }
        targetScene.Play();
        backSceneStack.Push(targetScene);
    }
}
