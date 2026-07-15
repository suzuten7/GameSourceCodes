using UnityEngine;
using UnityEngine.UI;
using static DataColors;
public class UIChange : MonoBehaviour
{
    [Tooltip("切り替えUIオブジェクト"), SerializeField] GameObject[] UIs;
    [Tooltip("ボタンイメージ"), SerializeField] Image[] ButtonUIs;
    public int UIID;

    void LateUpdate()
    {
        for (int i = 0; i < UIs.Length; i++) UIs[i].SetActive(i == UIID);
        for(int i = 0; i < ButtonUIs.Length; i++)
        {
            ButtonUIs[i].color = DCol.ColSelects(i == UIID);
        }
    }
    public void Change(int ID)
    {
        UIID = ID;
    }
}
