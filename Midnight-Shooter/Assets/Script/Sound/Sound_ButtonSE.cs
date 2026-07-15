using UnityEngine;
using UnityEngine.EventSystems;

/* 内容
 * ボタンにカーソルが合わさった時にSEを流す
 * ボタンをクリックしたときにSEを流す
 */

public class Sound_ButtonSE : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    #region 変数倉庫
    [Header("◆メインオプション")]
    [SerializeField] AudioClip selectSE;
    [SerializeField, Tooltip("音量"), Range(0, 1)] float select_Volume = 1f;
    [SerializeField, Tooltip("ピッチ"), Range(-3, 3)] float select_Pitch = 1f;

    [SerializeField] AudioClip clickSE;
    [SerializeField, Tooltip("音量"), Range(0, 1)] float click_Volume = 1f;
    [SerializeField, Tooltip("ピッチ"), Range(-3, 3)] float click_Pitch = 1f;
    #endregion

    /// <summary>
    /// ボタンにカーソルが乗った時
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    { if (selectSE != null)
        { Library_Sound.Sound_SystemSEPlay(selectSE, select_Volume, select_Pitch); }}

    /// <summary>
    /// ボタンをクリックした時
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    { if (clickSE != null)
        { Library_Sound.Sound_SystemSEPlay(clickSE, click_Volume, click_Pitch); }}
}
