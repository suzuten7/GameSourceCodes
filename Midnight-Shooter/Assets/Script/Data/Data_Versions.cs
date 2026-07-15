using UnityEngine;
[CreateAssetMenu(fileName = "Versions", menuName = "ScriptableObjects/Versions")]
public class Data_Versions : ScriptableObject
{
    public string version;
    public string date;
    [TextArea(1, 100)] public string info;
}
