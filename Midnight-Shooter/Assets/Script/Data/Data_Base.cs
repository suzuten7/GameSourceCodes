using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBase", menuName = "ScriptableObjects/DataBase")]
public class Data_Base : ScriptableObject
{
    static public Data_Base DB;
    public Player_Manager basePlayer;
    public Player_Marker marker;

    public List<Data_Versions> versions;
    public List<Data_Helps> helps;

    public List<Data_Stage> stages;
    public List<Data_Gun> guns;
    public List<Data_Gun> melles;
    public List<Data_Gadget> gadgets;
    public List<Data_Ult> ults;
    [Tooltip("必殺チャージ量\n[0]時間経過(秒)\n[1]与ダメ(%)\n[2]被ダメ(%)\n[3]回避判定(秒)\n[4]キル")]public float[] ultCharge;
    public List<Data_CharaImg> charaImgs;
    public Player_ImgAnims imgAnimObj;
    public List<Data_Reticles> reticles;
    public Player_ImgAnims reticleLoadsObj;
    public int CostMax;
    public List<Data_Passive> passives;
    static public Data_Passive PassiveDGet(Passive pass)
    {
        return DB.passives.Find(x => x.passive == pass);
    }

    public List<Data_Buf> bufs;

    public Data_MoveType moveTypeBase;
    public Data_MoveType moveTypeFly;
    public List<Data_MoveType> moveTypes;

    public Gradient disGrad;
    public Obj_Sound soundObj;
    public List<AudioClip> ses;
    public Color[] teamColors;
    public AnimationCurve aiBackRotMult;
    public float aiDisPow;
    static public Color TeamColorGet(int team)
    {
        if(team >= 0 && team < DB.teamColors.Length)return DB.teamColors[team];
        return Color.white;
    }
    static public Data_Buf BufDGet(BufType buf)
    {
        return DB.bufs.Find(x => x.type == buf);
    }
    static public string DisInfoStr(AnimationCurve disMult,float disMax)
    {
        float dmin = float.MaxValue;
        float dmax = float.MinValue;
        foreach (var key in disMult.keys)
        {
            if (key.value < dmin) dmin = key.value;
            if (key.value > dmax) dmax = key.value;
        }
        var infostr = $"{LocalizSystem.LocailzSCInfo("距離倍率")}{dmin}～{dmax}{LocalizSystem.LocailzSCInfo("倍")}({disMax}m)";
        infostr += "\n";
        for (int i = 0; i < 20; i++)
        {
            float f = disMult.Evaluate(i / 20f);
            var col = Color.green;
            if (f < 1)
            {
                float p = Mathf.InverseLerp(dmin, 1f, f);
                col = Data_Base.DB.disGrad.Evaluate(p / 2f);
            }
            else
            {
                float p = Mathf.InverseLerp(1f, dmax, f);
                col = Data_Base.DB.disGrad.Evaluate(p / 2f + 0.5f);
            }
            var ccode = ColorUtility.ToHtmlStringRGB(col);
            infostr += "<color=#" + ccode + ">|" + "</color>";
        }
        return infostr;
    }
    static public string BufInfoStr(BufAdd[] bufAdds)
    {
        var infostr = LocalizSystem.LocailzSCInfo("バフ付与");
        for(int i = 0; i < bufAdds.Length; i++)
        {
            var bufd = BufDGet(bufAdds[i].buf);
            if (bufAdds.Length > 1) infostr += "\n";
            infostr += $"[{(bufd != null ? LocalizSystem.LocailzString("BufName",bufd.name) : bufAdds[i].buf.ToString())}]({bufAdds[i].time}{LocalizSystem.LocailzSCInfo("秒")})";
        }
        return infostr;
    }

}
