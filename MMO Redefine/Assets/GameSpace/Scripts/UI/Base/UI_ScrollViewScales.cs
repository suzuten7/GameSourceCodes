namespace UIs
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_ScrollViewScales : MonoBehaviour
    {
        [SerializeField] ScrollRect ScRect;
        [SerializeField] Slider ScaleSlider;
        float BackScale = -1f;
        private void Update()
        {
            if (BackScale != ScaleSlider.value)
            {
                BackScale = ScaleSlider.value;
                ZoomAtCurrentCenter(ScRect, ScaleSlider.value);
            }
        }
        public void ZoomAtCurrentCenter(ScrollRect scrollRect, float newScale)
        {
            RectTransform content = scrollRect.content;
            RectTransform viewport = scrollRect.viewport;

            // 1. Viewport の中心（ローカル座標）
            Vector2 viewportLocalCenter = viewport.rect.center;

            // 2. Viewportローカル座標 → Contentローカル座標 へ変換
            Vector2 worldPoint = viewport.TransformPoint(viewportLocalCenter);
            Vector2 beforeLocalPoint = content.InverseTransformPoint(worldPoint);

            // 3. ズーム適用
            content.localScale = new Vector3(newScale, newScale, 1);

            Canvas.ForceUpdateCanvases();

            // 4. ズーム後の同じローカル座標点を再計算
            Vector2 afterWorldPoint = content.TransformPoint(beforeLocalPoint);

            // 5. Viewport中心に同じ地点を合わせるための補正
            Vector2 diff = (Vector2)viewport.TransformPoint(viewportLocalCenter) - afterWorldPoint;

            content.anchoredPosition += diff;
        }

        public void SetPosCenter()
        {
            ScRect.horizontalScrollbar.value = 0.5f;
            ScRect.verticalScrollbar.value = 0.5f;
        }
    }
}
