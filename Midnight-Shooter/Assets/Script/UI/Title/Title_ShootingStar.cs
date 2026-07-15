using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/* 内容
 * ・タイトル画面の星空にまれに流れ星を出す
 */

/// <summary>
/// 流れ星のデータ
/// </summary>
struct StarData
{
    public Color color;
    public Vector2 direction;
    public float angle;
    public float animeTime;
    public float size;
    public float rotSpeed;
    public float fallSpeed;
}

public class Title_ShootingStar: MonoBehaviour
{
    #region 変数倉庫
    #region メインオプション
    [Header("◆メインオプション")]
    [SerializeField] GameObject starPrefab;
    [SerializeField] RectTransform starSkyMask1, starSkyMask2;
    [SerializeField] RectTransform starSky;
    [SerializeField, Tooltip("星の出現確率(%/s)"),Range(0f, 100f)]
    float star_SpawnPercent = 1f;
    [SerializeField, Tooltip("透明化解除の時間(%)"), Range(0, 1)]
    float star_StartFadePercent = 0.1f;
    [SerializeField, Tooltip("透明化し始める時間(%)"), Range(0, 1)]
    float star_EndFadePercent = 0.8f;
    [SerializeField, Tooltip("流れ星の色\n※このGroundの中からランダムな色が出る")]
    Gradient star_Color;
    [SerializeField, Tooltip("流れ星トレイルのアルファ値")]
    Gradient star_TrailColor;
    #endregion
    #region サブオプション
    [Header("◆サブオプション")]
    [SerializeField, Tooltip("流れ星のアニメーション時間\nX：最低値 / Y：最大値")]
    Vector2 star_AnimeTime = new Vector2(0.5f, 2f);
    [SerializeField, Tooltip("流れ星のスケール\nX：最低値 / Y：最大値")]
    Vector2 star_Size = new Vector2(0.5f, 2f);
    [SerializeField, Tooltip("流れ星の回転速度\nX：最低値 / Y：最大値")]
    Vector2 star_RotSpeed = new Vector2(10f, 360f);
    [SerializeField, Tooltip("流れ星の落下角度\nX：最低値 / Y：最大値")]
    Vector2 star_FallRot = new Vector2(-15f, -75f);
    [SerializeField, Tooltip("流れ星の落下速度\nX：最低値 / Y：最大値")]
    Vector2 star_FallSpeed = new Vector2(0.25f, 2f);
    [SerializeField, Tooltip("流れ星のトレイル長さ(X, 倍率)\nX：最低値 / Y：最大値")]
    Vector2 star_TrailMultiX = new Vector2(0.5f, 2f);
    [SerializeField, Tooltip("流れ星のトレイル太さ(Y, 倍率)\nX：最低値 / Y：最大値")]
    Vector2 star_TrailMultiY = new Vector2(0.1f, 1f);
    #endregion
    #endregion

    //削除処理
    void OnDisable()
    {
        foreach (var obj in starSkyMask1.GetComponentsInChildren<RectTransform>())
        {
            if (obj == starSkyMask1 || obj == starSky) continue;
            Destroy(obj.gameObject);
        }
        foreach (var obj in starSkyMask2.GetComponentsInChildren<RectTransform>())
        {
            if (obj == starSkyMask2) continue;
            Destroy(obj.gameObject);
        }
    }

    //毎秒流れ星スポーン(確率)
    void Update()
    {
        if (Random.value < star_SpawnPercent * Time.deltaTime)
        { StartCoroutine(ShootingStar()); }
    }

    /// <summary>
    /// 流れ星の生成
    /// </summary>
    IEnumerator ShootingStar()
    {
        //生成
        //Maskのセット
        RectTransform mask = Random.value < 0.5f
            ? starSkyMask1 : starSkyMask2;
        GameObject obj = Instantiate(starPrefab, mask);

        //座標のセット
        RectTransform rect = obj.GetComponent<RectTransform>();
        Transform star = rect.Find("Star");
        Transform trail = rect.Find("Trail");

        RectTransform starRect = star.GetComponent<RectTransform>();
        RectTransform trailRect = trail.GetComponent<RectTransform>();

        //画像のセット
        Image starImage = star.GetComponent<Image>();
        RawImage trailImage = trail.GetComponent<RawImage>();

        //初期化
        StarData data = CreateStarData(mask, rect, trailRect, starImage, trailImage);

        //アニメーション
        yield return AnimateStar(rect, starRect, starImage, trailImage, data);

        //削除
        Destroy(obj);
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns> 生成する星のデータ </returns>
    StarData CreateStarData(RectTransform mask, RectTransform rect, RectTransform trailRect,
    Image starImage, RawImage trailImage)
    {
        float px = Random.Range(0.1f, 1f);
        float py = Random.Range(0.5f, 1f);

        //場所決め
        rect.anchoredPosition = new Vector2(
            Mathf.Lerp(-mask.rect.width * 0.5f, mask.rect.width * 0.5f, px),
            Mathf.Lerp(-mask.rect.height * 0.5f, mask.rect.height * 0.5f, py));

        //データセット
        StarData data = new StarData();

        data.animeTime = Random.Range(star_AnimeTime.x, star_AnimeTime.y);
        data.size = Random.Range(star_Size.x, star_Size.y);
        data.rotSpeed = Random.Range(star_RotSpeed.x, star_RotSpeed.y);
        data.fallSpeed = Random.Range(star_FallSpeed.x, star_FallSpeed.y);

        float sizeRate = Mathf.InverseLerp(
            star_FallSpeed.x, star_FallSpeed.y, data.fallSpeed);

        //速度に応じてトレイルを長く
        //大きさに応じて細く
        Vector3 scale = trailRect.localScale;
        scale.x = Mathf.Lerp(star_TrailMultiX.x, star_TrailMultiX.y, sizeRate);
        scale.y = Mathf.Lerp(star_TrailMultiY.y, star_TrailMultiY.x, sizeRate);
        trailRect.localScale = scale;

        //初期の角度決め
        data.angle = Random.Range(star_FallRot.x, star_FallRot.y);

        data.direction = new Vector2(
            Mathf.Cos(data.angle * Mathf.Deg2Rad),
            Mathf.Sin(data.angle * Mathf.Deg2Rad));

        rect.localEulerAngles = new Vector3(0f, 0f, data.angle); //進行方向を向かせる

        //サイズ決め
        Color baseColor = starImage.color;
        Color color = star_Color.Evaluate(Random.value);

        rect.localScale = Vector3.one * data.size;
        data.color = color;

        //色決め
        Color starColor = star_Color.Evaluate(Random.value);

        data.color = starColor;
        starImage.color = starColor;

        #region トレイルの色決め
        const int width = 128;
        const int height = 32;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < height; y++)
        {
            //Y：上下端=0 中央=1
            float yAlpha = 1f - Mathf.Abs((y / (float)(height - 1)) * 2f - 1f);

            for (int x = 0; x < width; x++)
            {
                //X：星側=1 尻尾側=0
                float xAlpha = star_TrailColor.Evaluate(
                    1f - x / (float)(width - 1)).a;

                Color c = starColor;
                c.a = xAlpha * yAlpha;

                tex.SetPixel(x, y, c);
            }
        }

        tex.Apply();
        trailImage.texture = tex;
        #endregion

        return data;
    }

    /// <summary>
    /// 流れ星のアニメーション
    /// </summary>
    /// <param name="data"> 星のデータ </param>
    IEnumerator AnimateStar(RectTransform rect, RectTransform starRect,
        Image starImage, RawImage trailImage, StarData data)
    {
        float timer = 0f;

        float fadeInEnd = data.animeTime * star_StartFadePercent;
        float fadeOutStart = data.animeTime * star_EndFadePercent;
        float startAlpha = data.color.a;

        while (timer < data.animeTime)
        {
            timer += Time.deltaTime;

            //移動と回転
            rect.anchoredPosition += data.direction * data.fallSpeed * Time.deltaTime;
            starRect.Rotate(0, 0, data.rotSpeed * Time.deltaTime);

            //色の変更
            Color color = data.color;

            //フェードイン
            if (timer < fadeInEnd)
            { color.a = Mathf.Lerp(0f, startAlpha, timer / fadeInEnd); }
            //フェードアウト
            else if (timer >= fadeOutStart)
            {
                color.a = Mathf.Lerp(startAlpha, 0f,
                    (timer - fadeOutStart) / (data.animeTime - fadeOutStart));
            }
            //表示維持
            else { color.a = startAlpha; }

            //色の適応
            starImage.color = color;

            Color trail = color;
            trail.a *= 0.5f;
            trailImage.color = trail;

            yield return null;
        }
    }
}
