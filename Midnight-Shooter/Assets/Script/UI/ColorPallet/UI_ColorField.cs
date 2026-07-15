using UnityEngine;
using UnityEngine.EventSystems;

/* 内容
 * ・カラーフィールド処理
 */

public class UI_ColorField : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    bool dragging_Flag;

    UI_ColorPalletManager cpm;

    RectTransform fieldRect;
    RectTransform handleRect;

    void Awake()
    {
        cpm = UI_ColorPalletManager.manager;

        fieldRect = cpm.cf_ColorFieldBG.GetComponent<RectTransform>();
        handleRect = cpm.cf_Handle.GetComponent<RectTransform>();
    }

    #region ドラッグなどの処理
    public void OnPointerDown(PointerEventData eventData)
    {
        dragging_Flag = false;
        if (!RectTransformUtility.RectangleContainsScreenPoint
            (fieldRect, eventData.position, eventData.pressEventCamera)) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle
           (fieldRect, eventData.position,
            eventData.pressEventCamera, out Vector2 localPoint);

        UpdateColor(localPoint);
        dragging_Flag = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging_Flag) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle
           (fieldRect, eventData.position,
            eventData.pressEventCamera, out Vector2 localPoint);

        UpdateColor(localPoint);
    }
    #endregion

    #region ハンドルの位置更新
    /// <summary>
    /// S/V値からハンドル位置を更新
    /// </summary>
    public void SetHandlePosition(float s, float v)
    {
        float x = Mathf.Lerp
           (-fieldRect.rect.width * 0.5f,
            fieldRect.rect.width * 0.5f,
            s);

        float y = Mathf.Lerp
           (-fieldRect.rect.height * 0.5f,
            fieldRect.rect.height * 0.5f, v);

        handleRect.anchoredPosition = new Vector2(x, y);
    }

    /// <summary>
    /// ハンドル色変更
    /// </summary>
    void UpdateColor(Vector2 localPos)
    {
        float halfW = fieldRect.rect.width * 0.5f;
        float halfH = fieldRect.rect.height * 0.5f;

        //範囲内に制限
        localPos.x = Mathf.Clamp(localPos.x, -halfW, halfW);
        localPos.y = Mathf.Clamp(localPos.y, -halfH, halfH);

        //ハンドル移動
        handleRect.anchoredPosition = localPos;

        float s = Mathf.InverseLerp(-halfW, halfW, localPos.x);
        float v = Mathf.InverseLerp(-halfH, halfH, localPos.y);

        float h = cpm.hue_Bar.value;

        //左上なら黒(透明度が50%未満でも黒)
        cpm.cf_Handle.color = ((s < 0.5f && v > 0.5f) || cpm.alpha_Bar.value < 0.5f)
        ? Color.black : Color.white;

        //色の適応
        Color selectedColor = Color.HSVToRGB(h, s, v);
        selectedColor.a = cpm.alpha_Bar.value;
        cpm.ApplyColor(selectedColor);
    }
    #endregion
}
