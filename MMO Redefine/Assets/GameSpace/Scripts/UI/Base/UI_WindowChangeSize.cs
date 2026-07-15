namespace UIs
{

    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UI_WindowChangeSize : MonoBehaviour, IDragHandler, IBeginDragHandler
    {

        [Header("◆メインオプション")]
        [SerializeField] UI_WindowSystem window;
        [SerializeField] float borderThickness = 10f;

        Vector2 startMousePosScreen;
        Vector2 startSize;
        Vector2 startPos;
        ResizeDirection currentDir;

        enum ResizeDirection
        {
            None, Left, Right, Top, Bottom,
            TopLeft, TopRight, BottomLeft, BottomRight
        }

        // ドラック開始時(実質：Start)
        public void OnBeginDrag(PointerEventData eventData)
        {
            startMousePosScreen = eventData.position;
            startSize = window.rtrans.sizeDelta;
            startPos = window.rtrans.anchoredPosition;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(window.rtrans, eventData.position, eventData.pressEventCamera, out localPoint);
            currentDir = GetResizeDirection(localPoint);
            window.FrontSet();
        }

        // ドラック中(実質：Update)
        public void OnDrag(PointerEventData eventData)
        {
            if (currentDir == ResizeDirection.None) return;

            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 clampedMousePos = new Vector2(
                Mathf.Clamp(eventData.position.x, 0, screenSize.x),
                Mathf.Clamp(eventData.position.y, 0, screenSize.y)
            );
            Vector2 mouseDelta = (clampedMousePos - startMousePosScreen) / window.Canvas.scaleFactor;
            Vector2 newSize = startSize;
            Vector2 newPos = startPos;
            Vector2 originalSize = startSize;

            Vector2 pivot = window.rtrans.pivot;

            // 横方向
            if (currentDir == ResizeDirection.Left ||
                currentDir == ResizeDirection.TopLeft ||
                currentDir == ResizeDirection.BottomLeft)
            {
                newSize.x -= mouseDelta.x;
                newSize.x = Mathf.Clamp(newSize.x, window.minSize.x, window.maxSize.x);

                float actual = originalSize.x - newSize.x;
                newPos.x += actual * (1 - pivot.x);
            }
            else if (currentDir == ResizeDirection.Right ||
                currentDir == ResizeDirection.TopRight ||
                currentDir == ResizeDirection.BottomRight)
            {
                newSize.x += mouseDelta.x;
                newSize.x = Mathf.Clamp(newSize.x, window.minSize.x, window.maxSize.x);

                float actual = newSize.x - originalSize.x;
                newPos.x += actual * (pivot.x);
            }

            // 縦方向
            if (currentDir == ResizeDirection.Top ||
                currentDir == ResizeDirection.TopLeft ||
                currentDir == ResizeDirection.TopRight)
            {
                newSize.y += mouseDelta.y;
                newSize.y = Mathf.Clamp(newSize.y, window.minSize.y, window.maxSize.y);

                float actual = newSize.y - originalSize.y;
                newPos.y += actual * pivot.y;
            }
            else if (currentDir == ResizeDirection.Bottom ||
                currentDir == ResizeDirection.BottomLeft ||
                currentDir == ResizeDirection.BottomRight)
            {
                newSize.y -= mouseDelta.y;
                newSize.y = Mathf.Clamp(newSize.y, window.minSize.y, window.maxSize.y);

                float actual = originalSize.y - newSize.y;
                newPos.y += actual * (1 - pivot.y);
            }

            window.rtrans.sizeDelta = newSize;
            window.rtrans.anchoredPosition = newPos;
        }

        // ドラッグしている端の取得
        ResizeDirection GetResizeDirection(Vector2 localPos)
        {
            Rect rect = window.rtrans.rect;
            bool left = localPos.x < rect.xMin + borderThickness;
            bool right = localPos.x > rect.xMax - borderThickness;
            bool top = localPos.y > rect.yMax - borderThickness;
            bool bottom = localPos.y < rect.yMin + borderThickness;

            if (left && top) return ResizeDirection.TopLeft;
            if (right && top) return ResizeDirection.TopRight;
            if (left && bottom) return ResizeDirection.BottomLeft;
            if (right && bottom) return ResizeDirection.BottomRight;
            if (left) return ResizeDirection.Left;
            if (right) return ResizeDirection.Right;
            if (top) return ResizeDirection.Top;
            if (bottom) return ResizeDirection.Bottom;

            return ResizeDirection.None;
        }
    }
}
