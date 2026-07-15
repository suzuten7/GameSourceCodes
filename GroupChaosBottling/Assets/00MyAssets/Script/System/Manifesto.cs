using UnityEngine;
using System;
using System.Collections.Generic;
using static Manifesto;
using static Calculation;
using NaughtyAttributes;
using System.Globalization;

static public class Manifesto
{
    #region Helpers
    public static string T(string key) => LocalizationManager.Instance != null ? LocalizationManager.Instance.GetText(key) : key;
    public static string ET<T>(T value) where T : Enum => LocalizationManager.Instance != null ? LocalizationManager.Instance.GetEnumText(value) : value.ToString();
    #endregion

    #region Const
    public const string Const_Ttp_BID = "分岐ID条件\n-だと無条件";
    public const string Const_Ttp_Times = "時間条件\n攻撃時間がx～yフレームの間\nzフレーム間隔";
    #endregion

    #region Class
    #region Class_Base
    [System.Serializable]
    public class Class_Base_SEPlay
    {
        [Tooltip("SEファイル")] public AudioClip Clip;
        [Tooltip("音量")] public float Volume = 100f;
        [Tooltip("音程-300～300"), Range(-300f, 300f)] public float Pitch = 100f;
    }
    [System.Serializable]
    public class Class_Base_BufSet
    {
        [HideInInspector] public string EditDisp;
        public Enum_Bufs Buf;
        [Tooltip("状態番号")] public int Index;
        [Tooltip("付与処理")] public Enum_BufSet Set;
        [Tooltip("時間付与フレーム\n0以下だと永続")] public int TimeVal;
        [Tooltip("段階付与式\n0以下だと段階表示なし\n" + TooltipStr), TextArea(1, 3)] public string PowVal;
        [Tooltip("時間上限フレーム\n0以下は上限無し")] public int TimeMax;
        [Tooltip("段階上限上限式\n0以下は上限無し\n" + TooltipStr), TextArea(1, 3)] public string PowMax;
        public string InfoStr(bool AddIf)
        {
            var OStr = "[" + ET(Buf) + "," + ET(Set) + "]\n";
            bool Adds = (Set == Enum_BufSet.付与増加 || Set == Enum_BufSet.不付与増加);
            OStr += T("UNIT_TIME") + ":" + (TimeVal / 60f).ToString("F1");
            if (Adds && TimeMax > 0) OStr += (AddIf ? "<size=30%>" : "<size=50%>") + "Max" + (TimeMax / 60f).ToString("F1") + "</size>";
            OStr += T("UNIT_SEC");
            var PowStr = CalStr(PowVal, true);
            var PowVald = double.TryParse(PowStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var oPVal) ? oPVal : 1;
            if (PowStr != "" && PowVald > 0)
            {
                OStr += "\n" + T("UNIT_LEVEL") + ":" + CalStr(PowVal, true);
                if (Adds)
                {
                    var MaxStr = CalStr(PowMax, true);
                    var MaxVal = double.TryParse(MaxStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var oVal) ? oVal : 1;
                    if (MaxStr != "" && MaxVal > 0) OStr += (AddIf ? "<size=30%>" : "<size=50%>") + "Max" + MaxStr + "</size>";
                }
            }
            return OStr;
        }
        public void EditDispSet()
        {
            EditDisp = ET(Buf);
            EditDisp += "[" + Index;
            EditDisp += "," + ET(Set) + "]";
            var Adds = Set == Enum_BufSet.付与増加 || Set == Enum_BufSet.不付与増加;
            if (TimeVal > 0)
            {
                EditDisp += "(" + T("UNIT_TIME") + ":付与" + TimeVal;
                if (Adds && TimeMax > 0) EditDisp += "Max" + TimeMax;
                EditDisp += ")";
            }
            else
            {
                EditDisp += "(時間無限)";
            }
            var PowStr = CalStr(PowVal, false);
            var PowVald = double.TryParse(PowStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var oPVal) ? oPVal : 1;
            if (PowStr != "" && PowVald > 0)
            {
                EditDisp += "(" + T("UNIT_LEVEL") + ":付与" + CalStr(PowVal, false);
                if (Adds)
                {
                    var MaxStr = CalStr(PowMax, false);
                    var MaxVal = double.TryParse(MaxStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var oVal) ? oVal : 1;
                    if (MaxStr != "" && MaxVal > 0) EditDisp += "Max" + MaxStr;
                }
                EditDisp += ")";
            }
        }
    }
    #endregion
    #region Class_Atk
    [System.Serializable]
    public class Class_Atk_BranchInfo
    {
        [Tooltip(Const_Ttp_BID)] public int BID;
        [Tooltip("分岐名")] public string Name;
        [Tooltip("終了時間変化")] public int ChangeEndTime;
        [Tooltip("非終了")] public bool NoEnd;
    }
    [System.Serializable]
    public class Class_Atk_Branch
    {
        [HideInInspector] public string EditDisp;
        [Tooltip("分岐ID条件いずれか")] public int[] BranchNums;
        [Tooltip(Const_Ttp_Times)] public Vector2Int Times;
        [Tooltip("追加条件")] public Enum_AtkIf[] Ifs;
        [Tooltip("長押し時間f")] public int StayFlTime;
        [Tooltip("MP消費")] public float UseMP;
        [Tooltip("UI分岐色上書き,a>0")] public Color BranchColor;
        [Tooltip("UI非表示")] public bool HideUI;
        [Tooltip("分岐先ID")] public int FutureNum;
        [Tooltip("分岐後攻撃時間(フレーム)")] public int FutureTime;
    }
    [System.Serializable]
    public class Class_Atk_Fixed
    {
        [HideInInspector] public string EditDisp;
        [Tooltip(Const_Ttp_BID)] public int BranchNum;
        [Tooltip(Const_Ttp_Times)] public Vector2Int Times;
        [Tooltip("移動減速")] public float SpeedRem;
        [Tooltip("ジャンプ不可")] public bool NoJump;
        [Tooltip("ダッシュ不可")] public bool NoDash;
        [Tooltip("照準モード")] public bool Aiming;
        [Tooltip("無重力")] public bool NGravity;
        [Tooltip("無敵")] public bool NoDamage;
    }
    [System.Serializable]
    public class Class_Atk_Shot_Base
    {
        [Tooltip("弾オブジェクト")] public GameObject Obj;
        [Tooltip("発射")] public Class_Atk_Shot_Fire[] Fires;
        [Tooltip("命中効果")] public Class_Atk_Shot_Hit[] Hits;
        [Tooltip("追加弾")] public Class_Atk_Shot_Add[] Adds;
        [Tooltip("召喚設定")]public Class_Atk_Shot_Summon Summon;
        [Tooltip("多段ヒットCT(0以下は単発ヒット)")] public int HitCT;
        public void EditDispSet()
        {
            for (int i = 0; i < Fires.Length; i++)
            {
                var Fire = Fires[i];
                Fire.EditDisp = "[" + i + "]";
                Fire.EditDisp += "BNum:" + Fire.BranchNum;
                Fire.EditDisp += ",Time:" + Fire.Times;
                Fire.EditDisp += ",Count:" + Fire.Count;
                Fire.EditDisp += ",Speed:" + Fire.Speed;
            }
            for (int i = 0; i < Hits.Length; i++)
            {
                var Hit = Hits[i];
                Hit.EditDisp = "[" + i + "]";
                Hit.EditDisp += "BNum:" + Hit.BranchNum + "|(";
                Hit.EditDisp += ET(Hit.DamageType) + ",";
                Hit.EditDisp += Hit.ShortAtk ? T("LABEL_SHORT") : T("LABEL_LONG");
                Hit.EditDisp += ",SP+" + Hit.SPAdd.ToString("F1");
                Hit.EditDisp += ")";
                if (Hit.DamCalc != "") Hit.EditDisp += CalStr(Hit.DamCalc, false);
                if (Hit.BufSets != null)
                    for (int j = 0; j < Hit.BufSets.Length; j++) Hit.BufSets[j].EditDispSet();
            }

        }
        public string OtherStrGet(int BNum, bool AddIf)
        {
            string Str = "";
            int ShotCounts = 0;
            bool Shot = false;
            if (Fires != null)
                for (int i = 0; i < Fires.Length; i++)
                {
                    var Fire = Fires[i];
                    if (Fire.BranchNum >= 0 && Fire.BranchNum != BNum) continue;
                    int TCounts = (Fire.Times.y - Fire.Times.x) / Mathf.Max(1, Fire.Times.z) + 1;
                    ShotCounts += Fire.Count * TCounts;
                    Shot = true;
                }
            if (Hits != null)
                for (int i = 0; i < Hits.Length; i++)
                {
                    var Hit = Hits[i];
                    if (Hit.BranchNum >= 0 && Hit.BranchNum != BNum) continue;
                    if (Str != "") Str += "\n";
                    Str += "<color=#FF8800>(";
                    if (Hit.EHit) Str += T("LABEL_ENEMY");
                    if (Hit.FHit) Str += T("LABEL_FRIEND");
                    if (Hit.MHit) Str += T("LABEL_SELF");
                    Str += ",";
                    Str += Hit.Heals ? T("LABEL_HEAL") : T("LABEL_ATK");
                    Str += ",";
                    Str += Hit.ShortAtk ? T("LABEL_SHORT") : T("LABEL_LONG");
                    if (Hit.SPAdd > 0) Str += ",SP+" + Hit.SPAdd.ToString("F1");
                    Str += ")";
                    if (ShotCounts != 1) Str += "×" + ShotCounts;
                    if (Hit.DamCalc != "") Str += "\n" + CalStr(Hit.DamCalc, true);
                    Str += "</color>";
                    if (Hit.BreakValue > 0) Str += "\n<color=#00FFFF>" + T("LABEL_BREAK") + ":" + Hit.BreakValue.ToString("F1") + "</color>";
                    if (Hit.MultDamChange.x != 0)
                    {
                        Str += "\n<color=#008800>" + T("LABEL_MULTIHIT_CHANGE") + "\n" + Hit.MultDamChange.x + "%/Hit";
                        Str += "(" + (Hit.MultDamChange.x >= 0 ? "Max" : "Min") + Hit.MultDamChange.y + "%)";
                        Str += "</color>";
                    }
                    for (int j = 0; j < Hit.BufSets.Length; j++)
                    {
                        Str += "\n<color=#8888FF>" + Hit.BufSets[j].InfoStr(AddIf) + "</color>";
                    }
                }
            if (Shot && HitCT > 0) Str += "\n" + T("LABEL_MULTIHIT") + HitCT + T("UNIT_F_PER_HIT");
            if (Adds != null)
                for (int i = 0; i < Adds.Length; i++)
                {
                    var Add = Adds[i];
                    if (Add.BranchNum >= 0 && Add.BranchNum != BNum) continue;
                    for (int j = 0; j < Add.AddShots.Length; j++)
                    {
                        var Ads = Add.AddShots[j];
                        for (int k = 0; k < Ads.Shots.Length; k++)
                        {
                            Str += "\n<color=#FF0000>[" + T("LABEL_ADD") + "]</color><size=50%>\n" + Ads.Shots[k].OtherStrGet(BNum, true) + "</size>";
                        }
                    }
                }
            if (Summon != null)
            {
                if (Summon.LimitTime > 0) Str += "\n<color=#FF00FF>" + T("LABEL_SUMMON_TIME") + Summon.LimitTime.ToString("F1") + T("UNIT_SEC") + "</color>";
                if (Summon.HPMulPer != 0) Str += "\n<color=#FF00FF>" + T("LABEL_SUMMON_HP") + Summon.HPMulPer.ToString("F0") + "%</color>";
                if (Summon.AtkMulPer != 0) Str += "\n<color=#FF00FF>" + T("LABEL_SUMMON_ATK") + Summon.AtkMulPer.ToString("F0") + "%</color>";
            }
            return Str;
        }
    }
    [System.Serializable]
    public class Class_Atk_Shot_Fire
    {
        [HideInInspector] public string EditDisp;
        [Tooltip(Const_Ttp_BID)] public int BranchNum;
        [Tooltip(Const_Ttp_Times)] public Vector3Int Times;
        [Tooltip("弾数")] public int Count;
        [Tooltip("弾速度x～y")] public Vector2 Speed;
        [Tooltip("発射形状")] public Class_Atk_Shot_TransBase Trans;
    }
    [System.Serializable]
    public class Class_Atk_Shot_TransBase
    {
        [Tooltip("位置基準")] public Enum_PosBase PosBase;
        [Tooltip("位置変化")] public Class_Atk_Shot_TransSet[] TransPoss;
        [Tooltip("角度基準")] public Enum_RotBase RotBase;
        [Tooltip("角度変化")] public Class_Atk_Shot_TransSet[] TransRots;
    }
    [System.Serializable]
    public class Class_Atk_Shot_TransSet
    {
        [Tooltip("変化方法")] public Enum_TransChange Change;
        [Tooltip("変化補正")] public Vector2 Fix;
        [Tooltip("変化値")] public Vector3 Val;
    }
    [System.Serializable]
    public class Class_Atk_Shot_Hit
    {
        [HideInInspector] public string EditDisp;
        [Tooltip(Const_Ttp_BID)] public int BranchNum;
        [Tooltip("敵命中")] public bool EHit = true;
        [Tooltip("味方命中")] public bool FHit = false;
        [Tooltip("自己命中")] public bool MHit = false;
        [Tooltip("回復")] public bool Heals;
        [Tooltip("ダメージタイプ")]public Enum_DamageType DamageType;
        [Tooltip("近距離攻撃")] public bool ShortAtk;
        [Tooltip("ダメージ式\n"+TooltipStr),TextArea(1,3)] public string DamCalc;
        [Tooltip("多段ダメージ変化%x=変値,y=上下限")] public Vector2 MultDamChange;
        [Tooltip("ブレイク値")] public float BreakValue;
        [Tooltip("命中時SP増加量")] public float SPAdd;
        [Tooltip("ノックバック")] public Class_Atk_Shot_KB[] KBs;
        [Tooltip("状態付与")] public Class_Base_BufSet[] BufSets;
    }
    [System.Serializable]
    public class Class_Atk_Shot_KB
    {
        [Tooltip("方向基準")]public Enum_KBBase Base;
        [Tooltip("衝撃値")] public Vector3 Pow;
        [Tooltip("質量無視")] public bool NoMass;
        [Tooltip("速度指定化")] public bool SetVect;
    }
    [System.Serializable]
    public class Class_Atk_Shot_Summon
    {
        [Tooltip("自然消滅時間(秒)")]public float LimitTime = 0;
        [Tooltip("HP補正%")] public float HPMulPer;
        [Tooltip("ATK補正%")] public float AtkMulPer;
    }
    [System.Serializable]
    public class Class_Atk_Shot_Add
    {
        [HideInInspector] public string EditDisp;
        [Tooltip(Const_Ttp_BID)] public int BranchNum;
        public Data_AddShot[] AddShots;
    }
    [System.Serializable]
    public class Class_Atk_Move
    {
        [Tooltip(Const_Ttp_BID)] public int BranchNum;
        [Tooltip(Const_Ttp_Times)] public Vector3Int Times;
        public Enum_RotBase Base;
        public Vector3 Vect;
        public bool SetSpeed;
    }
    [System.Serializable]
    public class Class_Atk_State
    {
        [Tooltip(Const_Ttp_BID)] public int BranchNum;
        [Tooltip(Const_Ttp_Times)] public Vector3Int Times;
        public Enum_State State;
        [Tooltip("増値式\n"+TooltipStr), TextArea(1, 3)] public string Adds;
    }
    [System.Serializable]
    public class Class_Atk_Buf
    {
        [Tooltip(Const_Ttp_BID)] public int BranchNum;
        [Tooltip(Const_Ttp_Times)] public Vector3Int Times;
        public Class_Base_BufSet[] BufSets;
    }
    [System.Serializable]
    public class Class_Atk_WeponSet
    {
        [Tooltip(Const_Ttp_BID)] public int BranchNum;
        [Tooltip(Const_Ttp_Times)] public Vector2Int Times;
        [Tooltip("武器オブジェクト")] public GameObject Obj;
        [Tooltip("表示部位")] public Enum_WeponSet Set;
        [Tooltip("位置ズレ")] public Vector3 PosChange;
        [Tooltip("角度ズレ")] public Vector3 RotChange;
    }
    [System.Serializable]
    public class Class_Atk_Anim
    {
        [HideInInspector] public string EditDisp;
        [Tooltip(Const_Ttp_BID)] public int BranchNum;
        [Tooltip(Const_Ttp_Times)] public Vector2Int Times;
        [Tooltip("アニメーションID")] public int ID;
        [Tooltip("アニメーション速度加算%")] public float Speed;
    }
    [System.Serializable]
    public class Class_Atk_SEPlay
    {
        [Tooltip(Const_Ttp_BID)] public int BranchNum;
        [Tooltip(Const_Ttp_Times)] public Vector3Int Times;
        [Tooltip("SEファイル")] public AudioClip Clip;
        [Tooltip("音量")] public float Volume = 100f;
        [Tooltip("音程-300～300"), Range(-300f, 300f)] public float Pitch = 100f;
    }
    #endregion
    #region Class_Sta
    [System.Serializable]
    public class Class_Sta_AtkCT
    {
        public int CT;
        public int CTMax;
    }
    [System.Serializable]
    public class Class_Sta_BufInfo
    {
        public int ID;
        public int Index;
        public int Time;
        public int TimeMax;
        public int Pow;
        public int PowMax;
    }
    #endregion
    #region Class_Enemy
    [System.Serializable]
    public class Class_Enemy_AtkAI
    {
        [HideInInspector] public string EditDisp;
        public Vector2Int TimeIf;
        public Class_Enemy_OtherIfs[] OtherIfs;
        public int AtkSlot;
        public Data_Atk AtkD;
        public bool StayInput;
    }
    [System.Serializable]
    public class Class_Enemy_OtherIfs
    {
        public Enum_OtherIfs Ifs;
        public Vector2 Val;
    }
    #endregion
    #region Class_Shot
    [System.Serializable]
    public class Class_Shot_Move
    {
        public Vector3Int Times;
        public Enum_MoveMode MoveMode;
        public Vector3 Pow;
        public Enum_TargetMode TargetMode;
        public int TargetCT;
        [HideInInspector] public State_Base Target;
        [HideInInspector] public State_Hit TargetHit;
        [HideInInspector] public int TCT;
    }
    [System.Serializable]
    public class Class_Shot_Add
    {
        public Vector3Int Times;
        public Data_AddShot AddShot;
    }
    #endregion
    #region Class_Save
    [System.Serializable]
    public class Class_Save_PSaves
    {
        public int MasterVol;
        public int BGMVol;
        public int SEVol;
        public int SystemVol;
        public int QualityLV;
        public int CamDistance;
        public int CamSpeed;
        public int TargetSpeed;
        public int DamTime;
        public int DamStack;
        public int AtkUISize;
        public int MoveStickSize;
        public int JumpDashUISize;
        public int PriSetID;
        public int AddSet1;
        public int AddSet2;
        public int AddSet3;
        public Class_Save_PSaves()
        {
            MasterVol = 100;
            BGMVol = 100;
            SEVol = 100;
            SystemVol = 100;
            switch (Application.platform)
            {
                default:
                    QualityLV = 2;
                    break;
                case RuntimePlatform.WindowsEditor:
                    QualityLV = 3;
                    break;
                case RuntimePlatform.WindowsPlayer:
                    QualityLV = 4;
                    break;
            }
            CamDistance = 75;
            CamSpeed = 100;
            TargetSpeed = 100;
            DamTime = 90;
            DamStack = 3;
            AtkUISize = 100;
            MoveStickSize = 100;
            JumpDashUISize = 100;
            PriSetID = 0;
            AddSet1 = -1;
            AddSet2 = -1;
            AddSet3 = -1;
        }
    }
    [System.Serializable]
    public class Class_Save_PriSet
    {
        public int CharaID;
        public string Disp;
        public string Memo;
        public Class_Save_Atks AtkF;
        public Class_Save_Atks AtkB;
        public Class_Save_Passive Passive;
        public Class_Save_AddAI AddAI_F;
        public Class_Save_AddAI AddAI_B;


        public Class_Save_PriSet()
        {
            CharaID = 0;
            Disp = "";
            Memo = "";
            AtkF = new Class_Save_Atks();
            AtkB = new Class_Save_Atks();
            AtkB.N_AtkID = 1;
            AtkB.S1_AtkID = 1;
            AtkB.S2_AtkID = 3;
            AtkB.E_AtkID = 1;
            Passive = new Class_Save_Passive();
            AddAI_F = new Class_Save_AddAI();
            AddAI_B = new Class_Save_AddAI();
            AddAI_B.Range = 600;
            AddAI_B.NAtk.PerOuR = 400;
        }
        public Class_Save_PriSet(Class_Save_PriSet Copy)
        {
            CharaID = Copy.CharaID;
            Disp = Copy.Disp;
            Memo = Copy.Memo;
            AtkF = Copy.AtkF;
            AtkB = Copy.AtkB;
            Passive = Copy.Passive;
            AddAI_F = Copy.AddAI_F;
            AddAI_B = Copy.AddAI_B;
        }
        public Class_Save_Atks AtkGet(bool Back)
        {
            return !Back ? AtkF : AtkB;
        }
        public Class_Save_AddAI AIGet(bool Back)
        {
            return !Back ? AddAI_F : AddAI_B;
        }
        public int PassiveLVGet(Enum_Passive Pass)
        {
            int LV = 0;
            if (Passive.P1_ID == (int)Pass) LV++;
            if (Passive.P2_ID == (int)Pass) LV++;
            if (Passive.P3_ID == (int)Pass) LV++;
            if (Passive.P4_ID == (int)Pass) LV++;
            return LV;
        }
    }
    [System.Serializable]
    public class Class_Save_Atks
    {
        public int N_AtkID;
        public int S1_AtkID;
        public int S2_AtkID;
        public int E_AtkID;
        public Class_Save_Atks()
        {
            N_AtkID = 0;
            S1_AtkID = 0;
            S2_AtkID = 2;
            E_AtkID = 0;
        }
    }
    [System.Serializable]
    public class Class_Save_Passive
    {
        public int P1_ID;
        public int P2_ID;
        public int P3_ID;
        public int P4_ID;
        public Class_Save_Passive()
        {
            P1_ID = 0;
            P2_ID = 0;
            P3_ID = 5;
            P4_ID = 6;
        }
    }
    [System.Serializable]
    public class Class_Save_AddAI
    {
        public int ChangePer;
        public int ChangeTime;
        public int AIMode;
        public bool PLTarget;
        public int PLDis;
        public int Range;
        public Class_Save_AIActions Jump;
        public Class_Save_AIActions Dash;
        public Class_Save_AIActions NAtk;
        public Class_Save_AIActions S1Atk;
        public Class_Save_AIActions S2Atk;
        public Class_Save_AIActions EAtk;
        public Class_Save_AddAI()
        {
            ChangePer = 20;
            ChangeTime = 150;
            AIMode = 0;
            PLTarget = false;
            Range = 150;
            PLDis = 2000;
            Jump = new Class_Save_AIActions
            {
                PerBase = 20,
                PerOuR = 10,
                PerPLD = 10,
            };
            Dash = new Class_Save_AIActions
            {
                PerBase = 10,
                PerOuR = 100,
                PerPLD = 100,
            };
            NAtk = new Class_Save_AIActions
            {
                PerBase = 800,
                PerOuR = 100,
                PerPLD = 100,
                TimeStay = 6,
            };
            S1Atk = new Class_Save_AIActions
            {
                PerBase = 200,
                PerOuR = 50,
                PerPLD = 50,
                TimeStay = 6,
            };
            S2Atk = new Class_Save_AIActions
            {
                PerBase = 200,
                PerOuR = 25,
                PerPLD = 50,
                TimeStay = 6,
            };
            EAtk = new Class_Save_AIActions
            {
                PerBase = 1000,
                PerOuR = 50,
                PerPLD = 50,
                TimeStay = 6,
            };
        }
    }
    [System.Serializable]
    public class Class_Save_AIActions
    {
        public int PerBase;
        public int PerOuR;
        public int PerPLD;
        public int TimeIn;
        public int TimeStay;
        public bool Stays;
        public int TimeWait;
        public Class_Save_AIActions()
        {
            PerBase = 100;
            PerOuR = 50;
            PerPLD = 50;
            TimeIn = 1000;
            TimeStay = 0;
            Stays = false;
            TimeWait = 0;
        }
    }
    [System.Serializable]
    public class Class_Save_Stages
    {
        public List<int> SoloStars;
        public List<int> MultStars;
        public Class_Save_Stages()
        {
            SoloStars = new List<int>();
            MultStars = new List<int>();
        }
    }
    [System.Serializable]
    public class Class_Save_Genes
    {
        public int ChaosGrain;
        public List<Class_Save_GeneSet> Sets;
        public List<Class_Save_GeneData> Datas;
        public Class_Save_Genes()
        {
            ChaosGrain = 0;
            Sets = new List<Class_Save_GeneSet>();
            for (int i = Sets.Count; i < 10; i++) Sets.Add(new Class_Save_GeneSet());
            Datas = new List<Class_Save_GeneData>();
        }
        public Class_Save_Genes(Class_Save_Genes Copy)
        {
            ChaosGrain = Copy.ChaosGrain;
            Sets = Copy.Sets;
            Datas = new List<Class_Save_GeneData>();
            for (int i = 0; i < Copy.Datas.Count; i++)
            {
                Datas.Add(new Class_Save_GeneData(Copy.Datas[i]));
            }
        }
    }
    [System.Serializable]
    public class Class_Save_GeneSet
    {
        public int G1_ID;
        public int G2_ID;
        public int G3_ID;
        public int G4_ID;
        public int G5_ID;
        public Class_Save_GeneSet()
        {
            G1_ID = -1;
            G2_ID = -1;
            G3_ID = -1;
            G4_ID = -1;
            G5_ID = -1;
        }
    }
    [System.Serializable]
    public class Class_Save_GeneData
    {
        public int Type;
        public int Format;
        public string Name;
        public bool Lock;
        public int LV;
        public int Main;
        public int Sub1;
        public int Add1;
        public int Sub2;
        public int Add2;
        public int Sub3;
        public int Add3;
        public Class_Save_GeneData()
        {
            Type = 0;
            Format = 0;
            Name = "無名因子";
            Lock = false;
            LV = 1;
            Main = 0;
            Sub1 = -1;
            Add1 = 0;
            Sub2 = -1;
            Add2 = 0;
            Sub3 = -1;
            Add3 = 0;
        }
        public Class_Save_GeneData(Class_Save_GeneData Copy)
        {
            Type = Copy.Type;
            Format = Copy.Format;
            Name = Copy.Name;
            Lock = Copy.Lock;
            LV = Copy.LV;
            Main = Copy.Main;
            Sub1 = Copy.Sub1;
            Add1 = Copy.Add1;
            Sub2 = Copy.Sub2;
            Add2 = Copy.Add2;
            Sub3 = Copy.Sub3;
            Add3 = Copy.Add3;
        }
    }

    [System.Serializable]
    public class Class_SDatas
    {
        public Class_Save_PSaves PSaves;
        public Class_Save_PriSet[] PriSets;
        public Class_Save_Stages Stages;
        public Class_Save_Genes Genes;
    }

    #endregion
    #region Class_Other
    [System.Serializable]
    public class Class_Wave
    {
        public GameObject[] Enemys;
        public Vector3[] Pos;
        public float[] HPMult;
        public float[] AtkMult;
    }
    [System.Serializable]
    public class Class_Tables
    {
        [HideInInspector]public string EditDisp;
        public float P;
        public float Mult;
    }
    [System.Serializable]
    public class Class_Chaos_State
    {
        public Enum_ChaosTarget Target;
        public Enum_ChaosSta State;
        public float Per;
    }
    [System.Serializable]
    public class Class_Chaos_BufUp
    {
        public Enum_ChaosTarget Target;
        public Enum_Bufs Buf;
        public float TimePer;
        public float PowPer;
    }
    #endregion

    #endregion

    #region Enum
    public enum Enum_Passive
    {
        HP増加,
        自然再生,
        MP増加,
        気力増幅,
        SPブースト,
        攻撃力増加,
        防御力増加,
        速度増加,
        CTカット,
        必殺再生,
        必殺返還,
        タルタル,
        根性,
        死に力,
        追斬,
        メイン強化,
        通常強化,
        重落強化,
        スキル強化,
        必殺強化,
        近距離強化,
        遠距離強化,
        Wシステム,
        生命の振動,
        硬部位貫通,
        弱点破壊,
        ボスキラー,
        近接防衛,
        遠隔防衛,
        クリティカルヒット,
        裏蓄積,
        無,
    }

    public enum Enum_AddAtk
    {
        タルタル,
        追斬,
        原刺の刃,
    }
    public enum Enum_OtherCT
    {
        タルタル,
        追斬,
        Wシステム,
        生命の振動,
        原刺の刃,
        因子_攻撃4,
        因子_防御4,
        因子_速度4,
        因子_毒殺4,
        因子_混沌4,
        因子_通常4,
        因子_スキル4,
        因子_落下4,
    }
    public enum Enum_PassiveFilter
    {
        基礎ステータス,
        攻撃強化,
        防御強化,
        回復,
        追撃 = 10,
        条件,
        メイン = 20,
        スキル,
        必殺,
    }

    public enum Enum_Bufs
    {
        HP増加 = 0,
        攻撃増加 = 10,
        防御増加 = 20,

        攻撃低下 = 110,
        防御低下 = 120,

        時間制限 = 999,
        毒 = 1000,
        HP再生 = 2000,
        シールド = 2010,
        バリア = 2011,
        根性 = 2100,
        根性CT = 2101,
        復活待機 = 2102,
        標的固定=2103,
        反撃蓄積=2104,
        与ダメージ増加 = 2200,
        近距離強化,
        遠距離強化,
        メイン強化,
        通常強化,
        重撃強化,
        落下強化,
        スキル強化,
        必殺強化,
        エンチャントウェポン = 2300,
        タイムブレイク = 2301,
        過力=2302,
        原刺の刃=2303,
    }
    public enum Enum_BufSet
    {
        付与,
        付与増加,
        不付与増加,
        切り替え,
        消去,
        上書き,
    }
    public enum Enum_BufType
    {
        他=0,
        バフ=1,
        デバフ=2,
    }
    public enum Enum_BufPowDisp
    {
        テキスト=0,
        テキスト割合=1,
        割合=2,
    }
    public enum Enum_OtherIfs
    {
        無 = -1,
        HP割合_x以下 = 0,
        HP割合_x以上,
        ターゲット距離_x以下 = 10,
        ターゲット距離_x以上,
        カオスバフ無し = 100,
        カオスバフ有り,
    }
    public enum Enum_MoveMode
    {
        重力落下_x = -2,
        速度向き = -1,
        加速_x = 0,
        速度変化_x = 1,
        物理ランダム回転_xyz = 2,
        物理指定回転_xyz = 3,
        オブジェランダム回転_xyz = 4,
        オブジェ指定回転_xyz = 5,
        直線補間ホーミング_x距離_y変値 = 10,
        曲線補間ホーミング_x距離_y変値 = 11,
        瞬間移動_x距離 = 12,
        向き合わせ_x距離_y変値 = 13,
    }
    public enum Enum_TargetMode
    {
        ターゲット,
        近敵ターゲット優先,
        近敵,
        ランダム敵,
        自身 = 10,
        味方 = 20,
        ランダム味方,
    }

    public enum Enum_PosBase
    {
        使用者位置,
        ターゲット位置,
    }
    public enum Enum_RotBase
    {
        固定 = -1,
        使用者向き,
        ターゲット方向,
        使用者カメラ方向,
    }
    public enum Enum_TransChange
    {
        ズレ,
        ブレ,
        拡散_掛け,
        拡散_Sin_値割x_ズレy,
        拡散_Cos_値割x_ズレy,
        時間_掛け,
        時間_Sin_値割x_ズレy,
        時間_Cos_値割x_ズレy,
    }
    public enum Enum_KBBase
    {
        固定=-1,
        使用者向き=0,
        弾向き=1,
        対象向き=2,
        使用者_対象方向=10,
        弾_対象方向=11,
    }
    public enum Enum_State
    {
        回復,
        ダメージ,
        MP増加,
        MP減少,
        SP増加,
        SP減少,
    }
    public enum Enum_WeponSet
    {
        基点 = -1,
        右手 = 0,
        左手 = 1,
        右足 = 10,
        左足 = 11,
        胴体 = 20,
        頭 = 30,
    }
    public enum Enum_AtkFilter
    {
        攻撃,
        移動,
        バフ,
        デバフ,
        回復,
        召喚,
        特殊,
        近距離 = 100,
        遠距離,
        照準,
        自己 = 110,
        味方 = 111,
        複数 = 200,
        多段 = 201,
        高頻度 = 202,
        追加攻撃 = 203,
        攻撃強化 = 300,
        防御強化 = 301,
    }
    public enum Enum_AtkIf
    {
        攻撃単入力 = 0,
        攻撃長入力,
        攻撃未入力,
        攻撃未長入力,
        攻撃入力離,
        地上 = 10,
        空中 = 11,
        MP有り = 20,
        MP無し = 21,
        反撃値有=30,
        反撃値無=31,
    }
    public enum Enum_AtkType
    {
        通常,
        スキル,
        必殺,
    }
    public enum Enum_DamageType
    {
        通常,
        重撃,
        落下,
        スキル,
        必殺,
        パッシブ,
    }

    public enum Enum_SetSlot
    {
        キャラ,
        表通常,
        表スキル1,
        表スキル2,
        表必殺,
        裏通常,
        裏スキル1,
        裏スキル2,
        裏必殺,
        パッシブ1,
        パッシブ2,
        パッシブ3,
        パッシブ4,
        因子1,
        因子2,
        因子3,
        因子4,
        因子5,
    }

    public enum Enum_GeneTypes
    {
        体力,
        攻撃,
        防御,
        必殺,
        速度,
        一撃,
        毒殺,
        混沌,
        通常,
        スキル,
        落下,
        終,
    }
    public enum Enum_GeneOptions
    {
        最大HP,
        HP回復速度,
        最大MP,
        MP回復速度,
        SP回復速度,
        SP回復量,
        攻撃力,
        防御力,
        近ダメージ,
        遠ダメージ,
        通常ダメージ,
        重撃ダメージ,
        落下ダメージ,
        スキルダメージ,
        必殺ダメージ,
        終,
    }
    public enum Enum_GeneFormat
    {
        形状1,
        形状2,
        形状3,
        形状4,
        形状5,
        終,
    }

    public enum Enum_PCamLock_Look
    {
        無,
        オブジェ位置,
        キャラ位置,
    }
    public enum Enum_Stage
    {
        チュートリアル,
        テスト,
        ザコ,
        Wブロッコリー,
        悪夢,
        ウェーブ制,
        エッフェル塔,
        EXステージ,
        BOSブロッコリー,
        フライボース,
        危険な斧,
        ながいやーつ,
        ビックマッスル,
        ユニティちゃん,
        ディフェンス,
        フレンドリファイヤー,
        ちーたー,
    }

    public enum Enum_ChaosTarget
    {
        全敵,
        敵ザコ,
        敵ボス,
        全味方,
        プレイヤー,
        召喚味方,
    }
    public enum Enum_ChaosSta
    {
       最大HP,
       HP回復速度,
       攻撃力,
       防御力,

       弱点倍率=100,
    }
    #endregion
}
