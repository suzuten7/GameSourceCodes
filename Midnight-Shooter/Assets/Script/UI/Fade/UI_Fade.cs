using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* 内容
 * ・このオブジェクトをDon'tDestroy化
 * ・フェードの処理
 * ・シーン遷移
 */

#region 変数倉庫1
/// <summary>
/// BGMのセット
/// </summary>
[System.Serializable]
public class FadeSet_BGM
{
    [Tooltip("使う各シーン名")] public string sceneName;
    [Tooltip("流すBGM")] public AudioClip set_BGM;

    [Tooltip("音量"), Range(0, 1)] public float volume = 1f;
    [Tooltip("ピッチ"), Range(-3, 3)] public float pitch = 1f;
}

/// <summary>
/// SEのセット
/// </summary>
[System.Serializable]
public class FadeSet_SE
{
    [Tooltip("流すSE")] public AudioClip set_SE;

    [Tooltip("音量"), Range(0, 1)] public float volume = 1f;
    [Tooltip("ピッチ"), Range(-3, 3)] public float pitch = 1f;
}

/// <summary>
/// フェードの種類
/// </summary>
enum FadeType
{
    scroll,
    rotation,
    fade,
}

/// <summary>
/// スクロールする向き
/// </summary>
enum ScrollDirection
{
    Up,
    Down,
    Left,
    Right
}
#endregion

public class UI_Fade : MonoBehaviour
{
    #region 変数倉庫2
    #region メインオプション
    [Header("◆メインオプション")]
    [SerializeField] Image fade_Image;

    [SerializeField, Tooltip("始まった時にフェードを入れるか")]
    bool fade_StartFlag = true;
    bool fadeIn_Flag = false;

    [SerializeField, Tooltip("フェードの種類")]
    FadeType fadeType;

    string changeSceneName;
    int changeSceneID;
    [SerializeField, Tooltip("各シーンで流す音")] List<FadeSet_BGM> fs_BGM;
    [SerializeField, Tooltip("流す音(フェードイン)")] FadeSet_SE fs_SE;
    #endregion
    #region サブオプション
    [Header("◆サブオプション")]
    [Header("-共通-")]
    [SerializeField, Tooltip("[ALL]\nフェード時間")]
    float setfade_Time = 1f;
    [Header("-スクロール-")]
    [SerializeField, Tooltip("[Scroll]\n移動量")]
    float scroll_MoveValue = 2500f;
    [SerializeField, Tooltip("[Scroll]\n移動方向")]
    ScrollDirection scrollDirection;
    [Header("-回転-")]
    [SerializeField, Tooltip("[Rotation]\n回転回数")]
    int rotation_RotationValue = 1;
    [SerializeField, Tooltip("[Rotation]\nスケール拡大乗数")]
    float scaleExp = 2f;
    [SerializeField, Tooltip("[Rotation]\n右回転")]
    bool right_Rotation = true;
    //-フェード-
    RectTransform fadeRT;
    Color fadeColorA;
    bool now_FadeAnime = false;
    bool now_Play = false;
    #endregion
    #region その他
    static public UI_Fade ui_Fade;
    #endregion
    #endregion

    //重複防止処理
    void Awake()
    {
        if (ui_Fade != null)
        {
            Destroy(gameObject);
            return;
        }
        //初回のみ
        ui_Fade = this;
        DontDestroyOnLoad(gameObject);

        fade_Image.raycastTarget = false;
        fadeIn_Flag = !fade_StartFlag;
        FadeReset();

        if (fade_StartFlag)
        {
            if (now_Play) return;
            now_Play = true;
            StartCoroutine(SceneChangeProcess());
        }
    }

    //void Start()
    //{
    //    //BGMのセット
    //    for (int i = 0; i < fs_BGM.Count; i++)
    //    {
    //        if (fs_BGM[i].sceneName == SceneManager.GetActiveScene().name)
    //        { Sound_Library.Sound_BGMPlay(fs_BGM[i].set_BGM, fs_BGM[i].volume, fs_BGM[i].pitch); }
    //    }
    //}

    /// <summary>
    /// シーンの変更処理
    /// </summary>
    /// <param name="changeSceneName"> 変更したいシーン名 </param>
    public void ChangeScene(string changeSceneName)
    {
        if (now_Play) return;
        now_Play = true;

        this.changeSceneName = changeSceneName;
        changeSceneID = -1;

        StartCoroutine(SceneChangeProcess());
    }

    /// <summary>
    /// シーンの変更処理
    /// </summary>
    /// <param name="changeSceneID"> 変更したいシーンID </param>
    public void ChangeScene(int changeSceneID)
    {
        if (now_Play) return;
        now_Play = true;

        this.changeSceneID = changeSceneID;
        changeSceneName = "";

        StartCoroutine(SceneChangeProcess());
    }

    #region フェードの処理
    /// <summary>
    /// フェードの処理
    /// </summary>
    IEnumerator SceneChangeProcess()
    {
        //Sound_Library.Sound_SEPlay(fs_SE.set_SE, fs_SE.volume, fs_SE.pitch);
        fade_Image.gameObject.SetActive(true);

        //初めのフェード時
        if (fade_StartFlag)
        {
            fadeIn_Flag = false;
            yield return StartCoroutine(FadeSet());
            fade_StartFlag = false;
            fade_Image.raycastTarget = false;
            now_FadeAnime = false;
            now_Play = false;
            yield break;
        }

        //シーンの変更
        AsyncOperation loadScene = null;
        if (changeSceneName != "")loadScene = SceneManager.LoadSceneAsync(changeSceneName);
        else if (changeSceneID >= 0) loadScene = SceneManager.LoadSceneAsync(changeSceneID);
        if (loadScene != null) loadScene.allowSceneActivation = false;

        fade_Image.raycastTarget = true;
        fadeIn_Flag = true;
        yield return StartCoroutine(FadeSet());
        float wtime = 0;

        //待機
        while (true)
        {
            wtime += Time.unscaledDeltaTime;
            //Debug.Log("ロード待機中..." + wtime.ToString("F2"));
            if (!Net_SceneChange.loads) break;
            if (loadScene != null && (wtime < 1.0f || !loadScene.isDone)) break;
            yield return null;
        }

        //読み込みを待ってからフェードイン / アウト
        if (loadScene != null)loadScene.allowSceneActivation = true;


        ////BGMのセット
        //for (int i = 0; i < fs_BGM.Count; i++)
        //{
        //    if (fs_BGM[i].sceneName == SceneManager.GetActiveScene().name)
        //    { Sound_Library.Sound_BGMPlay(fs_BGM[i].set_BGM, fs_BGM[i].volume, fs_BGM[i].pitch); }
        //}
        fadeIn_Flag = false;
        yield return StartCoroutine(FadeSet());
        fade_Image.raycastTarget = false;

        now_FadeAnime = false;
        now_Play = false;
    }

    /// <summary>
    /// フェードの選択
    /// </summary>
    IEnumerator FadeSet()
    {
        FadeReset();

        switch (fadeType)
        {
            //スクロール
            case FadeType.scroll:
                {
                    yield return StartCoroutine(ScrollAnime());
                    break;
                }

            //回転
            case FadeType.rotation:
                {
                    yield return StartCoroutine(RotationAnime());
                    break;
                }

            //フェード
            case FadeType.fade:
                {
                    yield return StartCoroutine(FadeAnime());
                    break;
                }
        }
    }

    /// <summary>
    /// フェードの初期化
    /// </summary>
    void FadeReset()
    {
        fadeRT = fade_Image.rectTransform;

        //スクロールの初期化
        if (fadeType == FadeType.scroll)
        {
            switch (scrollDirection)
            {
                case ScrollDirection.Up:
                    fadeRT.anchoredPosition = new Vector2(0f, fadeIn_Flag ? -scroll_MoveValue : 0f);
                    break;
                case ScrollDirection.Down:
                    fadeRT.anchoredPosition = new Vector2(0f, fadeIn_Flag ? scroll_MoveValue : 0f);
                    break;
                case ScrollDirection.Left:
                    fadeRT.anchoredPosition = new Vector2(fadeIn_Flag ? scroll_MoveValue : 0f, 0f);
                    break;
                case ScrollDirection.Right:
                    fadeRT.anchoredPosition = new Vector2(fadeIn_Flag ? -scroll_MoveValue : 0f, 0f);
                    break;
            }
        }
        else
        { fadeRT.anchoredPosition = new Vector2(0f, 0f); }

        //回転とスケールの初期化
        fadeRT.localRotation = Quaternion.Euler(0f, 0f, fadeRT.localEulerAngles.z);
        if (fadeType == FadeType.rotation) { fadeRT.localScale = Vector3.one * (fadeIn_Flag ? 0f : 1f); }
        else { fadeRT.localScale = Vector3.one; }

        //フェードの初期化
        fadeColorA = fade_Image.color;
        if (fadeType == FadeType.fade) { fadeColorA.a = fadeIn_Flag ? 0f : 1f; }
        else { fadeColorA.a = 1f; }
        fade_Image.color = fadeColorA;
    }

    /// <summary>
    /// スクロール
    /// </summary>
    IEnumerator ScrollAnime()
    {
        now_FadeAnime = true;

        fadeRT = fade_Image.rectTransform;
        Vector2 startPos = fadeRT.anchoredPosition;
        Vector2 endPos = startPos;

        //方向を適応
        switch (scrollDirection)
        {
            case ScrollDirection.Up:
                endPos.y += scroll_MoveValue;
                break;
            case ScrollDirection.Down:
                endPos.y -= scroll_MoveValue;
                break;
            case ScrollDirection.Left:
                endPos.x -= scroll_MoveValue;
                break;
            case ScrollDirection.Right:
                endPos.x += scroll_MoveValue;
                break;
        }

        float timer = 0f;
        while (timer < setfade_Time)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / setfade_Time);

            fadeRT.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        fadeRT.anchoredPosition = endPos;

        now_FadeAnime = false;
    }

    /// <summary>
    /// 回転
    /// </summary>
    IEnumerator RotationAnime()
    {
        now_FadeAnime = true;

        fadeRT = fade_Image.rectTransform;

        //回転
        float startRot = fadeRT.localEulerAngles.z;
        float endRot = (360f + fadeRT.localEulerAngles.z) * rotation_RotationValue;
        endRot *= right_Rotation ? 1 : -1;

        //スケール
        float timer = 0f;
        float startScale = fadeIn_Flag ? 0f : 1f;
        float endScale = fadeIn_Flag ? 1f : 0f;

        while (timer < setfade_Time)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / setfade_Time);

            //回転
            float currentRot = Mathf.Lerp(startRot, endRot, t);
            fadeRT.localRotation = Quaternion.Euler(0f, 0f, currentRot);

            //スケール
            float expT = fadeIn_Flag ? 1f - Mathf.Pow(1f - t, scaleExp) : Mathf.Pow(t, scaleExp);
            float currentScale = Mathf.Lerp(startScale, endScale, expT);
            fadeRT.localScale = Vector3.one * currentScale;

            yield return null;
        }

        fadeRT.localRotation = Quaternion.Euler(0f, 0f, fadeRT.localEulerAngles.z);
        fadeRT.localScale = Vector3.one * endScale;

        now_FadeAnime = false;
    }

    /// <summary>
    /// フェード
    /// </summary>
    IEnumerator FadeAnime()
    {
        now_FadeAnime = true;

        float timer = 0f;
        float startAlpha = fadeIn_Flag ? 0f : 1f;
        float endAlpha = fadeIn_Flag ? 1f : 0f;

        while (timer < setfade_Time)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / setfade_Time);
            fadeColorA.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fade_Image.color = fadeColorA;
            yield return null;
        }

        fadeColorA.a = endAlpha;
        fade_Image.color = fadeColorA;

        now_FadeAnime = false;
    }
    #endregion
}
