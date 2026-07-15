using UnityEngine;
[CreateAssetMenu(fileName = "Helps", menuName = "ScriptableObjects/Helps")]
public class Data_Helps : ScriptableObject
{
    public string title;
    [TextArea(1, 100)]public string info;
}
