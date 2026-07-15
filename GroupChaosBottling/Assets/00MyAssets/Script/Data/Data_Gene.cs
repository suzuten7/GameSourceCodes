using UnityEngine;
using static Manifesto;

[CreateAssetMenu(menuName ="DataCre/Gene")]
public class Data_Gene : ScriptableObject
{
    [EnumIndex(typeof(Enum_GeneFormat))]
    public Texture[] Images;
    [TextArea]
    public string Info;
    [TextArea]
    public string Set2;
    [TextArea]
    public string Set4;
}
