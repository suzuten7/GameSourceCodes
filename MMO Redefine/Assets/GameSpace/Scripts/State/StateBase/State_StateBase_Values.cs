using Fusion;
using System.Collections.Generic;
using UnityEngine;
using GmSystem;
using System;
namespace State
{
    public partial class State_StateBase
    {
        [System.Serializable]
        public class Class_State_SettingValues
        {
            public Rigidbody Rig;
            public GameObject ModelObj;
            public bool AutoRem;
            public bool DeathMessage;
            public float DeathMovePer;
            public float DeathRemSec;
            public float SyncTime;
            public bool Boss;
        }
        [System.Serializable]
        public class Class_State_CommonValues
        {
            public string Name;
            public Enum_Team Team;
            public int LV;
            public GS_GlobalState.Enum_Element BaseEleRide;
        }

        [System.Serializable]
        public class Class_State_BaseValues
        {
            public float MHP;
            public float HPRegene;
            public float HPRegTime;
            public float MMP;
            public float MPRegene;
            public float MST;
            public float STRegene;
            public float STRegTime;
            public float LowSTEnd;
            public float PAtk;
            public float MAtk;
            public float PDef;
            public float MDef;
        }
        [System.Serializable]
        public class Class_State_BaseLv1
        {
            public float MHP;
            public float HPRegene;
            public float PAtk;
            public float MAtk;
            public float PDef;
            public float MDef;
        }
        [System.Serializable]
        public class Class_State_AddValues
        {
            public float CritPer;
            public float CritMult;
            public float AtkSpeed;
            public float BaseElementRegisit;
            public Class_State_Element_Regist[] Element_Regists;
            public float EXTimeCharge;
            public float EXDamageCharge;
            public float EXHitCharge;

            public float SkillCT;
        }
        [System.Serializable]
        public class Class_State_ChangeValues
        {
            public float HP;
            public float MP;
            public float ST;
            public float EX;
            public bool LowST;
            public int STUseTime;
            public List<Struct_Bufs> Bufs = new List<Struct_Bufs>();

            public bool Ground;
            public bool Water
            {
                get
                {
                    var check = false;
                    for (int i = Waters.Count - 1; i >= 0; i--)
                    {
                        if (Waters[i] == null)
                        {
                            Waters.RemoveAt(i);
                            continue;
                        }
                        check = true;
                    }
                    return check;
                }
            }
            public List<Obj.Obj_Water> Waters;
            public float TimeLim;
            public bool DeathEv;
            public int DeathTic;
            public int LastAtkTime;

            public int LastLookTime;
            public State_StateBase LastHitSta = null;
            public int LastAddDamageTime;
            public int LastAddHealTime;
            public State_StateBase LastAttackSta = null;
            public int LastTakeDamageTime;
            public int LastTakeHealTime;

            public State_StateBase TyouhatuSta = null;
        }

        [System.Serializable]
        public class Class_State_Element_Regist
        {
            public GS_GlobalState.Enum_Element Element;
            public float RegistPer;
        }
        [System.Serializable]
        public class Class_State_AnimValues
        {
            public int MoveID;
            public float MoveSpeed;
            public int LAtkID;
            public int LAtkCo;
            public float LAtkSpeed;
            public int RAtkID;
            public int RAtkCo;
            public float RAtkSpeed;
            public int SAtkID;
            public int SAtkCo;
            public float SAtkSpeed;
            public int OtherID;
            public float OtherSpeed;
        }

        [System.Serializable]
        public struct Struct_Bufs : INetworkStruct
        {
            public short ID;
            public byte Op;
            public short Index;
            public int CTime;
            public int MTime;
            public float CPow;
            public float MPow;
        }
        [System.Serializable]
        public struct Struct_AtkValues : INetworkStruct
        { 
            public int aslot;
            public bool crit;
            public byte atktype;
            public byte element;
            public byte rangetype;
        }
        public enum Enum_Team
        {
            [InspectorName("独立")]Other = -9999,
            [InspectorName("プレイヤーチームC")] PLTeamC = -3,
            [InspectorName("プレイヤーチームB")] PLTeamB = -2,
            [InspectorName("プレイヤーチームA")] PLTeamA = -1,
            [InspectorName("通常敵")] EnemyN = 0,
            [InspectorName("採取")] Collect = 100,
            [InspectorName("中立")] Neutral = 1000,
        }


    }
}
