using UnityEngine;
[CreateAssetMenu(fileName = "DataBuf", menuName = "ScriptableObjects/Buf")]
public class Data_Buf : ScriptableObject
{
    [Tooltip("タイプ")] public BufType type;
    [Tooltip("名前")] public string name;
    [Tooltip("説明"),TextArea(1,5)]public string info;
    [Tooltip("アイコン画像")] public Texture icon;
    [Tooltip("アイコン色")] public Color iconColor = Color.white;
    [Tooltip("背景色")] public Color col;
    [Tooltip("効果値")] public float[] values;
}
