
namespace UIs
{
    using Pixiv.VroidSdk.Api.DataModel;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_ValueGraph_BrokenLine: Graphic
    {
        public Color negColor;
        public bool MaxPers;
        public Class_LingArray lingVal;
        [System.Serializable]
        public class Class_LingArray
        {
            public float[] values;
            public int head;
            public void ValueAdd(float val)
            {
                values[(int)Mathf.Repeat(head - 1, values.Length)] += val;
            }
            public void StepAdd(float val)
            {
                values[head] = val;
                head = (head + 1) % values.Length;
            }
            public void Set(float[] vals)
            {
                values = vals;
                head = 0;
            }
            public float GetValue(int index)
            {
                return values[(index + head) % values.Length];
            }
            public float GetTotal()
            {
                var val = 0f;
                for (int i = 0; i < values.Length; i++)
                {
                    val += values[i];
                }
                return val;
            }
            public float GetMax()
            {
                var vmax = 0f;
                for (int i = 0; i < values.Length; i++)
                {
                    vmax = Mathf.Max(vmax, values[i]);
                }
                return vmax;
            }
        }



        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (lingVal.values.Length < 2) return;

            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;


            var vmax = !MaxPers ? 1f : Mathf.Max(1, lingVal.GetMax());

            for (int i = 0; i < lingVal.values.Length - 1; i++)
            {
                var v1 = lingVal.GetValue(i) / vmax;
                var v2 = lingVal.GetValue(i + 1) / vmax;

                float x0 = i / (float)(lingVal.values.Length - 1) * width;
                float y0 = Mathf.Clamp01(Mathf.Abs(v1))   * height;
                float x1 = (i + 1) / (float)(lingVal.values.Length - 1) * width;
                float y1 = Mathf.Clamp01(Mathf.Abs(v2)) * height;

                DrawLine(vh, new Vector2(x0, y0), new Vector2(x1, y1), v2<=0);
            }
        }

        void DrawLine(VertexHelper vh, Vector2 p0, Vector2 p1,bool neg)
        {
            var col = !neg ? color : negColor;

            float thickness = 2f;
            Vector2 dir = (p1 - p0).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x) * thickness;

            int index = vh.currentVertCount;



            vh.AddVert(p0 - normal, col, Vector2.zero);
            vh.AddVert(p0 + normal, col, Vector2.zero);
            vh.AddVert(p1 + normal, col, Vector2.zero);
            vh.AddVert(p1 - normal, col, Vector2.zero);

            vh.AddTriangle(index, index + 1, index + 2);
            vh.AddTriangle(index, index + 2, index + 3);
        }
    }
}

