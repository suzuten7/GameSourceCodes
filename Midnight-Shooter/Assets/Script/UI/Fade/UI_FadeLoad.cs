using UnityEngine;

/* 内容
 * ・読み取り用のScriptが無い時に使用(Buttonなど)
 */

public class UI_FadeLoad : MonoBehaviour
{
    /// <summary>
    /// シーンの読み取り
    /// </summary>
    /// <param name="sceneName"> 変更したいシーン名 </param>
    public void LoadFade(string sceneName)
    { UI_Fade.ui_Fade.ChangeScene(sceneName); }
}
