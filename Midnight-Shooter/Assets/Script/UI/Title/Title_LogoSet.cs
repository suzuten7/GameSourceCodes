using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* 内容
 * ・タイトルロゴ等のアニメーション
 * ・タイトルロゴ等の透過＆削除
 */

#region 変数倉庫1
[System.Serializable, HideInInspector]
public class TextData
{
    [SerializeField, Tooltip("各テキスト")]
    public TextMeshProUGUI text;
    [SerializeField, Tooltip("アニメーションの遅延時間")]
    public float anime_StartDelay;
    [Tooltip("N秒かけてアニメーションする")]
    public float anime_Time;
    [Tooltip("N秒毎にアニメーションする")]
    public float anime_LoopDelay;

    public float nowTimer;
    public Vector2 startPos;
    public Color start_color;
}
#endregion

public class Title_LogoSet : MonoBehaviour
{
    #region 変数倉庫2
    #region メインオプション
    [Header("◆メインオプション")]
    [SerializeField, Tooltip("このスクリプトの大半の部分")]
    CanvasGroup startGroup;
    [SerializeField, Tooltip("ボタンを押した後のグループ")]
    CanvasGroup mainGroup;
    [SerializeField] Image logo;
    [SerializeField] Transform texts;
    TextData[] text;

    bool goMain_Flag = false;
    bool comeBack_Flag = false;
    #endregion
    #region サブオプション
    [Header("◆サブオプション")]
    [Header("- 共通 -")]
    [SerializeField, Tooltip("ゲームスタート時の待機時間")]
    float setFade_Time = 0.5f;
    float nowFade_Time;

    [Header("- ロゴ -")]
    [SerializeField, Tooltip("回転角度\n※片方に回転する角度")]
    float anime_Angle = 5;
    [SerializeField, Tooltip("N秒かけてアニメーションする\n※片方に回転する時の時間")]
    float anime_LogoTime = 2;
    float now_AnimeLogoTime;
    Quaternion startRotation;

    [Header("- テキスト -")]
    //時間
    [SerializeField, Tooltip("アニメーションの遅延時間\n※開始時のみ")]
    float anime_StartDelay = 1f;
    [SerializeField, Tooltip("各テキスト毎のアニメーションの加算遅延時間\n※開始時のみ")]
    float anime_Delay = 0.25f;
    [SerializeField, Tooltip("N秒かけてアニメーションする")]
    float setAnime_Time = 0.5f;
    [SerializeField, Tooltip("N秒毎にアニメーションする")]
    float setAnime_LoopDelay = 3f;

    //時間
    [SerializeField, Tooltip("最大移動量")]
    float anime_Value = 10f;
    [SerializeField, Tooltip("アニメーション時の色の変化")]
    Gradient anime_gradient;
    #endregion
    #endregion

    void Awake()
    {
        startGroup.alpha = 1f;
        startGroup.blocksRaycasts = true;
        startGroup.interactable = true;
        mainGroup.alpha = 0f;
        mainGroup.blocksRaycasts = true;
        mainGroup.interactable = false;
        mainGroup.gameObject.SetActive(false);

        nowFade_Time = 0f;
        goMain_Flag = false;
        comeBack_Flag = false;

        //ロゴの初期化
        now_AnimeLogoTime = 0f;
        startRotation = logo.rectTransform.localRotation;

        //テキストの初期化
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            texts.GetComponent<RectTransform>());

        text = new TextData[texts.childCount];
        int countIndex = 0;

        for (int i = 0; i < texts.childCount; i++)
        {
            var tmp = texts.GetChild(i).GetComponent<TextMeshProUGUI>();

            //空文字または空白だけならスキップ
            if (!tmp.gameObject.activeInHierarchy || string.IsNullOrWhiteSpace(tmp.text))
                continue;

            text[i] = new TextData
            {
                text = tmp,
                anime_StartDelay = anime_StartDelay + anime_Delay * countIndex,
                anime_Time = setAnime_Time,
                anime_LoopDelay = setAnime_LoopDelay,

                nowTimer = 0f,
                startPos = tmp.rectTransform.anchoredPosition,
                start_color = tmp.color
            };

            countIndex++;
        }
    }

    void Update()
    {
        //アニメーション終了処理
        if (nowFade_Time >= setFade_Time * 2) { return; }
        else if(goMain_Flag)
        { GameStart(); }
        else if (comeBack_Flag)
        { ComebackTitle_Update(); }

        //各アニメーション処理
        AnimeLogo();
        AnimeText();
    }

    #region アニメーション処理
    /// <summary>
    /// ロゴの回転
    /// </summary>
    void AnimeLogo()
    {
        now_AnimeLogoTime += Time.deltaTime;
        float angle = Mathf.Sin(now_AnimeLogoTime * Mathf.PI / anime_LogoTime) * anime_Angle;
        logo.rectTransform.localRotation = startRotation * Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// テキスト
    /// </summary>
    void AnimeText()
    {
        foreach (var data in text)
        {
            if (data == null) continue;
            data.nowTimer += Time.deltaTime;

            //開始待機
            if (data.nowTimer < data.anime_StartDelay) continue;

            float animeTimer = data.nowTimer - data.anime_StartDelay;

            //アニメーション
            if (animeTimer < data.anime_Time)
            {
                //位置の変更
                //0→1→0
                float timer = animeTimer / data.anime_Time;
                float posY = Mathf.Sin(timer * Mathf.PI) * anime_Value;

                data.text.rectTransform.anchoredPosition =
                    data.startPos + Vector2.up * posY;

                //色の変更
                Color gradientColor = anime_gradient.Evaluate(timer);
                Color color = Color.Lerp(data.start_color, gradientColor, Mathf.Sin(timer * Mathf.PI));
                data.text.color = color;
            }
            //次のループ開始時間
            else
            {
                data.text.rectTransform.anchoredPosition = data.startPos;
                data.text.color = data.start_color;
                data.nowTimer -= data.anime_Time + data.anime_LoopDelay;
            }
        }
    }
    #endregion

    /// <summary>
    /// ゲーム開始時の処理
    /// </summary>
    public void GameStart()
    {
        if (!goMain_Flag) goMain_Flag = true;

        nowFade_Time += Time.deltaTime;
        float titleTimer = 1 - nowFade_Time / setFade_Time;

        //タイトルの透明度を下げる
        if(nowFade_Time < setFade_Time)
        { startGroup.alpha = titleTimer; }
        //メインの透明度を上げる
        else if (nowFade_Time >= setFade_Time)
        {
            float mainTimer = (nowFade_Time - setFade_Time) / setFade_Time;

            //タイトルのCanvasGroup無効化
            if (startGroup.interactable)
            {
                startGroup.alpha = 0f;
                startGroup.blocksRaycasts = false;
                startGroup.interactable = false;
                mainGroup.gameObject.SetActive(true);
            }

            mainGroup.alpha = mainTimer;

            //メインのCanvasGroup有効化
            if (nowFade_Time >= setFade_Time * 2)
            {
                mainGroup.alpha = 1f;
                mainGroup.blocksRaycasts = true;
                mainGroup.interactable = true;
                
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Titleに戻ってきたときの処理
    /// </summary>
    /// <remarks> その前にこのオブジェクトをアクティブにすることを忘れずに!! </remarks>
    public void ComebackTitle()
    {
        #region 初期化処理
        startGroup.alpha = 0f;
        startGroup.blocksRaycasts = false;
        startGroup.interactable = false;
        mainGroup.alpha = 1f;
        mainGroup.blocksRaycasts = false;

        nowFade_Time = 0f;
        goMain_Flag = false;
        comeBack_Flag = true;

        //ロゴの初期化
        now_AnimeLogoTime = 0f;
        logo.rectTransform.localRotation = startRotation;

        //テキストの初期化
        if (text == null) { return; }
        foreach (var data in text)
        {
            if (data == null) continue;
            data.nowTimer = 0f;
        }
        #endregion
    }

    /// <summary>
    /// タイトルに戻るためのフェード
    /// </summary>
    void ComebackTitle_Update()
    {
        nowFade_Time += Time.deltaTime;
        float mainTimer = 1 - nowFade_Time / setFade_Time;

        //メインの透明度を下げる
        if (nowFade_Time < setFade_Time)
        { mainGroup.alpha = mainTimer; }
        //タイトルの透明度を上げる
        else if (nowFade_Time >= setFade_Time)
        {
            float titleTimer = (nowFade_Time - setFade_Time) / setFade_Time;

            //メインのCanvasGroup無効化
            if (mainGroup.interactable)
            {
                mainGroup.alpha = 0f;
                mainGroup.interactable = false;
                mainGroup.gameObject.SetActive(false);
            }

            startGroup.alpha = titleTimer;

            //タイトルのCanvasGroupを有効化
            if (nowFade_Time >= setFade_Time * 2)
            {
                startGroup.alpha = 1f;
                startGroup.interactable = true;
                startGroup.blocksRaycasts = true;
                nowFade_Time = 0f;
                comeBack_Flag = false;
            }
        }
    }
}
