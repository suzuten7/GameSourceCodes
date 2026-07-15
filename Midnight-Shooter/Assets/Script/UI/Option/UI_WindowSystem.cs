using UnityEngine;
using UnityEngine.EventSystems;

/* 内容
 * ・Windowの移動部分を再現
 */

public class UI_WindowSystem : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    #region 変数倉庫
    bool canDrag = false;

    Vector2 dragOffset;
    [SerializeField, Tooltip("ドラックに合わせて動かす物")]
    RectTransform moveTarget;
    [SerializeField, Tooltip("ドラック開始時に移動を許可するエリア")]
    RectTransform tabArea;
    Canvas clampArea;
    #endregion

    //初期化処理
    void Awake()
    { if (clampArea == null) clampArea = GetComponentInParent<Canvas>(); }

    /// <summary>
    /// ドラック開始時(実質：Start)
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Tabの上を掴んでいるか判定
        if (!RectTransformUtility.RectangleContainsScreenPoint(
                tabArea, eventData.position, eventData.pressEventCamera))
        { canDrag = false; return; }

        canDrag = true;
        RectTransform clampRect = clampArea.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            clampRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        dragOffset = localPoint - (Vector2)moveTarget.localPosition;
        moveTarget.SetAsLastSibling();
    }

    /// <summary>
    /// ドラック中(実質：Update)
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;

        RectTransform clampRect = clampArea.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            clampRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            moveTarget.localPosition = localPoint - dragOffset;
            UIClamp();
        }
    }

    /// <summary>
    /// 移動処理(可動範囲の制限付き)
    /// </summary>
    void UIClamp()
    {
        RectTransform clampRect = clampArea.GetComponent<RectTransform>();

        Vector2 targetPos = moveTarget.localPosition;
        Vector2 targetSize = clampRect.rect.size;
        Vector2 selfSize = moveTarget.rect.size;

        float halfW = selfSize.x * 0.5f;
        float halfH = selfSize.y * 0.5f;

        float clampX = Mathf.Clamp(targetPos.x,
            -targetSize.x * 0.5f + halfW, targetSize.x * 0.5f - halfW);
        float clampY = Mathf.Clamp(targetPos.y,
            -targetSize.y * 0.5f + halfH, targetSize.y * 0.5f - halfH);

        moveTarget.localPosition = new Vector3(
            clampX, clampY, moveTarget.localPosition.z);
    }
}
