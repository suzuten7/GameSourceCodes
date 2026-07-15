using UnityEngine;
using UnityEngine.UI;

/* 内容
 * ・UIの 表示 / 非表示 切り替え
 */

[System.Serializable]
public class SetImage
{
    [SerializeField, Tooltip("切り替える画像")]
    public Sprite set_Sprite;

    [SerializeField, Tooltip("この画像の透明度"), Range(0, 1)]
    public float set_Alpha = 1.0f;
}

public class UI_HideChange : MonoBehaviour
{
    #region 変数倉庫
    [Header("◆メインオプション")]
    [SerializeField, Tooltip("表示 / 非表示 を切り替えるオブジェクト")]
    GameObject hide_ChangeObj;
    [SerializeField, Tooltip("表示 / 非表示 を切り替えるボタン画像")]
    Image hide_ButtonImage;
    [SerializeField, Tooltip("表示時のボタン画像など")]
    SetImage noHide;
    [SerializeField, Tooltip("非表示時のボタン画像など")]
    SetImage nowhide;
    bool nowHideFlag = false;
    #endregion

    void Awake()
    {
        nowHideFlag = false;

        //画像変更初期化
        hide_ButtonImage.sprite = noHide.set_Sprite;

        //透明度初期化
        Color color = hide_ButtonImage.color;
        color.a = noHide.set_Alpha;
        hide_ButtonImage.color = color;
    }

    /// <summary>
    /// UI透過の切り替えボタン
    /// </summary>
    public void UI_Hide()
    {
        nowHideFlag = !nowHideFlag;

        //アクティブの切り替え
        hide_ChangeObj.SetActive(!nowHideFlag);

        SetImage data = nowHideFlag ? nowhide : noHide;

        //画像変更
        hide_ButtonImage.sprite = data.set_Sprite;

        //透明度変更
        Color color = hide_ButtonImage.color;
        color.a = data.set_Alpha;
        hide_ButtonImage.color = color;
    }
}
