namespace UIs
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using static GmSystem.GS_SaveValues;
    public class UI_WindowSystem : MonoBehaviour,IPointerDownHandler, IDragHandler, IBeginDragHandler
    {
        public Canvas Canvas
        {
            get
            {
                if (canvas == null)
                {
                    canvas = GetComponentInParent<Canvas>();
                }
                return canvas;
            }
        }
        [SerializeField]Canvas canvas;
        public RectTransform rtrans;
        public Vector2 minSize = new Vector2(100, 100);
        public Vector2 maxSize = new Vector2(20000, 20000);

        Vector2 dragOffset;

        int bsize = -1;
        private void LateUpdate()
        {
            var size = GetSave_Option.UISizes[0];
            if(bsize != size)
            {
                bsize = size;
                UIClamp();
            }
        }
        //最前面
        private void OnEnable()
        {
            FrontSet();
        }
        public void FrontSet()
        {
            rtrans.SetAsLastSibling();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            FrontSet();
        }
        // ドラック開始時(実質：Start)
        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle
                (rtrans, eventData.position, eventData.pressEventCamera, out dragOffset);
            FrontSet();
        }

        // ドラック中(実質：Update)
        public void OnDrag(PointerEventData eventData)
        {
            RectTransform parentRect = rtrans.parent as RectTransform;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            {
                Vector2 targetPos = localPoint - dragOffset;
                rtrans.localPosition = new Vector3(targetPos.x, targetPos.y, rtrans.localPosition.z);
                UIClamp();
            }
            FrontSet();
        }

        // 移動(可動範囲の制限付き)
        void UIClamp()
        {
            RectTransform parentRect = rtrans.parent as RectTransform;

            Vector2 targetPos = rtrans.localPosition;
            Vector2 parentSize = parentRect.rect.size;
            Vector2 selfSize = rtrans.rect.size;

            // 範囲制限
            float clampX = Mathf.Clamp(targetPos.x,
                -parentSize.x / 2 + selfSize.x / 1,
                 parentSize.x / 2 + 0);

            float clampY = Mathf.Clamp(targetPos.y,
                -parentSize.y / 2 + 25,
                 parentSize.y / 2 + 0);

            rtrans.localPosition = new Vector3(clampX, clampY, rtrans.localPosition.z);
        }
    }
}
