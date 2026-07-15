
namespace State
{
    using Datas;
    using Fusion;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using static Datas.Data_Attack;
    using static Datas.Data_EnemyBattle;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalState;
    using static GmSystem.GS_GlobalValues;
    using static State.State_StateBase;

    public class State_EnemyAI : NetworkBehaviour
    {
        [System.Serializable]
        public class Class_State_TargetOptions
        {
            public Dictionary<State_StateBase, float> Targets = new Dictionary<State_StateBase, float>();
            public float ChangeCT;
            [Tooltip("初期蓄積")] public float StartVal;
            [Tooltip("蓄積上限")] public float MaxVal;
            [Tooltip("時間減少蓄積")] public float TimeRem;
            [Tooltip("最近蓄積")] public float NearAdd;
            [Tooltip("ダメージ蓄積補正%")] public float DamAddPer;
            [Tooltip("挑発蓄積補正%")] public float TurAddPer;
        }
        public State_StateBase Sta;
        public Class_State_TargetOptions TargetOptions;
        [Tooltip("移動速度")]
        public float MoveSpeed;
        [Tooltip("減速%(x=水平,y=垂直)")]
        public Vector2 SpeedRemPer;
        [Tooltip("回転速度%")]
        public float RotPer;
        public bool Flys;
        [Tooltip("ホーム範囲:x=巡回,y=追跡限界")]
        public Vector2 HomeRange;
        [Tooltip("巡回位置変化時間")]
        public float PatrleChangeWait;
        [Tooltip("戦闘感知範囲")]
        public float CheckRange;
        [Tooltip("撤退時間")]
        public float RetreatTime;
        public Data_EnemyBattle BattleD;
        [Header("変数")]

        public State_StateBase Target;
        public int ctime;
        public int rtime;
        public int ptime;
        public int btime;
        public int aid;
        public Vector3 homePos;
        public Vector3 patrlePos;
        bool lmv;
        bool VeUsed
        {
            get
            {
                return Flys || Sta.ChangeValues.Water;
            }
        }
        float SpeedMlt
        {
            get
            {
                var bspeed = 1f - Sta.BufPowGet(Enum_Buf.Reiki) * 0.01f;
                bspeed *= Sta.ValGet(Enum_StateAddsType.MoveSpeed) * 0.01f;
                return bspeed;
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (Sta == null) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(Sta.PosGet, HomeRange.x);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(Sta.PosGet, HomeRange.y);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Sta.PosGet, CheckRange);
        }
#endif
        private void Start()
        {
            homePos = Sta.PosGet;
        }
        void FixedUpdate()
        {
            if (!CanControl(Object))return;
            var rigVect = Sta.SettingValues.Rig.linearVelocity;
            var verPer = 1f - SpeedRemPer.x * 0.01f;
            rigVect.x *= verPer;
            rigVect.z *= verPer;
            rigVect.y *= 1f - SpeedRemPer.y * 0.01f;
            Sta.SettingValues.Rig.linearVelocity = rigVect;
            Sta.AnimValues.MoveID = 0;
            if (Sta.HP > 0)
            {
                State_StateBase nsta = null;
                var tneardis = CheckRange;
                foreach (var t in StateList)
                {
                    if (t == null) continue;
                    if (Sta.TeamCheck(t.CommonValues.Team) != Enum_TeamCheck.Enemy) continue;
                    if (t.HP <= 0) continue;
                    var tdis = Vector3.Distance(Sta.PosGet, t.PosGet);
                    if (tneardis > tdis)
                    {
                        tneardis = tdis;
                        nsta = t;
                    }
                }
                if (nsta != null)
                {
                    TargetAdds(0, nsta, TargetOptions.NearAdd / 60f);
                }
                var tysta = Sta.ChangeValues.TyouhatuSta;
                var tyval = Sta.BufPowGet(Enum_Buf.Tyouhatu);
                if (tyval > 0 && tysta != null)
                {
                    TargetAdds(2, tysta, tyval / 60f);
                }
                else tysta = null;
                var tkey = TargetOptions.Targets.Keys.ToArray();
                foreach (var ts in tkey)
                {
                    if (ts == nsta) continue;
                    if (ts == Target) continue;
                    if (ts == tysta) continue;
                    TargetOptions.Targets[ts] -= TargetOptions.TimeRem/60f;
                    if (TargetOptions.Targets[ts] <= 0 || ts.HP <= 0) TargetOptions.Targets.Remove(ts);
                }
                ctime--;
                if(ctime <= 0 || (Target == null && TargetOptions.Targets.Count>0) || (Target != null && Target.HP <= 0))
                {
                    ctime = Mathf.RoundToInt(TargetOptions.ChangeCT * 60);
                    Target = null;
                    var mval = float.MinValue;
                    foreach(var ts in TargetOptions.Targets.Keys)
                    {
                        if (ts == null) continue;
                        if (ts.HP <= 0) continue;
                        if(mval < TargetOptions.Targets[ts] || ts.HP <= 0)
                        {
                            mval = TargetOptions.Targets[ts];
                            Target = ts;
                        }
                    }
                }
                if (Target == null)
                {
                    NTargetUpdate();
                }
                else
                {
                    TargetUpdate();
                }
                var AtkKeys = Sta.AttackVals.Keys.ToArray();

                for (int i = 0; i < AtkKeys.Length; i++)
                {
                    var AtkVal = Sta.AttackVals[AtkKeys[i]];
                    var Has = false;
                    var Enter = false;
                    var Stay = false;
                    var Pos = Sta.PosGet;
                    var Rot = Sta.RotGet;
                    for (int k = 0; k < BattleD.Acts.Count; k++)
                    {
                        var EB = BattleD.Acts[k];
                        switch (EB)
                        {
                            case Class_Data_EBEvent_Attack:
                                Debug.Log("Atk");
                                var EBAtk = (Class_Data_EBEvent_Attack)EB;
                                if (AtkKeys[i] != EBAtk.ID) continue;
                                Has = true;
                                Pos += Quaternion.Euler(Rot) * EBAtk.PosOffSet;
                                Rot += EBAtk.RotOffSet;

                                if (Target == null || !EB_Act_Check(EB)) break;
                                if (!EBAtk.Stay)
                                {
                                    if (btime % 6 == 0) Enter = true;
                                    if (btime % 6 <= 2) Stay = true;
                                }
                                else
                                {
                                    if (AtkVal.ITime <= 1) Enter = true;
                                    Stay = true;
                                }
                                break;
                        }
                    }
                    if (Has)
                    {

                        Sta.AttackTrans(AtkKeys[i], Pos, Rot);
                        Sta.AttackInput(AtkKeys[i], false, Enter);
                        Sta.AttackInput(AtkKeys[i], true, Stay);
                    }
                    //Debug.Log("Atk" + AtkKeys[i] + "," + Has + "," + Enter + "," + Stay);
                }
            }

        }

        void NTargetUpdate()
        {
            Sta.HP = Sta.F_MHP;
            Sta.MP = Sta.F_MMP;
            Sta.ST = Sta.F_MST;
            Sta.EX = 0;
            Sta.ChangeValues.Bufs.Clear();
            Sta.SkillCTs.Clear();
            Sta.AttackVals.Clear();
            rtime = 0;
            btime = 0;
            aid = 0;
            ptime--;
            if (ptime <= 0)
            {
                ptime = Mathf.RoundToInt(PatrleChangeWait * 60);
                var rvect = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized;
                if (!Flys) rvect.y = 0;
                patrlePos = homePos + rvect * Random.Range(0, HomeRange.x);
            }
            var rigVect = Sta.SettingValues.Rig.linearVelocity;
            var mvect = patrlePos - Sta.PosGet;
            if(mvect.magnitude >= 1.5f)
            {
                var mvvect = mvect.normalized * 0.01f * MoveSpeed * SpeedMlt;
                rigVect += mvvect;
                Sta.SettingValues.Rig.linearVelocity = rigVect;
                LookVects(mvvect, RotPer);
                Sta.AnimValues.MoveID = 1;
            }
        }
        void TargetUpdate()
        {

            ptime = 0;
            btime++;
            var mhdis = Vector3.Distance(Sta.PosGet, homePos);
            if (mhdis >= HomeRange.y)
            {
                var rigVect = Sta.SettingValues.Rig.linearVelocity;
                var mvect = homePos - Sta.PosGet;
                var mvvect = Quaternion.Euler(0,lmv ? 45 : -45, 0) * mvect.normalized * 0.01f * MoveSpeed * SpeedMlt * 4f;
                if (Random.value <= 0.05f) lmv = !lmv;
                rigVect += mvvect;
                Sta.SettingValues.Rig.linearVelocity = rigVect;
                Sta.AnimValues.MoveID = 1;
            }
            var thdis = Vector3.Distance(Target.PosGet, homePos);
            if (thdis >= HomeRange.y)
            {
                rtime++;
                var rigVect = Sta.SettingValues.Rig.linearVelocity;
                var mvect = homePos - Sta.PosGet;
                var mvvect = mvect.normalized * 0.01f * MoveSpeed * SpeedMlt * 2f;
                rigVect += mvvect;
                Sta.SettingValues.Rig.linearVelocity = rigVect;
                Sta.AnimValues.MoveID = 1;
            }
            else rtime -=2;
            rtime = Mathf.Max(0, rtime);
            if (rtime >= Mathf.RoundToInt(RetreatTime * 60))
            {
                Sta.ChangeValues.LastAttackSta = null;
                Target = null;
                Sta.SettingValues.Rig.position = homePos;
                TargetOptions.Targets.Clear();
                NTargetUpdate();
                return;
            }
            for (int i = 0; i < BattleD.Acts.Count; i++)
            {
                var EB = BattleD.Acts[i];
                if (!EB_Act_Check(EB)) continue;
                switch (EB)
                {
                    case Class_Data_EBEvent_Branch: EB_Act_Branch((Class_Data_EBEvent_Branch)EB); break;
                    case Class_Data_EBEvent_Move: EB_Act_Move((Class_Data_EBEvent_Move)EB); break;
                    case Class_Data_EBEvent_Rot: EB_Act_Rot((Class_Data_EBEvent_Rot)EB); break;
                    case Class_Data_EBEvent_Attack: EB_Act_Atk((Class_Data_EBEvent_Attack)EB); break;
                    case Class_Data_EBEvent_MyValState:EB_Act_MyValState((Class_Data_EBEvent_MyValState)EB);break;
                    case Class_Data_EBEvent_MyBuf:EB_Act_MyBuf((Class_Data_EBEvent_MyBuf)EB);break;
                }
            }
            var endTime = 1f;
            for (int i = 0; i < BattleD.Branchs.Count; i++)
            {
                if (BattleD.Branchs[i].AID == aid) endTime = BattleD.Branchs[i].EndTime;
            }
            if (btime >= Mathf.RoundToInt(endTime * 60)) btime = 0;

        }
        void LookVects(Vector3 mvect,float RotPers)
        {
            var rotVect = (Sta.PosGet + mvect) - Sta.PosGet;
            if (!VeUsed) rotVect.y = 0;
            var lerpVect = Vector3.Slerp(Sta.SettingValues.Rig.transform.forward, rotVect, RotPers * 0.01f);
            if (!VeUsed) lerpVect.y = 0;
            Sta.SettingValues.Rig.transform.LookAt(Sta.SettingValues.Rig.transform.position + lerpVect);
        }
        bool EB_Act_Check(Class_Data_EBEvent_Base EB)
        {
            var acheck = false;
            if (aid == EB.ActID) acheck = true;
            if (EB.ActIDs != null) for (int i = 0; i < EB.ActIDs.Length; i++)
                {
                    if (aid == EB.ActIDs[i])
                    {
                        acheck = true;
                        break;
                    }
                }
            if (!acheck) return false;
            if (!V3TimeCheck(btime, EB.Times)) return false;
            for(int i = 0; i < EB.Ifs.Length; i++)
            {
                var cval = 0f;
                Vector3 spos = Sta.PosGet;
                Vector3 tpos = Target.PosGet;
                var tsta = EB.Ifs[i].Target ? Target : Sta;
                switch (EB.Ifs[i].Ifs)
                {
                    case Enum_EBattleIfs.TargetDis_x:
                        cval = Vector3.Distance(spos, tpos);
                        break;
                    case Enum_EBattleIfs.TargetVirtDis_x:
                        spos.y = 0;
                        tpos.y = 0;
                        cval = Vector3.Distance(spos, tpos);
                        break;
                    case Enum_EBattleIfs.TargetHoriDis_x:
                        cval = Mathf.Abs(spos.y - tpos.y);
                        break;
                    case Enum_EBattleIfs.HPPer_x:
                        cval = tsta.HP / Mathf.Max(tsta.F_MHP);
                        break;
                    case Enum_EBattleIfs.HPC_x:
                        cval = tsta.HP;
                        break;
                    case Enum_EBattleIfs.MPPer_x:
                        cval = tsta.MP / Mathf.Max(tsta.F_MMP);
                        break;
                    case Enum_EBattleIfs.MPC_x:
                        cval = tsta.MP;
                        break;
                    case Enum_EBattleIfs.STPer_x:
                        cval = tsta.ST / Mathf.Max(tsta.F_MST);
                        break;
                    case Enum_EBattleIfs.STC_x:
                        cval = tsta.ST;
                        break;
                    case Enum_EBattleIfs.EX_x:
                        cval = tsta.EX;
                        break;
                }
                var below = EB.Ifs[i].Down;
                var ival = EB.Ifs[i].Val;
                if (!below && cval < ival) return false;
                if (below && cval > ival) return false;
            }
            return true;
        }
        Vector3 RotBaseGet(Enum_EBattle_Rot BRot)
        {
            switch (BRot)
            {
                case Enum_EBattle_Rot.World:
                    return Vector3.zero;
                case Enum_EBattle_Rot.TargetLook:
                    if (Target == null) break;
                    return Quaternion.LookRotation(Target.PosGet - Sta.PosGet).eulerAngles;
                case Enum_EBattle_Rot.LastAttackLook:
                    if (Sta.ChangeValues.LastAttackSta == null) break;
                    return Quaternion.LookRotation(Sta.ChangeValues.LastAttackSta.PosGet - Sta.PosGet).eulerAngles;
                case Enum_EBattle_Rot.LastHitLook:
                    if (Sta.ChangeValues.LastHitSta == null) break;
                    return Quaternion.LookRotation(Sta.ChangeValues.LastHitSta.PosGet - Sta.PosGet).eulerAngles;
            }
            return Sta.RotGet;
        }
        void EB_Act_Branch(Class_Data_EBEvent_Branch EBAct)
        {
            var pm = 0f;
            for(int i=0;i< EBAct.Nexts.Length; i++)
            {
                pm += EBAct.Nexts[i].P;
            }
            var ps = 0f;
            var r = Random.Range(0, pm);
            for (int i = 0; i < EBAct.Nexts.Length; i++)
            {
                ps += EBAct.Nexts[i].P;
                if (r > ps) continue;
                aid = EBAct.Nexts[i].ID;
                btime = Mathf.RoundToInt(EBAct.Nexts[i].Time*60);
                break;
            }
        }
        void EB_Act_Move(Class_Data_EBEvent_Move Move)
        {
            var rigVect = Sta.SettingValues.Rig.linearVelocity;
            var rot = RotBaseGet(Move.BaseRot);
            var cvect = Move.SpeedPer <= 0 ? Move.AddVect : Move.AddVect.normalized * MoveSpeed * Move.SpeedPer * 0.01f * SpeedMlt;
            var mvect = Quaternion.Euler(rot) * cvect * 0.01f;
            if (!Move.VeUsed)
            {
                var mg = mvect.magnitude;
                mvect.y = 0;
                mvect =  mvect.normalized * mg;
            }
            LookVects(mvect, Move.LookPer);
            Sta.AnimValues.MoveID = 2;
            if (!Move.SetVect) rigVect += mvect;
            else rigVect = mvect;
            Sta.SettingValues.Rig.linearVelocity = rigVect;
        }
        void EB_Act_Rot(Class_Data_EBEvent_Rot Rot)
        {
            var rot = RotBaseGet(Rot.BaseRot);
            rot += Rot.AddRot;
            var rvect = Quaternion.Euler(rot) * Vector3.forward;
            LookVects(rvect, Rot.LookPer);
        }
        void EB_Act_Atk(Class_Data_EBEvent_Attack Atk)
        {
            var Pos = Sta.PosGet;
            var Rot = Sta.RotGet;
            Pos += Quaternion.Euler(Rot) * Atk.PosOffSet;
            Rot += Atk.RotOffSet;
            Sta.AttackStart(Atk.ID, Atk.AttackD, Pos,Rot,Atk.NoUse,Atk.ASpdAdd);
        }
        void EB_Act_MyValState(Class_Data_EBEvent_MyValState mval)
        {
            var val = Calc(mval.Val, Sta, Sta, null);
            switch (mval.State)
            {
                case Enum_ValState.HP:
                    if (val > 0)
                    {
                        val *= Sta.ValGet(Enum_StateAddsType.TakeHealMult) * 0.01f;
                    }
                    Sta.Damage(Sta,Sta.PosGet,-val,new Struct_AtkValues { });
                    break;
                case Enum_ValState.MP:
                    Sta.MP += val;
                    break;
                case Enum_ValState.ST:
                    Sta.ST += val;
                    break;
                case Enum_ValState.EX:
                    Sta.EX += val;
                    break;
            }
        }

        void EB_Act_MyBuf(Class_Data_EBEvent_MyBuf mbuf)
        {
            var cpow = mbuf.CPow.Length > 0 ? Calc(mbuf.CPow, Sta, Sta, null) : 0;
            var mpow = mbuf.MPow.Length > 0 ? Calc(mbuf.MPow, Sta, Sta, null) : -1;
            Sta.BufSet(mbuf.ID, mbuf.Op, mbuf.Index, mbuf.Set, mbuf.AddTimes.x, mbuf.AddTimes.y, cpow, mpow);
        }
        public void TargetAdds(int type,State_StateBase tsta,float val)
        {
            if (TargetOptions.Targets.Count <= 0)
            {
                var dis = Vector3.Distance(tsta.PosGet, homePos);
                if (dis > HomeRange.y) return;
            }
            if (!TargetOptions.Targets.ContainsKey(tsta)) TargetOptions.Targets.Add(tsta, TargetOptions.StartVal);
            var mult = 0f;
            switch (type)
            {
                case 1: mult = TargetOptions.DamAddPer;break;
                case 2: mult = TargetOptions.TurAddPer; break;
            }
            TargetOptions.Targets[tsta] = Mathf.Min(TargetOptions.Targets[tsta] + (val / Sta.F_MHP) * (1f + mult * 0.01f),TargetOptions.MaxVal);
        }
    }
    
}

