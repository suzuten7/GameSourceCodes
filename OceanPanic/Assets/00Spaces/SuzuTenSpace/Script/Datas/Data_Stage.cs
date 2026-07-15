using UnityEngine;
[CreateAssetMenu(menuName ="DataCre/Stage")]
public class Data_Stage : ScriptableObject
{
    public string Name;
    public Texture StageImage;
    public int SceneID;
    public bool NoRandoms;
}
