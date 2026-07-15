using UnityEngine;
[CreateAssetMenu(fileName = "DataCharaImg", menuName = "ScriptableObjects/CharaImg")]
public class Data_CharaImg : ScriptableObject
{
    [Tooltip("名前")] public string name;
    [TextArea,Tooltip("説明")]public string info;
    [Tooltip("アイコン")] public Texture icon;
    [Tooltip("アイコン色")] public Color iconColor;
    [Tooltip("外見オブジェクト")] public GameObject imgObj;
    [Tooltip("チーム色変更無し")] public bool colorNoChange;
}
