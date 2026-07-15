using System.Collections.Generic;
using UnityEngine;
using static Manifesto;

[CreateAssetMenu(menuName ="DataCre/Passive")]
public class Data_Passive : ScriptableObject
{
    public string Name;
    [TextArea]public string Info;
    public Texture Icon;
    [Tooltip("フィルター")] public List<Enum_PassiveFilter> Filters;
}
