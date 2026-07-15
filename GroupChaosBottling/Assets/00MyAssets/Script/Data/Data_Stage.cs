using UnityEngine;
using static Manifesto;
[CreateAssetMenu(menuName ="DataCre/Stage")]
public class Data_Stage : ScriptableObject
{
    public string Name;
    [TextArea]public string Info;
    public Texture Icon;
    public int SceneID;

    public int TimeLimSec;
    public int TimeStar;
    public int DeathStar;
    public float DifencePer;
    public bool NoClears;
    [TextArea]
    public string ChaosBuf;
    public Class_Chaos_State[] ChaosStates;
    public Class_Chaos_BufUp[] ChaosBfUps;

    public int ClearChaosGrain;
    public Vector2Int GeneDropCount;
    public Enum_GeneTypes[] GeneDropTypes;

}
