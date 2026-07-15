using UnityEngine;

/* 内容
 * ・カラーパレットの数字変換
 * ├Hex    ：16進数
 * └Percent：パーセント
 */

public class UI_HexNotation : MonoBehaviour
{
    UI_ColorPalletManager cpm;

    void Awake()
    {
        cpm = UI_ColorPalletManager.manager;

        //保存値を取得
        cpm.cp_Dorpdown.value = Library_SaveFiles.LoadFileInt("Color","HexNotation", 0);
        cpm.set_cpNotation = (HexNotation)cpm.cp_Dorpdown.value;

        //イベント登録
        cpm.cp_Dorpdown.onValueChanged.AddListener(OnChanged);
    }

    void OnChanged(int index)
    {
        //enumへ変換
        cpm.set_cpNotation = (HexNotation)index;

        //保存
        Library_SaveFiles.SaveFile("Color","HexNotation", index.ToString());
    }
}
