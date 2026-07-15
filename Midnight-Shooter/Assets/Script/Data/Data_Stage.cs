using UnityEngine;
[CreateAssetMenu(fileName = "StageData", menuName = "ScriptableObjects/Stage")]
public class Data_Stage : ScriptableObject
{
    public string name;
    [TextArea]public string info;
    public Texture icon;
    public Color iconCol;
    public int sceneID;
    public bool randomNo;
}
