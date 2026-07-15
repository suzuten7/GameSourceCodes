using UnityEngine;
using UnityEngine.UI;

/* 内容
 * ・色の簡易変換
 */

public class UI_ColorApply : MonoBehaviour
{
    [SerializeField, Tooltip("Defaultの場合はfalse")]
    bool custom_Flag = true;
    public Image image;

    void Awake()
    {
        UI_ColorPalletManager cpm = UI_ColorPalletManager.manager;
        if (cpm == null) return;

        if (!custom_Flag) return;
        image.color = cpm.after_ColorImage.color;
    }

    /// <summary>
    /// 色の適応
    /// </summary>
    public void SetColor()
    {
        UI_ColorPalletManager cpm = UI_ColorPalletManager.manager;
        if (cpm == null) return;

        cpm.ApplyColor(image.color);
    }
}
