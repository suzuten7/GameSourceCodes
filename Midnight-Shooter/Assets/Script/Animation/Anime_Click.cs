using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 内容
 * ・クリックのアニメーション
 * ・カーソル移動のアニメーション
 * ・ドラッグのアニメーション
 */

#region 変数倉庫1
//色/縮小/拡大の種類
internal enum Cursor_AnimePattern
{
    Color,
    Reduction,
    Expansion,
    RedMix,
    ExpMix
}

//変化カーブの種類
internal enum Cursor_ChangePattern
{
    InExpo,   //初め：遅い / 最後：早い
    Linear,   //等速
    OutExpo   //初め：早い / 最後：遅い
}
#endregion
#region 変数倉庫2
//クリック
[System.Serializable]
internal class ClickOption
{
    [SerializeField, Tooltip("アニメーション(クリック)")]
    public Sprite sp_Click;
    [SerializeField, Tooltip("マテリアル(クリック)")]
    public Material mt_Click;
    [SerializeField, Tooltip("ランダム初期表示倍率(クリック)\nx：最低値\ny：最高値")]
    public Vector2 def_ClickMulti = new Vector2(0.05f, 0.05f);
    [SerializeField, Tooltip("ランダム最大表示倍率(クリック)\nx：最低値\ny：最高値")]
    public Vector2 max_ClickMulti = new Vector2(0.5f, 1f);
    [SerializeField, Tooltip("ランダム最小表示倍率(クリック)\nx：最低値\ny：最高値")]
    public Vector2 min_ClickMulti = new Vector2(0f, 0f);
    [SerializeField, Tooltip("色/縮小/拡大の種類(クリック)" +
        "\nColor[色]\nReduction[縮小]\nExpansion[拡大]" +
        "\nRedMix[色と縮小]\nExpMix[色と拡大]")]
    public Cursor_AnimePattern ap_Click = Cursor_AnimePattern.ExpMix;
    [SerializeField, Tooltip("変化カーブの種類の種類(クリック)" +
        "\nInExpo：[初め：遅い / 最後：早い]\nLinear：[等速]" +
        "\nOutExpo[初め：早い / 最後：遅い]")]
    public Cursor_ChangePattern cp_Click = Cursor_ChangePattern.OutExpo;
    [SerializeField, Tooltip("ランダム回転速度(クリック)\nx：最低値\ny：最高値")]
    public Vector2 rt_Click = new Vector2(0f, 0f);
    [SerializeField, Tooltip("ランダムクールタイム(クリック)\nx：最低値\ny：最高値")]
    public Vector2 ct_Click = new Vector2(0f, 0f);
    [SerializeField, Tooltip("ランダムアニメーション時間(クリック)\nx：最低値\ny：最高値")]
    public Vector2 at_Click = new Vector2(0.5f, 1f);
    [SerializeField, Tooltip("アニメーションの色(クリック)" +
        "\n※色のアニメーションでは無い時は\n　グラデーションの一番最初の色を適応")]
    public Gradient g_Click;
}

//カーソル移動
[System.Serializable]
internal class CursorMoveOption
{
    [SerializeField, Tooltip("アニメーション(カーソル移動)")]
    public Sprite sp_CursorMove;
    [SerializeField, Tooltip("マテリアル(カーソル移動)")]
    public Material mt_CursorMove;
    [SerializeField, Tooltip("ランダム初期表示倍率(カーソル移動)\nx：最低値\ny：最高値")]
    public Vector2 def_CursorMoveMulti = new(0.1f, 0.25f);
    [SerializeField, Tooltip("ランダム最大表示倍率(カーソル移動)\nx：最低値\ny：最高値")]
    public Vector2 max_CursorMoveMulti = new(0.5f, 1f);
    [SerializeField, Tooltip("ランダム最小表示倍率(カーソル移動)\nx：最低値\ny：最高値")]
    public Vector2 min_CursorMoveMulti = new(0f, 0f);
    [SerializeField, Tooltip("色/縮小/拡大の種類(カーソル移動)" +
        "\nColor[色]\nReduction[縮小]\nExpansion[拡大]" +
        "\nRedMix[色と縮小]\nExpMix[色と拡大]")]
    public Cursor_AnimePattern ap_CursorMove = Cursor_AnimePattern.RedMix;
    [SerializeField, Tooltip("変化カーブの種類(カーソル移動)" +
        "\nInExpo：[初め：遅い / 最後：早い]\nLinear：[等速]" +
        "\nOutExpo[初め：早い / 最後：遅い]")]
    public Cursor_ChangePattern cp_CursorMove = Cursor_ChangePattern.InExpo;
    [SerializeField, Tooltip("ランダム回転速度(カーソル移動)\nx：最低値\ny：最高値")]
    public Vector2 rt_CursorMove = new Vector2(-180f, 180f);
    [SerializeField, Tooltip("ランダムクールタイム(カーソル移動)\nx：最低値\ny：最高値")]
    public Vector2 ct_CursorMove = new Vector2(0.01f, 0.1f);
    [SerializeField, Tooltip("ランダムアニメーション時間(カーソル移動)\nx：最低値\ny：最高値")]
    public Vector2 at_CursorMove = new Vector2(0.5f, 1f);
    [SerializeField, Tooltip("アニメーションの色(カーソル移動)" +
        "\n※色のアニメーションでは無い時は\n　グラデーションの一番最初の色を適応")]
    public Gradient g_CursorMove;
}

//ドラッグ
[System.Serializable]
internal class DragOption
{
    [SerializeField, Tooltip("アニメーション(ドラッグ)")]
    public Sprite sp_Drag;
    [SerializeField, Tooltip("マテリアル(ドラッグ)")]
    public Material mt_Drag;
    [SerializeField, Tooltip("ランダム初期表示倍率(ドラッグ)\nx：最低値\ny：最高値")]
    public Vector2 def_DragMulti = new(0.1f, 0.25f);
    [SerializeField, Tooltip("ランダム最大表示倍率(ドラッグ)\nx：最低値\ny：最高値")]
    public Vector2 max_DragMulti = new(0.5f, 1f);
    [SerializeField, Tooltip("ランダム最小表示倍率(ドラッグ)\nx：最低値\ny：最高値")]
    public Vector2 min_DragMulti = new(0f, 0f);
    [SerializeField, Tooltip("色/縮小/拡大の種類(ドラッグ)" +
        "\nColor[色]\nReduction[縮小]\nExpansion[拡大]" +
        "\nRedMix[色と縮小]\nExpMix[色と拡大]")]
    public Cursor_AnimePattern ap_Drag = Cursor_AnimePattern.RedMix;
    [SerializeField, Tooltip("変化カーブの種類(ドラッグ)" +
        "\nInExpo：[初め：遅い / 最後：早い]\nLinear：[等速]" +
        "\nOutExpo[初め：早い / 最後：遅い]")]
    public Cursor_ChangePattern cp_Drag = Cursor_ChangePattern.InExpo;
    [SerializeField, Tooltip("ランダム回転速度(ドラッグ)\nx：最低値\ny：最高値")]
    public Vector2 rt_Drag = new Vector2(-180f, 180f);
    [SerializeField, Tooltip("ランダムクールタイム(ドラッグ)\nx：最低値\ny：最高値")]
    public Vector2 ct_Drag = new Vector2(0.01f, 0.1f);
    [SerializeField, Tooltip("ランダムアニメーション時間(ドラッグ)\nx：最低値\ny：最高値")]
    public Vector2 at_Drag = new Vector2(0.5f, 1f);
    [SerializeField, Tooltip("アニメーションの色(ドラッグ)\n" +
        "※色のアニメーションでは無い時は\n　グラデーションの一番最初の色を適応")]
    public Gradient g_Drag;
}
#endregion

//※現在未対応
public class Anime_Click : MonoBehaviour
{
    #region 変数倉庫3
    #region メインオプション
    [Header("◆メインオプション")]
    [Header("- クリック -")]
    [SerializeField] ClickOption clickOption;
    float nowRT_Click, nowCT_Click, nowAT_Click = 0f;
    [Header("- カーソル移動 -")]
    [SerializeField] CursorMoveOption cursorMoveOption;
    float nowRT_CursorMove, nowCT_CursorMove, nowAT_CursorMove = 0f;
    [Header("- ドラッグ -")]
    [SerializeField] DragOption dragOption;
    float nowRT_Drag, nowCT_Drag, nowAT_Drag = 0f;
    #endregion
    #region サブオプション
    [Header("◆サブオプション")]
    [SerializeField, Tooltip("アニメーションを再生しないカーソル移動距離(px)")]
    float cursor_AnimeReqDis = 25f;
    [SerializeField, Tooltip("ドラッグ判定までの時間")]
    float setHold_DragTime = 0.25f;
    [SerializeField, Tooltip("変化カーブ倍率\n※等速は関係なし")]
    float cp_Multi = 1f;

    List<GameObject> animePool = new List<GameObject>();

    float nowHold_DragTime, nowMoveDis;
    Vector2 lastMousePos;
    bool isMouseDown = false;
    bool isDragging = false;
    RectTransform anime_Pos;

    //軽量化用
    Dictionary<GameObject, RectTransform> rtCache = new();
    Dictionary<GameObject, Image> imgCache = new();
    #endregion
    #endregion

    //初期化
    void Awake()
    {
        anime_Pos = GetComponent<RectTransform>();
        nowHold_DragTime = setHold_DragTime;
    }

    //初期化
    void Start()
    { lastMousePos = Input.mousePosition; }

    void Update()
    {
        if (!Application.isFocused) return; //画面フォーカス中かの判定

        Vector2 now = Input.mousePosition;

        nowMoveDis += Vector2.Distance(now, lastMousePos);
        lastMousePos = now;

        nowCT_Click -= nowCT_Click > 0f ? Time.deltaTime : 0f;
        nowCT_CursorMove -= nowCT_CursorMove > 0f ? Time.deltaTime : 0f;
        nowCT_Drag -= nowCT_Drag > 0f ? Time.deltaTime : 0f;

        #region クリック
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)
            || Input.GetMouseButtonDown(2))
        {
            isMouseDown = true;
            isDragging = true;
            nowHold_DragTime = setHold_DragTime;
        }

        // 押している間
        if (isMouseDown && nowHold_DragTime > 0f)
        {
            nowHold_DragTime -= Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (clickOption.sp_Click != null && nowCT_Click <= 0f && nowHold_DragTime > 0f)
            {
                nowRT_Click = Random.Range(clickOption.rt_Click.x, clickOption.rt_Click.y);
                nowAT_Click = Random.Range(clickOption.at_Click.x, clickOption.at_Click.y);
                nowCT_Click = Random.Range(clickOption.ct_Click.x, clickOption.ct_Click.y);

                Start_Anime(now, clickOption.sp_Click, clickOption.def_ClickMulti,
                    clickOption.max_ClickMulti, clickOption.min_ClickMulti,
                    clickOption.ap_Click, clickOption.cp_Click, nowRT_Click, nowAT_Click,
                    clickOption.g_Click, clickOption.mt_Click);
            }

            isMouseDown = false;
            isDragging = false;
        }
        #endregion
        #region カーソル移動
        if (cursorMoveOption.sp_CursorMove != null && nowHold_DragTime > 0f
            && nowCT_CursorMove <= 0f && nowMoveDis >= cursor_AnimeReqDis)
        {
            nowMoveDis = 0f;

            nowRT_CursorMove = Random.Range(cursorMoveOption.rt_CursorMove.x, cursorMoveOption.rt_CursorMove.y);
            nowAT_CursorMove = Random.Range(cursorMoveOption.at_CursorMove.x, cursorMoveOption.at_CursorMove.y);
            float setCT_CursorMove = Random.Range(cursorMoveOption.ct_CursorMove.x, cursorMoveOption.ct_CursorMove.y);

            nowCT_CursorMove = setCT_CursorMove;
            Vector2 pos = now;

            Start_Anime(pos, cursorMoveOption.sp_CursorMove, cursorMoveOption.def_CursorMoveMulti,
                cursorMoveOption.max_CursorMoveMulti, cursorMoveOption.min_CursorMoveMulti,
                cursorMoveOption.ap_CursorMove, cursorMoveOption.cp_CursorMove,
                nowRT_CursorMove, nowAT_CursorMove, cursorMoveOption.g_CursorMove, cursorMoveOption.mt_CursorMove);
        }
        else if (cursorMoveOption.sp_CursorMove != null && nowHold_DragTime > 0f
            && nowCT_Drag > 0f && nowCT_CursorMove > 0f)
        { nowCT_CursorMove += Time.deltaTime; }
        #endregion
        #region ドラッグ
        //ドラッグを[開始/終了]
        if (Input.GetMouseButtonDown(0)) { isDragging = true; }
        if (Input.GetMouseButtonUp(0)) { isDragging = false; nowHold_DragTime = setHold_DragTime; }

        if (isDragging && nowHold_DragTime > 0f) { nowHold_DragTime -= Time.deltaTime; }
        else if (dragOption.sp_Drag != null && nowHold_DragTime <= 0f
            && nowCT_Drag <= 0f && nowMoveDis >= cursor_AnimeReqDis)
        {
            nowMoveDis = 0f;

            nowRT_Drag = Random.Range(dragOption.rt_Drag.x, dragOption.rt_Drag.y);
            nowAT_Drag = Random.Range(dragOption.at_Drag.x, dragOption.at_Drag.y);
            float setCT_Drag = Random.Range(dragOption.ct_Drag.x, dragOption.ct_Drag.y);

            nowCT_Drag = setCT_Drag;
            Vector2 pos = now;

            Start_Anime(pos, dragOption.sp_Drag, dragOption.def_DragMulti,
                dragOption.max_DragMulti, dragOption.min_DragMulti,
                dragOption.ap_Drag, dragOption.cp_Drag, nowRT_Drag, nowAT_Drag, dragOption.g_Drag, dragOption.mt_Drag);
        }
        else if (dragOption.sp_Drag != null && nowHold_DragTime <= 0f
            && nowCT_Drag > 0f && nowCT_CursorMove > 0f)
        { nowCT_Drag -= Time.deltaTime; }
        #endregion
    }

    #region アニメーション処理
    //アニメーション生成
    void Start_Anime
        (Vector2 screenPos, Sprite sprite, Vector2 defMulti, Vector2 maxMulti, Vector2 minMulti,
        Cursor_AnimePattern ap, Cursor_ChangePattern cp, float rotate, float animTime, Gradient grad, Material mat)
    {
        //スクリーン画面外なら無視
        if (!RectTransformUtility.RectangleContainsScreenPoint(anime_Pos, screenPos, null)) return;

        //使いまわしのオブジェクト取得
        GameObject obj = Get_AnimeObj();
        obj.transform.SetParent(anime_Pos, false);

        RectTransform rt = rtCache[obj];
        Image img = imgCache[obj];

        rtCache[obj] = rt;
        imgCache[obj] = img;

        //再利用リセット
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;

        img.sprite = sprite;
        img.color = Color.white;
        img.material = mat != null ? mat : null;

        img.sprite = sprite;
        img.raycastTarget = false;

        //マテリアルの適応
        if (mat != null) { img.material = mat; }

        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (anime_Pos, screenPos, null, out Vector2 localPos);

        rt.anchoredPosition = localPos;
        rt.localScale = Vector3.one;

        StartCoroutine(Update_Anime
            (obj, rt, img, defMulti, maxMulti, minMulti,
             ap, cp, rotate, animTime, grad));
    }

    //アニメーションの続き
    IEnumerator Update_Anime
        (GameObject obj, RectTransform rt, Image img, Vector2 defMulti,
        Vector2 maxMulti, Vector2 minMulti, Cursor_AnimePattern ap,
        Cursor_ChangePattern cp, float rotate, float animTime, Gradient grad)
    {
        float timer = 0f;

        float startMulti = GetRandomMulti(defMulti);
        float endMulti = startMulti;

        switch (ap)
        {
            case Cursor_AnimePattern.Reduction:
            case Cursor_AnimePattern.RedMix:
                endMulti = GetRandomMulti(minMulti);
                break;

            case Cursor_AnimePattern.Expansion:
            case Cursor_AnimePattern.ExpMix:
                endMulti = GetRandomMulti(maxMulti);
                break;
        }

        Vector3 startScale = Vector3.one * startMulti;
        Vector3 endScale = Vector3.one * endMulti;

        rt.localScale = startScale;

        while (timer < animTime)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / animTime);
            float rate = ChangePattern_Anime(t, cp);

            //色
            if (ap == Cursor_AnimePattern.Color ||
                ap == Cursor_AnimePattern.RedMix ||
                ap == Cursor_AnimePattern.ExpMix)
            { img.color = grad.Evaluate(rate); }
            else { img.color = grad.colorKeys[0].color; }

            //サイズ
            rt.localScale = Vector3.Lerp(startScale, endScale, rate);

            //回転
            if (rotate != 0f)
            { rt.Rotate(0f, 0f, rotate * Time.deltaTime); }

            yield return null;
        }

        obj.SetActive(false);
    }
    #endregion
    #region 画面フォーカスではないときの初期化処理
    //画面切り替えや画面最大化時の処理
    void OnApplicationFocus(bool hasFocus) { if (!hasFocus) { ResetInputState(); } }

    //画面最小化時の処理
    void OnApplicationPause(bool pause) { if (pause) { ResetInputState(); } }

    //画面フォーカス後の初期化
    void ResetInputState()
    {
        isMouseDown = false;
        isDragging = false;

        nowHold_DragTime = setHold_DragTime;
        nowMoveDis = 0f;

        nowCT_Click = 0f;
        nowCT_CursorMove = 0f;
        nowCT_Drag = 0f;
    }
    #endregion
    #region 軽量化処理
    //使いまわしのオブジェクト生成
    GameObject Create_AnimeObj()
    {
        GameObject obj = new("AnimeObj");
        obj.transform.SetParent(anime_Pos, false);

        RectTransform rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);

        Image img = obj.AddComponent<Image>();
        img.raycastTarget = false;

        rtCache[obj] = rt;
        imgCache[obj] = img;

        return obj;
    }

    //使いまわしのオブジェクトが無いか検索
    GameObject Get_AnimeObj()
    {
        foreach (GameObject obj in animePool)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        //足りなければ追加生成
        GameObject newObj = Create_AnimeObj();
        animePool.Add(newObj);
        return newObj;
    }
    #endregion
    #region その他の処理
    //サイズをランダムに取得
    float GetRandomMulti(Vector2 range) { return Random.Range(range.x, range.y); }

    //変化カーブ
    float ChangePattern_Anime(float t, Cursor_ChangePattern cp)
    {
        t = Mathf.Clamp01(t);
        float rate;

        switch (cp)
        {
            case Cursor_ChangePattern.InExpo:
                rate = (t == 0f) ? 0f : Mathf.Pow(2f, (10f * cp_Multi) * (t - 1f));
                break;

            case Cursor_ChangePattern.OutExpo:
                rate = (t == 1f) ? 1f : 1f - Mathf.Pow(2f, -(10f * cp_Multi) * t);
                break;

            case Cursor_ChangePattern.Linear:
            default:
                rate = t;
                break;
        }

        return Mathf.Clamp01(rate);
    }
    #endregion
}
