using UnityEngine;
using UnityEngine.UI;

/* 内容
 * ・背景色変更(グラデーションループ)
 */

public class nime_Gradation : MonoBehaviour
{
    #region 変数倉庫
    #region メインオプション
    [Header("◆メインオプション")]
    [SerializeField] Image gradation_BG;
    [SerializeField, Tooltip("色のグラデーション")]
    Gradient gradation_Color;
    [SerializeField, Tooltip("グラデーションの速度")]
    float gradation_Speed = 0.1f;

    float timer = 0;
    #endregion
    #endregion

    void Update()
    {
        timer += Time.deltaTime * gradation_Speed;
        Color pauseColor = gradation_Color.Evaluate(timer);
        gradation_BG.fillAmount = timer;
        gradation_BG.color = pauseColor;

        if (timer >= 1) { timer = 0; }
    }
}
