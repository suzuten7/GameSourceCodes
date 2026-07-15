using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

/* 内容
 * ・UnityEditorのみ
 * ・開始時にエラーなどが発生しないように、読み込む前にシーンを切り替える
 * ・終了時に元のシーンへ戻す
 */

[InitializeOnLoad]
public class Debug_StartSceneChange : MonoBehaviour
{
    const bool Active = false;

    //開始シーン
    const string startScene_Path = "Assets/Scenes/Title.unity";
    //元シーン
    const string sessionScene_Path = "DEBUG_BEFORE_SCENE_PATH";

    /// <summary>
    /// Editor更新時に1回だけ生成確認
    /// </summary>
    static Debug_StartSceneChange() { EditorApplication.update += Initialize; }

    static void Initialize()
    {
        if (!Active) return;
        //1回だけ実行
        EditorApplication.update -= Initialize;

        //既に存在するなら終了
        if (FindAnyObjectByType<Debug_StartSceneChange>() != null) return;

        //GameObject生成
        GameObject obj = new("[Debug_StartSceneChange]");

        //非表示 + 保存しない
        obj.hideFlags = HideFlags.HideAndDontSave;

        //アタッチ
        obj.AddComponent<Debug_StartSceneChange>();

        //PlayModeイベント登録
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    /// <summary>
    /// 開始時 / 終了時の処理
    /// </summary>
    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!Active) return;
        #region 再生開始直前
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // 保存確認
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            { EditorApplication.isPlaying = false; return; }

            //開始シーン存在確認
            if (!System.IO.File.Exists(startScene_Path))
            {
                Debug.LogError($"開始シーンが存在しません\nPath : {startScene_Path}");

                EditorApplication.isPlaying = false;
                return;
            }

            //現在シーン保存
            string beforeScenePath =
                EditorSceneManager.GetActiveScene().path;

            //Sessionへ保存
            SessionState.SetString(sessionScene_Path, beforeScenePath);

            //同一シーンなら不要
            if (beforeScenePath == startScene_Path) return;

            //シーン切り替え
            EditorSceneManager.OpenScene(startScene_Path);
        }
        #endregion
        #region 再生終了後
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            //Session取得
            string beforeScenePath =
                SessionState.GetString(sessionScene_Path, "");

            Debug.Log(beforeScenePath);

            if (string.IsNullOrEmpty(beforeScenePath))
                return;

            EditorSceneManager.OpenScene(beforeScenePath);
        }
        #endregion
    }
}
#endif
