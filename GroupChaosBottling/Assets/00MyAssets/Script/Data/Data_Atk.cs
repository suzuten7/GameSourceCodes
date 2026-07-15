using System.Collections.Generic;
using UnityEngine;
using static Manifesto;
using static Calculation;
using System.Linq;
using static Photon.Pun.UtilityScripts.PunTeams;
using Unity.VisualScripting;

[CreateAssetMenu(menuName ="DataCre/Atk")]
public class Data_Atk : ScriptableObject
{

    public string Name;
    [TextArea]public string Info;
    public Texture Icon;
    [Tooltip("タイプ")] public Enum_AtkType AtkType;
    [SerializeField,Tooltip("フィルター自動設定")]bool FilterSet;
    [Tooltip("フィルター")] public List<Enum_AtkFilter> Filters;
    [Tooltip("終了時間(フレーム)")] public int EndTime;
    [Tooltip("CT(秒)")] public float CT;
    [Tooltip("SP消費")] public int SPUse;

    [Header("分岐情報")]public List<Class_Atk_BranchInfo> BranchInfos;
    [Header("分岐先")] public Class_Atk_Branch[] Branchs;
    [Header("制限")] public Class_Atk_Fixed[] Fixeds;
    [Header("弾発射")] public Class_Atk_Shot_Base[] Shots;
    [Header("移動")] public Class_Atk_Move[] Moves;
    [Header("ステータス変化")] public Class_Atk_State[] States;
    [Header("状態変化")] public Class_Atk_Buf[] Bufs;
    [Header("武器表示")] public Class_Atk_WeponSet[] WeponSets;
    [Header("アニメーション")] public Class_Atk_Anim[] Anims;
    [Header("効果音再生")] public Class_Atk_SEPlay[] SEPlays;



    public string InfoGets()
    {
        string Str = "";
        if (Info != "") Str = "\n" + Info;
        if (Str != "") Str += "\n";
        Str += "<color=#888888>CT" + CT+"秒</color>";
        Str += "\n<color=#008888>攻撃時間" + EndTime + "f</color>";
        if (SPUse > 0)Str += "\n<color=#FFFF00>SP" + SPUse+ "</color>";
        Str += "<size=70%>";
        if (BranchInfos.Count > 0)
        {
            for (int i = 0; i < BranchInfos.Count; i++)
            {
                var BInfo = BranchInfos[i];
                Str += "\n<size=100%>" + BInfo.Name+"</size>";
                Str += "\n" + InfoGetBranchs(BInfo.BID);
            }
        }
        else
        {
            Str += "\n" + InfoGetBranchs(0);
        }
        Str += "</size>";
        while (true)
        {
            if (Str.Contains("\n\n"))
            {
                Str = Str.Replace("\n\n", "\n");
            }
            else break;
        }
        return Str;
    }
    public string InfoGetBranchs(int BID)
    {
        var OStr = "";

        for (int j = 0; j < Shots.Length; j++)
        {
            var Shot = Shots[j];
            if (OStr != "") OStr += "\n";
            OStr += Shot.OtherStrGet(BID,false);
        }


        for (int j = 0; j < States.Length; j++)
        {
            var State = States[j];
            if (State.BranchNum >= 0 && State.BranchNum != BID) continue;
            if (OStr != "") OStr += "\n";
            OStr += "<color=#88FF88>";
            OStr += "(自身" + State.State.ToString()+")";
            OStr += CalStr(State.Adds,true);
            OStr += "</color>";
        }


        for (int j = 0; j < Bufs.Length; j++)
        {
            var Buf = Bufs[j];
            if (Buf.BranchNum >= 0 && Buf.BranchNum != BID) continue;
            if (OStr != "") OStr += "\n";
            OStr += "<color=#FF88FF>";
            OStr += "(自身状態変化)";
            for (int k = 0; k < Buf.BufSets.Length; k++) OStr += "\n" + Buf.BufSets[k].InfoStr(false);
            OStr += "</color>";
        }
        return OStr;
    }

    public string InfoDams(int BTime, int BDam, int BHit, float BBreak, int BHeal)
    {
        var Str = "Times:" + BTime;
        if (BDam > 0)
        {
            Str += "\nDam:" + BDam;
            Str += "\nDPS:" + (BDam / (float)BTime * 60).ToString("F0");
            Str += "\nHit:" + BHit;
            Str += "\nHPS:" + (BHit / (float)BTime * 60).ToString("F2");
            Str += "\nBreak:" + BBreak;
            Str += "\nBPS:" + (BBreak / (float)BTime * 60).ToString("F2");
        }
        if (BHeal > 0)
        {
            Str += "\nHeal:" + BHeal;
            Str += "\nHPS:" + (BHeal / (float)BTime * 60).ToString("F0");
        }
        return Str;
    }
    public void DamGets(int BID,int HTime,int STime,out int Dam,out int Hit, out float Break, out int Heal)
    {
        DGetd(Shots, BID,HTime, STime, out var oDam, out var oHit, out var oBreak, out var oHeal);
        Dam = oDam;
        Hit = oHit;
        Break = oBreak;
        Heal = oHeal;
    }
    void DGetd(Class_Atk_Shot_Base[] Shots, int BID, int HTime, int STime, out int Dam, out int Hit, out float Break, out int Heal)
    {
        Dam = 0;
        Hit = 0;
        Break = 0;
        Heal = 0;
        for (int i = 0; i < Shots.Length; i++)
        {
            var SD = Shots[i];
            var MulHitCo = 0;
            int HitCo = 1;
            int HitS = 0;
            if (SD.HitCT > 0) HitCo = Mathf.CeilToInt((float)HTime / SD.HitCT);
            for (int j = 0; j < SD.Fires.Length; j++)
            {
                var SFD = SD.Fires[j];
                if (SFD.BranchNum >= 0 && SFD.BranchNum != BID) continue;
                var TRange = SFD.Times.y - Mathf.Max(STime, SFD.Times.x);
                if (TRange >= 0) HitS += SFD.Count * ((TRange / Mathf.Max(1, SFD.Times.z)) + 1);
            }
            for (int j = 0; j < SD.Hits.Length; j++)
            {
                var SHD = SD.Hits[j];
                if (SHD.BranchNum >= 0 && SHD.BranchNum != BID) continue;
                var CVal = (float)Cal(SHD.DamCalc, DataBase.DB.Player, DataBase.DB.Player,true);
                for(int k= 0; k < HitS * HitCo; k++)
                {
                    var MultDam = Mathf.Clamp(100f + SHD.MultDamChange.x * MulHitCo, Mathf.Min(100f, SHD.MultDamChange.y), Mathf.Max(100f, SHD.MultDamChange.y)) * 0.01f;
                    if (!SHD.Heals && SHD.EHit)
                    {
                        Dam += Mathf.RoundToInt(MultDam * CVal);
                        if(Dam>0)Hit++;
                        Break += MultDam * SHD.BreakValue;
                    }
                    if (SHD.Heals && (SHD.FHit || SHD.MHit))
                    {
                        Heal += Mathf.RoundToInt(MultDam * CVal);
                    }
                    MulHitCo++;
                }

            }
            for(int  j = 0; j < SD.Adds.Length; j++)
            {
                var SAD = SD.Adds[j];
                if (SAD.BranchNum >= 0 && SAD.BranchNum != BID) continue;
                for(int k = 0; k < SAD.AddShots.Length; k++)
                {
                    DGetd(SAD.AddShots[k].Shots, BID,HTime, 0, out var oDam, out var oHit, out var oBreak, out var oHeal);
                    Dam += HitS * oDam;
                    Hit += HitS * oHit;
                    Break += HitS * oBreak;
                    Heal += HitS * oHeal;
                }
            }
        }
    }
    private void OnValidate()
    {
        if(Fixeds!=null)
        for (int i = 0; i < Fixeds.Length; i++)
        {
            var Fixed = Fixeds[i];
            Fixed.EditDisp = "[" + i + "]";
            Fixed.EditDisp += "BNum:" + Fixed.BranchNum;
            Fixed.EditDisp += "Time:" + Fixed.Times;

        }
        if (Branchs != null)
            for (int i = 0; i < Branchs.Length; i++)
            {
                var Branch = Branchs[i];
                Branch.EditDisp = "[" + i + "]";
                Branch.EditDisp += "BNum:";
                for (int j = 0; j < Branch.BranchNums.Length; j++)
                {
                    if (j > 0) Branch.EditDisp += ",";
                    Branch.EditDisp += Branch.BranchNums[j];
                }
                Branch.EditDisp += "Time:" + Branch.Times;
                Branch.EditDisp += "MP:" + Branch.UseMP;
                Branch.EditDisp += "Future{Num:" + Branch.FutureNum;
                Branch.EditDisp += "Time:" + Branch.FutureTime + "}";
            }
        if (Shots != null)
        for (int i = 0; i < Shots.Length; i++) Shots[i].EditDispSet();
        if (Bufs != null)
        {
            for(int i = 0; i < Bufs.Length; i++)
            {
                var Buf = Bufs[i];
                for (int j = 0; j < Buf.BufSets.Length; j++)
                {
                    Buf.BufSets[j].EditDispSet();
                }
            }
        }
        if (Anims != null)
        for (int i = 0; i < Anims.Length; i++)
        {
            var Anim = Anims[i];
            Anim.EditDisp = "[" + i + "]";
            Anim.EditDisp += "BNum:" + Anim.BranchNum;
            Anim.EditDisp += "Time:" + Anim.Times;
            Anim.EditDisp += "ID:" + Anim.ID;
        }

        if (FilterSet)
        {
            FilterSet = false;
            FilterAutos();
        }
    }
    void FilterAutos()
    {
        Filters.Clear();
        Filter_AShot(Shots);
        for(int i = 0; i < Fixeds.Length; i++)
        {
            var Fix = Fixeds[i];
            if (Fix.SpeedRem < 0) Filters.Add(Enum_AtkFilter.移動);
            if (Fix.Aiming) Filters.Add(Enum_AtkFilter.照準);
        }
        for (int i = 0; i < Moves.Length; i++)
        {
            var Move = Moves[i];
            var MVect = Move.Vect;
            MVect.y = Mathf.Max(0, MVect.y);
            if (MVect.magnitude>=1f) Filters.Add(Enum_AtkFilter.移動);
        }
        for (int i = 0; i < States.Length; i++)
        {
            Filters.Add(Enum_AtkFilter.自己);
            if (States[i].State == Enum_State.回復) Filters.Add(Enum_AtkFilter.回復);
        }
        for (int i = 0; i < Bufs.Length; i++)
        {
            Filters.Add(Enum_AtkFilter.自己);
            for(int j = 0; j < Bufs[i].BufSets.Length; j++) Filter_Buf(Bufs[i].BufSets[j].Buf);
        }

        Filters = Filters.Distinct().ToList();
        if (Filters.Count <= 0) Filters.Add(Enum_AtkFilter.特殊);
    }
    void Filter_AShot(Class_Atk_Shot_Base[] AShots)
    {
        for (int i = 0; i < AShots.Length; i++)
        {
            var Shot = AShots[i];

            for (int j = 0; j < Shot.Hits.Length; j++)
            {
                var Hit = Shot.Hits[j];
                if(Hit.DamCalc != "" && !Hit.Heals)
                {
                    Filters.Add(Enum_AtkFilter.攻撃);
                    if (Hit.ShortAtk) Filters.Add(Enum_AtkFilter.近距離);
                    if (!Hit.ShortAtk) Filters.Add(Enum_AtkFilter.遠距離);
                }
                if (Hit.DamCalc != "" && Hit.Heals) Filters.Add(Enum_AtkFilter.回復);


                for (int k = 0; k < Hit.BufSets.Length; k++)
                {
                    Filter_Buf(Hit.BufSets[k].Buf);
                }


                if (Hit.FHit) Filters.Add(Enum_AtkFilter.味方);
                if (Hit.MHit) Filters.Add(Enum_AtkFilter.自己);
            }
            for (int j = 0; j < Shot.Fires.Length; j++)
            {
                var Fire = Shot.Fires[j];
                if(Fire.Count > 1) Filters.Add(Enum_AtkFilter.複数);
                if (Fire.Times.y > Fire.Times.x) Filters.Add(Enum_AtkFilter.高頻度);
            }
            if (Shot.HitCT > 0) Filters.Add(Enum_AtkFilter.多段);
            if (Shot.Adds != null)
                for (int j = 0; j < Shot.Adds.Length; j++)
                {
                    for (int k = 0; k < Shot.Adds[j].AddShots.Length; k++)
                    {
                        Filters.Add(Enum_AtkFilter.追加攻撃);
                        Filter_AShot(Shot.Adds[j].AddShots[k].Shots);
                    }
                }
            if (Shot.Summon != null && Shot.Summon.LimitTime > 0) Filters.Add(Enum_AtkFilter.召喚);
        }
    }
    void Filter_Buf(Enum_Bufs Buf)
    {
        bool UBuf = false;
        bool DBuf = false;
        bool DamAdd = false;
        bool DefAdd = false;
        if (Buf == Enum_Bufs.HP増加)
        {
            UBuf = true;
            DefAdd = true;
        }
        if (Buf == Enum_Bufs.攻撃増加)
        {
            UBuf = true;
            DamAdd = true;
        }
        if (Buf == Enum_Bufs.防御増加)
        {
            UBuf = true;
            DefAdd = true;
        }
        if (Buf == Enum_Bufs.バリア)
        {
            UBuf = true;
            DefAdd = true;
        }
        if (Buf == Enum_Bufs.シールド)
        {
            UBuf = true;
            DefAdd = true;
        }
        if (Buf == Enum_Bufs.与ダメージ増加)
        {
            UBuf = true;
            DamAdd = true;
        }
        if (Buf == Enum_Bufs.攻撃低下) DBuf = true;
        if (Buf == Enum_Bufs.防御低下) DBuf = true;
        if (Buf == Enum_Bufs.毒) DBuf = true;
        if (Buf == Enum_Bufs.標的固定) DBuf = true;

        if (UBuf) Filters.Add(Enum_AtkFilter.バフ);
        if (DBuf) Filters.Add(Enum_AtkFilter.デバフ);

        if (DamAdd) Filters.Add(Enum_AtkFilter.攻撃強化);
        if (DefAdd) Filters.Add(Enum_AtkFilter.防御強化);
    }
}
