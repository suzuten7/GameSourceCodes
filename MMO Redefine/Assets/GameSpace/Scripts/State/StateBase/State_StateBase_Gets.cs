namespace State
{
    using System.Collections.Generic;
    using UnityEngine;
    using static GmSystem.GS_GlobalState;
    using static FNet.Fusion_NetValue;
    public partial class State_StateBase
    {
        Dictionary<Enum_StateAddsType, float>[] ValCash = new Dictionary<Enum_StateAddsType, float>[]
        {
            new Dictionary<Enum_StateAddsType, float>(),
            new Dictionary<Enum_StateAddsType, float>(),
            new Dictionary<Enum_StateAddsType, float>(),
            new Dictionary<Enum_StateAddsType, float>(),
            new Dictionary<Enum_StateAddsType, float>(),
        };
        float CashCT = 0;
        public void CashRem()
        {
            CashCT -= Time.fixedDeltaTime;
            if (CashCT <= 0)
            {
                CashCT = 1.0f;
                for (int i = 0; i < ValCash.Length; i++)
                {
                    ValCash[i].Clear();
                }
            }
        }
        void ValTypesOption(Enum_StateAddsType State,out float Val,out Enum_Buf? Bufs,out Enum_DifcVal DifcType, out bool Rev, out bool EXBoost)
        {
            Val = 0f;
            Bufs = null;
            DifcType = Enum_DifcVal.Non;
            Rev = false;
            EXBoost = false;
            switch (State)
            {
                case Enum_StateAddsType.MaxHP: Val = BaseValues.MHP; Bufs = Enum_Buf.MaxHPChange; DifcType = Enum_DifcVal.HP; EXBoost = true; break;
                case Enum_StateAddsType.HPRegene: Val = BaseValues.HPRegene; Bufs = Enum_Buf.HPRegeneChange; DifcType = Enum_DifcVal.HP; EXBoost = true; break;

                case Enum_StateAddsType.MaxMP: Val = BaseValues.MMP; Bufs = Enum_Buf.MaxMPChange; break;
                case Enum_StateAddsType.MPRegene: Val = BaseValues.MPRegene; Bufs = Enum_Buf.MPRegeneChange; break;
                case Enum_StateAddsType.MaxST: Val = BaseValues.MST; Bufs = Enum_Buf.MaxSTChange; break;
                case Enum_StateAddsType.STRegene: Val = BaseValues.STRegene; Bufs = Enum_Buf.STRegeneChange; break;

                case Enum_StateAddsType.PAtk: Val = BaseValues.PAtk; Bufs = Enum_Buf.PAtkChange; DifcType = Enum_DifcVal.Atk; EXBoost = true; break;
                case Enum_StateAddsType.MAtk: Val = BaseValues.MAtk; Bufs = Enum_Buf.MAtkChange; DifcType = Enum_DifcVal.Atk; EXBoost = true; break;

                case Enum_StateAddsType.PDef: Val = BaseValues.PDef; Bufs = Enum_Buf.PDefChange; DifcType = Enum_DifcVal.Def; EXBoost = true; break;
                case Enum_StateAddsType.MDef: Val = BaseValues.MDef; Bufs = Enum_Buf.MDefChange; DifcType = Enum_DifcVal.Def; EXBoost = true; break;

                case Enum_StateAddsType.CritPer: Val = AddValues.CritPer; Bufs = Enum_Buf.CritPerChange; break;
                case Enum_StateAddsType.CritMult: Val = AddValues.CritMult; Bufs = Enum_Buf.CritMultChange; break;
                case Enum_StateAddsType.AtkSpeed: Val = 100 + AddValues.AtkSpeed; Bufs = Enum_Buf.AtkSpeedChange; break;
                case Enum_StateAddsType.MoveSpeed: Val = 100; Bufs = Enum_Buf.MoveSpeedChange; break;
                case Enum_StateAddsType.EXTimeCharge: Val = AddValues.EXTimeCharge; Bufs = Enum_Buf.EXTimeChargeChange; break;
                case Enum_StateAddsType.EXDamageCharge: Val = AddValues.EXDamageCharge; Bufs = Enum_Buf.EXDamageChargeChange; break;
                case Enum_StateAddsType.EXHitCharge:Val = 100 + AddValues.EXHitCharge;Bufs = Enum_Buf.EXHitChargeChange;break;

                case Enum_StateAddsType.SkillCT: Val = 100 + AddValues.SkillCT;Bufs = Enum_Buf.SkillCTChange;break;

                case Enum_StateAddsType.AddDamageMult: Val = 100; Bufs = Enum_Buf.AddDamageMultChange; break;
                case Enum_StateAddsType.TakeDamageRegist: Val = 100; Bufs = Enum_Buf.TakeDamageRegistChange; Rev = true; break;
                case Enum_StateAddsType.AddHealMult: Val = 100; Bufs = Enum_Buf.AddHealMultChange; break;
                case Enum_StateAddsType.TakeHealMult: Val = 100; Bufs = Enum_Buf.TakeHealMultChange; break;

                case Enum_StateAddsType.HitPer: Val = 100; Bufs = Enum_Buf.HitPerChange; break;
                case Enum_StateAddsType.DogePer: Val = 100; Bufs = Enum_Buf.DogePerChange;Rev = true; break;

                case Enum_StateAddsType.NormalDamageMult:Val = 100;Bufs = Enum_Buf.NormalDamageMultChange;break;
                case Enum_StateAddsType.HevDamageMult: Val = 100; Bufs = Enum_Buf.HevDamageMultChange; break;
                case Enum_StateAddsType.SkillDamageMult: Val = 100; Bufs = Enum_Buf.SkillDamageMultChange; break;
                case Enum_StateAddsType.EXDamageMult: Val = 100; Bufs = Enum_Buf.EXDamageMultChange; break;

                case Enum_StateAddsType.ShortDamageMult: Val = 100; Bufs = Enum_Buf.ShortDamageMultChange; break;
                case Enum_StateAddsType.MidleDamageMult: Val = 100; Bufs = Enum_Buf.MidleDamageMultChange; break;
                case Enum_StateAddsType.LongDamageMult: Val = 100; Bufs = Enum_Buf.LongDamageMultChange; break;

                case Enum_StateAddsType.AllEleRegist: Val = 100 - AddValues.BaseElementRegisit; Bufs = Enum_Buf.AllEleRegistChange; Rev = true; break;
                case Enum_StateAddsType.NonEleRegist: Val = EleRegs(Enum_Element.Non); Bufs = Enum_Buf.NonEleRegistChange; Rev = true; break;
                case Enum_StateAddsType.FireEleRegist: Val = EleRegs(Enum_Element.Fire); Bufs = Enum_Buf.FireEleRegistChange; Rev = true; break;
                case Enum_StateAddsType.WaterEleRegist: Val = EleRegs(Enum_Element.Water); Bufs = Enum_Buf.WaterEleRegistChange; Rev = true; break;
                case Enum_StateAddsType.WindEleRegist: Val = EleRegs(Enum_Element.Wind); Bufs = Enum_Buf.WindEleRegistChange; Rev = true; break;
                case Enum_StateAddsType.EarthEleRegist: Val = EleRegs(Enum_Element.Earth); Bufs = Enum_Buf.EarthEleRegistChange; Rev = true; break;
                case Enum_StateAddsType.LightEleRegist: Val = EleRegs(Enum_Element.Light); Bufs = Enum_Buf.LightEleRegistChange; Rev = true; break;
                case Enum_StateAddsType.DarkEleRegist: Val = EleRegs(Enum_Element.Dark); Bufs = Enum_Buf.DarkEleRegistChange; Rev = true; break;

                case Enum_StateAddsType.NonEleMult: Val = 100; Bufs = Enum_Buf.NonEleMultChange; break;
                case Enum_StateAddsType.FireEleMult: Val = 100; Bufs = Enum_Buf.FireEleMultChange; break;
                case Enum_StateAddsType.WaterEleMult: Val = 100; Bufs = Enum_Buf.WaterEleMultChange; break;
                case Enum_StateAddsType.WindEleMult: Val = 100; Bufs = Enum_Buf.WindEleMultChange; break;
                case Enum_StateAddsType.EarthEleMult: Val = 100; Bufs = Enum_Buf.EarthEleMultChange; break;
                case Enum_StateAddsType.LightEleMult: Val = 100; Bufs = Enum_Buf.LightEleMultChange; break;
                case Enum_StateAddsType.DarkEleMult: Val = 100; Bufs = Enum_Buf.DarkEleMultChange; break;

            }
        }
        public float ValGet(Enum_StateAddsType State, Enum_StateAddsOption OptionLV = Enum_StateAddsOption.FinalConst)
        {
            if (ValCash[(int)OptionLV].ContainsKey(State)) return ValCash[(int)OptionLV][State];
            ValTypesOption(State, out var Val, out var Bufs, out var DifcType, out var Rev, out var EXBoost);

            for(int i = 0; i <= (int)Enum_StateAddsOption.FinalConst; i++)
            {
                var AddsVal = AddsGet(State, (Enum_StateAddsOption)i, Rev);
                var BufAddVal = Bufs != null ? BufPowGet((Enum_Buf)Bufs, Enum_BufOp.Add_BaseConst + i, Rev, i == (int)Enum_StateAddsOption.FinalRate) : 0;
                var BufRemVal = Bufs != null ? BufPowGet((Enum_Buf)Bufs, Enum_BufOp.Rem_BaseConst + i, Rev, i == (int)Enum_StateAddsOption.FinalRate) : 0;
                switch ((Enum_StateAddsOption)i)
                {
                    case Enum_StateAddsOption.BaseConst:
                        Val += AddsVal + BufAddVal - BufRemVal;
                        Val *= DificChange((int)CommonValues.Team, DifcType);
                        break;
                    case Enum_StateAddsOption.AddRate:
                        Val *= 1f + ((AddsVal + BufAddVal - BufRemVal) * 0.01f);
                        break;
                    case Enum_StateAddsOption.BackConst:
                        Val += AddsVal + BufAddVal - BufRemVal;
                        break;
                    case Enum_StateAddsOption.FinalRate:
                        if (EXBoost && BufHas(Enum_Buf.Kakusei)) Val *= 1.5f;
                        Val *= AddsVal;
                        if (Bufs != null)Val *= BufAddVal * (2f - BufRemVal);
                        break;
                    case Enum_StateAddsOption.FinalConst:
                        Val += AddsVal + BufAddVal - BufRemVal;
                        break;
                }
                if ((int)OptionLV <= i) return Val;
            }
            ValCash[(int)OptionLV].Add(State, Val);
            return Val;
        }
        public float HP
        {
            get { return ChangeValues.HP; }
            set { ChangeValues.HP = value; }
        }
        public float MP
        {
            get { return ChangeValues.MP; }
            set { ChangeValues.MP = value; }
        }
        public float ST
        {
            get { return ChangeValues.ST; }
            set { ChangeValues.ST = value; }
        }
        public float EX
        {
            get { return ChangeValues.EX; }
            set { ChangeValues.EX = value; }
        }
        public const float EXMax = 100;
        public float F_MHP
        {
            get{return ValGet(Enum_StateAddsType.MaxHP);}
        }
        public float F_MMP
        {
            get{return ValGet(Enum_StateAddsType.MaxMP);}
        }
        public float F_MST
        {
            get{return ValGet(Enum_StateAddsType.MaxST);}
        }
        public float F_PAtk
        {
            get{return ValGet(Enum_StateAddsType.PAtk);}
        }
        public float F_MAtk
        {
            get{return ValGet(Enum_StateAddsType.MAtk);}
        }
        public float F_PDef
        {
            get{return ValGet(Enum_StateAddsType.PDef);}
        }
        public float F_MDef
        {
            get{return ValGet(Enum_StateAddsType.MDef);}
        }
        public Vector3 PosGet
        {
            get
            {
                var Pos = SettingValues.Rig == null ? transform.position : SettingValues.Rig.transform.position;
                Pos.y += 1.2f;
                return Pos;
            }
        }
        public Vector3 RotGet
        {
            get
            {
                var Rot = SettingValues.Rig == null ? transform.eulerAngles : SettingValues.Rig.transform.eulerAngles;
                return Rot;
            }
        }
        public float ElementRegistGet(Enum_Element Ele)
        {
            return ValGet(Enum_StateAddsType.NonEleRegist + (int)Ele);
        }
        public float ElementAddGet(Enum_Element Ele)
        {
            return ValGet(Enum_StateAddsType.NonEleMult + (int)Ele);
        }
        public float BufPowGet(Enum_Buf Buf, Enum_BufOp Op = Enum_BufOp.Non, bool Rev = false, bool Mult = false)
        {
            var Pows = !Mult ? 0f : 1f;
            foreach (var Bufd in ChangeValues.Bufs)
            {
                if (Bufd.ID != (short)Buf) continue;
                if (Op != Enum_BufOp.Non && Bufd.Op != (byte)Op) continue;
                if (!Mult) Pows += Bufd.CPow * (!Rev ? 1 : -1);
                else Pows *= 1f + Bufd.CPow * (!Rev ? 1 : -1) * 0.01f;
            }
            return Pows;
        }
        public bool BufHas(Enum_Buf Buf, Enum_BufOp Op = Enum_BufOp.Non)
        {
            if (Op == Enum_BufOp.Non) return ChangeValues.Bufs.FindIndex(0, x => x.ID == (short)Buf) >= 0;
            else return ChangeValues.Bufs.FindIndex(0, x => x.ID == (short)Buf && x.Op == (byte)Op) >= 0;
        }
        public List<int> BufIndexs(Enum_Buf Buf)
        {
            var indexs = new List<int>();
            for(int i = 0; i < ChangeValues.Bufs.Count; i++)
            {
                if (ChangeValues.Bufs[i].ID == (short)Buf)indexs.Add(i);
            }
            return indexs;
        }
        float EleRegs(Enum_Element Ele)
        {
            var Val = ValGet(Enum_StateAddsType.AllEleRegist);
            for (int i = 0; i < AddValues.Element_Regists.Length; i++)
                if (AddValues.Element_Regists[i].Element == Ele)
                    Val -= AddValues.Element_Regists[i].RegistPer;
            return Val;
        }
        virtual public float AddsGet(Enum_StateAddsType State, Enum_StateAddsOption Option, bool Rev)
        {
            return Option != Enum_StateAddsOption.FinalRate ? 0 : 1;
        }
        public Enum_TeamCheck TeamCheck(Enum_Team OtherTeam)
        {
            if (CommonValues.Team == OtherTeam) return Enum_TeamCheck.Friend;
            if (OtherTeam == Enum_Team.Collect)
            {
                switch (CommonValues.Team)
                {
                    case Enum_Team.PLTeamC:
                    case Enum_Team.PLTeamB:
                    case Enum_Team.PLTeamA:
                        return Enum_TeamCheck.Enemy;
                    default:
                        return Enum_TeamCheck.Non;
                }
            }
            return Enum_TeamCheck.Enemy;
        }
        virtual public Transform CameraGet => null;
        virtual public Vector3 CameraRot
        {
            get
            {
                return RotGet;
            }
        }
        virtual public GameObject TargetObjGet
        {
            get
            {
                if (ChangeValues.LastHitSta != null) return ChangeValues.LastHitSta.gameObject;
                if (ChangeValues.LastAttackSta != null) return ChangeValues.LastAttackSta.gameObject;
                return null;
            }
        }
        virtual public State_StateBase TargetStaGet
        {
            get
            {
                if (ChangeValues.LastHitSta != null) return ChangeValues.LastHitSta;
                if (ChangeValues.LastAttackSta != null) return ChangeValues.LastAttackSta;
                return null;
            }
        }
        virtual public Vector3 TargetPosGet
        {
            get
            {
                if (ChangeValues.LastHitSta != null) return ChangeValues.LastHitSta.PosGet;
                if (ChangeValues.LastAttackSta != null) return ChangeValues.LastAttackSta.PosGet;
                return PosGet;
            }
        }
        virtual public Enum_Element EleRideGet(int ID = 0)
        {
            var eles = new List<Enum_Element>();
            var mix = ID < 0 && BufHas(Enum_Buf.MixEleRide);
            for (int k = 0; k < ChangeValues.Bufs.Count; k++)
            {
                for (int i = 0; i <= (int)Enum_Element.Dark; i++)
                {
                    if (ChangeValues.Bufs[k].ID == (short)(Enum_Buf.NonEleRide + i))
                    {
                        eles.Add((Enum_Element)i);
                    }
                }
            }
            if (eles.Count <= 0) return CommonValues.BaseEleRide;
            else if (!mix || ID == -1 || eles.Count <= 1) return eles[eles.Count - 1];
            else return eles[eles.Count - 2];
        }

        float GetMultiplier(int lv)
        {
            float basePart = 1f + 0.05f * (lv - 1);
            float stagePart = 1f + 0.1f * Mathf.Floor(lv / 10f);
            return basePart * stagePart;
        }
        public float GetConvertRate(int fromLv, int toLv)
        {
            return GetMultiplier(toLv) / GetMultiplier(fromLv);
        }
    }

}

