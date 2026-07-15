namespace UIs
{
    using UnityEngine;
    using UnityEngine.UI;
    using static GmSystem.GS_SaveValues;
    public class UI_SizeChange : MonoBehaviour
    {
        [SerializeField] RectTransform Rect;
        [SerializeField] CanvasScaler Canvs;
        [SerializeField] int ID;
        const float BaseWidth = 1920;
        const float BaseHight = 1080;
        int backsize = 0;

        void Update()
        {
            var uisize = GetSave_Option.UISizes[ID];
            if(backsize != uisize)
            {
                backsize = uisize;
                if(Canvs != null)Canvs.referenceResolution = new Vector2(BaseWidth / (uisize * 0.01f), BaseHight / (uisize * 0.01f));
                if (Rect != null) Rect.localScale = new Vector3(uisize * 0.01f, uisize * 0.01f, 1);
            }
        }
    }
}

