using UnityEngine;
using UnityEngine.UI;

/* 内容
 * ボタンを押したらオブジェクトを【畳む / 開く】
 */

public class UI_Tab : MonoBehaviour
{
    public GameObject mainPanel;

    /// <summary>
    /// オブジェクトを【畳む / 開く】
    /// </summary>
    public void Toggle()
    {
        mainPanel.SetActive(!mainPanel.activeSelf);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            transform.parent as RectTransform);
    }
}
