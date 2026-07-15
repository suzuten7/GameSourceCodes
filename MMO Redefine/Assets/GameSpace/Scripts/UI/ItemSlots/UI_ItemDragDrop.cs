namespace UIs
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using static UI_System;
    using static Player.Player_Controle;

    public class UI_ItemDragDrop : MonoBehaviour, IDragHandler,
        IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region 変数倉庫
        #region メインオプション
        [Header("◆メインオプション")]
        public UI_ItemSlotBase ItemSlot;
        float holdTime = 0.15f;
        #endregion
        #region その他
        bool isHolding = false;
        bool isDragging = false;
        float holdTimer = 0f;

        GameObject cloneObj;
        RectTransform cloneRect;
        RectTransform rectTransform;
        #endregion

        #endregion

        void Update()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (ItemSlot == null) ItemSlot = GetComponent<UI_ItemSlotBase>();
            // ホールドチェック
            if (isHolding && !isDragging)
            {
                // カーソルが範囲内かチェック
                if (!RectTransformUtility.RectangleContainsScreenPoint
                    (rectTransform, PCont.PI.actions["Point"].ReadValue<Vector2>(),ui_system.DragCanvas.worldCamera))
                {
                    ResetState(); return;
                }

                // 時間経過でドラッグ開始
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdTime) { MakeDragClone(); }
            }
        }

        //Clone生成
        void MakeDragClone()
        {
            UI_System.IsDrag = true;
            isDragging = true;
            // Cloneを生成
            cloneObj = Instantiate(gameObject, ui_system.DragCanvas.transform);
            cloneRect = cloneObj.GetComponent<RectTransform>();

            // 位置移動
            cloneRect.position = rectTransform.position;
            cloneRect.sizeDelta = rectTransform.sizeDelta;

            // 半透明化
            CanvasGroup cg = cloneObj.GetComponent<CanvasGroup>();
            if (cg == null) cg = cloneObj.AddComponent<CanvasGroup>();
            cg.alpha = 0.6f;
            cg.blocksRaycasts = false;
        }

        #region D&D機能
        // 押した瞬間
        public void OnPointerDown(PointerEventData eventData)
        {
            isHolding = true;
            holdTimer = 0f;
        }

        // 離した瞬間
        public void OnPointerUp(PointerEventData eventData)
        {
            ResetState();
        }

        // ドラック初めの瞬間
        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging || cloneRect == null || cloneRect.Equals(null)) return;

            // マウス追従
            cloneRect.anchoredPosition += eventData.delta / ui_system.DragCanvas.scaleFactor;
        }

        // ドラッグ離した瞬間
        public void OnEndDrag(PointerEventData eventData)
        {
            UI_System.IsDrag = false;
            //Debug.Log("投下！！！");
            // if (!isDragging) return;

            if (cloneObj != null)
            {
                Destroy(cloneObj);
                cloneObj = null;
                cloneRect = null;
            }

            ResetState();
        }
        #endregion

        // ステータス初期化
        void ResetState()
        {
            isHolding = false;
            isDragging = false;
            holdTimer = 0f;

            if (cloneObj != null)
            {
                Destroy(cloneObj);
                cloneObj = null;
                cloneRect = null;
            }
        }
    }
}
