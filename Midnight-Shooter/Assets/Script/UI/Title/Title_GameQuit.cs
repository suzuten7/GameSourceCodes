using UnityEngine;

/* 内容
 * ・ゲーム終了処理
 */

public class Title_GameQuit : MonoBehaviour
{
    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
