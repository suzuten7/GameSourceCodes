using UnityEngine;
using UnityEngine.UI;

public class UI_AlphaOptionSet : MonoBehaviour
{
    [SerializeField] int ID;
    [SerializeField] float alphaBase;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] MaskableGraphic img;
    private void Update()
    {
        var key = "UI_Option " + ID.ToString("D2");
        var alpha = UI_OptionManager.OptionGetFloat(key, alphaBase) *0.01f;
        if(canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
        if(img != null)
        {
            var col = img.color;
            col.a = alpha;
            img.color = col;
        }
    }
}
