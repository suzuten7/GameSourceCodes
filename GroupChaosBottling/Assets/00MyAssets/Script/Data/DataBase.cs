using UnityEngine;
using System.Collections.Generic;
using static Manifesto;
[CreateAssetMenu(menuName ="DataCre/DataBase")]
public class DataBase : ScriptableObject
{
    static DataBase DBs;
    static public DataBase DB
    {
        get
        {
            if (DBs == null) DBs = (DataBase)Resources.Load("DataBase");
            return DBs;
        }
    }
    public LayerMask CamLayer;
    public LayerMask HitLayer;
    public DamageObj DamageObjs;
    public Material[] DamageMats;
    public AudioSource SEObj;
    public GameObject[] HitEffects;
    public GameObject[] HealEffects;

    public Class_Base_SEPlay BarriaHitSE;
    public Class_Base_SEPlay ShildHitSE;

    public GameObject BreakEffect;

    public LineRenderer FixTargetLine;

    public State_Base Player;

    public Data_Chara[] Charas;
    public Data_Atk[] N_Atks;
    public Data_Atk[] S_Atks;
    public Data_Atk[] E_Atks;

    [EnumIndex(typeof(Enum_Passive))]
    public Data_Passive[] Passives;
    [EnumIndex(typeof(Enum_AddAtk))]
    public Data_AddShot[] AddAtks;
    [EnumIndex(typeof(Enum_GeneTypes))]
    public Data_Gene[] Genes;
    public Class_Tables[] GeneDropTables;

    public List<GameObject> Wepons;
    public List<AudioClip> SEs;
    [EnumIndex(typeof(Enum_Stage))]
    public List<Data_Stage> Stages;
    public List<Data_Buf> Bufs;

    private void OnValidate()
    {
        float TPer = 0;
        for(int i = 0; i < GeneDropTables.Length; i++)
        {
            TPer += GeneDropTables[i].P;
        }
        for (int i = 0; i < GeneDropTables.Length; i++)
        {
            var GeneDT = GeneDropTables[i];
            GeneDT.EditDisp = (Enum_GeneOptions)i + "";
            GeneDT.EditDisp += ":" + (GeneDT.P / Mathf.Max(1f, TPer) * 100).ToString("F1") + "%";
            GeneDT.EditDisp += ":×" + GeneDT.Mult;
        }
    }
}
