using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* 内容
 * ・ボタンのサイズ変更アニメーション
 */

public class UI_ButtonAnime : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    #region 変数倉庫
    #region メインオプション
    [Header("◆メインオプション")]
    [SerializeField, Tooltip("アニメーションする時間")]
    float animeTime = 0.05f;

    [SerializeField, Tooltip("選択時、サイズを大きくする\n※ベーススケールに加算"), Range(-1, 1)]
    float selectAddScale = 0.05f;
    [SerializeField, Tooltip("クリック時、サイズを大きくする\n※ベーススケールに加算"), Range(-1, 1)]
    float clickAddScale = -0.05f;
    #endregion
    #region サブオプション
    [Header("◆サブオプション")]
    [SerializeField] Button animeButton;
    [SerializeField, Tooltip("アニメーションスタートするオブジェクト")]
    GameObject animeObj;
    Vector3 baseScale;
    Coroutine animeCoroutine;

    bool hover_Flag = false;
    bool press_Flag = false;
    #endregion
    #endregion

    void Start()
    {
        baseScale = animeObj.transform.localScale;

        RectTransform rect = animeObj.GetComponent<RectTransform>();

        Vector2 size = rect.rect.size;
        Vector2 deltaPivot = new Vector2(0.5f, 0.5f) - rect.pivot;

        rect.anchoredPosition += Vector2.Scale(size, deltaPivot);
        rect.pivot = new Vector2(0.5f, 0.5f);
    }

    /// <summary>
    /// ボタンの拡大縮小アニメーションを追加する
    /// </summary>
    public void ButtonAnime(float addScale_Flag)
    {
        if (!animeButton.interactable) return;

        Vector3 targetScale = baseScale * (1f + addScale_Flag);

        if (animeCoroutine != null) { StopCoroutine(animeCoroutine); }
        animeCoroutine = StartCoroutine(ScaleAnime(targetScale));
    }

    /// <summary>
    /// スケールのアニメーション
    /// </summary>
    /// <param name="targetScale"> 目標サイズ </param>
    IEnumerator ScaleAnime(Vector3 targetScale)
    {
        Vector3 start = animeObj.transform.localScale;
        float time = 0f;

        while (time < animeTime)
        {
            time += Time.deltaTime;
            animeObj.transform.localScale = Vector3.Lerp(start, targetScale, time / animeTime);
            yield return null;
        }

        animeObj.transform.localScale = targetScale;
    }

    #region カーソルがボタン上に乗った時の処理
    /// <summary>
    /// マウスが乗った
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        hover_Flag = true;

        if (!press_Flag) { ButtonAnime(selectAddScale); }
    }

    /// <summary>
    /// マウスが離れた
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        hover_Flag = false;

        if (!press_Flag) { ButtonAnime(0f); }
    }
    #endregion
    #region ボタンを押したときの処理
    /// <summary>
    /// ボタンを押した
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        press_Flag = true;

        ButtonAnime(clickAddScale);
    }

    /// <summary>
    /// ボタンを離した
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        press_Flag = false;

        //ボタン上なら選択サイズ
        //ボタン外なら通常サイズ
        ButtonAnime(hover_Flag ? selectAddScale : 0f);
    }
    #endregion
}
