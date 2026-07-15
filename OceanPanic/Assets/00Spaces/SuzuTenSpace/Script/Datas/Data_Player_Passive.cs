using UnityEngine;
[CreateAssetMenu(menuName = "DataCre/Passives")]
public class Data_Player_Passive : ScriptableObject
{
    public string Names;
    public DataBase.SkillTypeE SkillColor;
    [TextArea]
    public string Info;
    public PassLVsC[] PassLVs;
    [TextArea]
    public string EndInfo;
    public bool NRand;
    public bool Debug;

    [System.Serializable]
    public class PassLVsC
    {
        public string LVInfo;
        public int Cost;
    }
}
