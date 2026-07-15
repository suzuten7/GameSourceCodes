
namespace State
{
    using UnityEngine;
    using static Datas.Data_Attack;
    using static GmSystem.GS_GlobalState;

    public partial class State_StateBase
    {
        virtual protected void Event_AddDamage(State_StateBase TSta, float val, Struct_AtkValues atkval)
        {
            if(TeamCheck(TSta.CommonValues.Team) == Enum_TeamCheck.Enemy)ChangeValues.LastHitSta = TSta;
            ChangeValues.LastAddDamageTime = 0;
        }
        virtual protected void Event_TakeDamage(State_StateBase TSta, float val, Struct_AtkValues atkval)
        {
            if(TeamCheck(TSta.CommonValues.Team) == Enum_TeamCheck.Enemy) ChangeValues.LastAttackSta = TSta;
            ChangeValues.LastTakeDamageTime = 0;
        }
        virtual protected void Event_AddHeal(State_StateBase TSta, float val, Struct_AtkValues atkval)
        {
            ChangeValues.LastAddHealTime = 0;
        }
        virtual protected void Event_TakeHeal(State_StateBase TSta, float val, Struct_AtkValues atkval)
        {
            ChangeValues.LastTakeHealTime = 0;
        }
        virtual protected void Event_Death() { }
        virtual protected void Event_Atk(int id, int bid) { }

        public void STUse(float val)
        {
            ST -= val;
            ChangeValues.STUseTime = 0;
        }
        public void LvSets(int Lv)
        {
            LVStateChange(false, Lv);
            CommonValues.LV = Lv;
            NetServs();
        }

        void LVStateChange(bool Lv1Val, int LV)
        {
            var Mult = GetConvertRate(CommonValues.LV, LV);
            float[] values = new float[6];
            values[0] = Mathf.Round(BaseValues.MHP * Mult);
            values[1] = Mathf.Round(BaseValues.HPRegene * Mult);
            values[2] = Mathf.Round(BaseValues.PAtk * Mult);
            values[3] = Mathf.Round(BaseValues.MAtk * Mult);
            values[4] = Mathf.Round(BaseValues.PDef * Mult);
            values[5] = Mathf.Round(BaseValues.MDef * Mult);
            if (Lv1Val)
            {
                BaseLv1.MHP = values[0];
                BaseLv1.HPRegene = values[1];
                BaseLv1.PAtk = values[2];
                BaseLv1.MAtk = values[3];
                BaseLv1.PDef = values[4];
                BaseLv1.MDef = values[5];
            }
            else
            {
                BaseValues.MHP = values[0];
                BaseValues.HPRegene = values[1];
                BaseValues.PAtk = values[2];
                BaseValues.MAtk = values[3];
                BaseValues.PDef = values[4];
                BaseValues.MDef = values[5];
            }
            LVChangeOther(Lv1Val, Mult);
        }
        protected virtual void LVChangeOther(bool Lv1Val, float Mult) { }
        public void BufPowChange(int BufIndex,float Pow)
        {
            var Bufd = ChangeValues.Bufs[BufIndex];
            Bufd.CPow += Pow;
            Bufd.MPow = Mathf.Max(Bufd.CPow, Bufd.MPow);
            ChangeValues.Bufs[BufIndex] = Bufd;
        }
    }
}
