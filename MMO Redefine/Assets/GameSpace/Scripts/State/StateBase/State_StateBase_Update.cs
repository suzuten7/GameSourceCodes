namespace State
{
    using UnityEngine;
    using static GmSystem.GS_GlobalState;
    using FNet;
    using static FNet.Fusion_Reliable;
    public partial class State_StateBase
    {
        int addrt = 0;
        int addst = 0;
        int buft = 0;
        protected void BaseUpdate()
        {
            if (HP <= 0)
            {
                DeathUpdate();
            }
            else if(!float.IsNaN(HP))
            {
                LiveUpdate();
            }
            CashRem();
            LastValueUpdate();
            STUpdate();
            CTUpdate();
            AttackUpdate();
            BufUpdate();

            NetServs();
        }
        void LiveUpdate()
        {
            AnimValues.OtherID = 0;
            ChangeValues.DeathEv = false;
            ChangeValues.DeathTic = 0;
            addrt++;
            if(addrt >= 10)
            {
                var hpr = -BufPowGet(Enum_Buf.Poison) + BufPowGet(Enum_Buf.Regene);
                if (ChangeValues.TimeLim > 0 && !BufHas(Enum_Buf.TimeLimit))
                {
                    hpr -= F_MHP / 5f;
                }
                else if(ChangeValues.LastTakeDamageTime >= Mathf.RoundToInt(BaseValues.HPRegTime * 60))
                {
                    hpr += ValGet(Enum_StateAddsType.HPRegene);
                }
                HP += hpr / 6f;
                MP += ValGet(Enum_StateAddsType.MPRegene) / 6f;
                EX += ValGet(Enum_StateAddsType.EXTimeCharge) / 360f;

                addrt = 0;
            }

            HP = Mathf.Clamp(HP, 1f, F_MHP);
            MP = Mathf.Min(MP, F_MMP);
            EX = Mathf.Clamp(EX, 0, EXMax);
        }
        void DeathUpdate()
        {
            AnimValues.OtherID = -1;
            if (!ChangeValues.DeathEv && !SettingValues.AutoRem) Event_Death();
            ChangeValues.DeathEv = true;
            if (ChangeValues.DeathTic == 0 && SettingValues.DeathMessage)
            {
                var messageStr = CommonValues.Name + "は";
                if (ChangeValues.LastAttackSta != null) messageStr += ChangeValues.LastAttackSta.CommonValues.Name + "に倒された!!!";
                else messageStr += "倒れた!!!";
                var teamStr = TeamGet_Str((int)CommonValues.Team, out var TeamCol);
                teamStr = "<color=#" + ColorUtility.ToHtmlStringRGB(TeamCol) + ">" + teamStr + "</color>";
                Fusion_Reliable.ChatMessage(Enum_MesID.Death, "デス:" + teamStr, messageStr);
            }
            ChangeValues.DeathTic++;
            var rev = BufIndexs(Enum_Buf.Revive);
            if (rev.Count > 0)
            {
                if (ChangeValues.DeathTic >= 60)
                {
                    HP = F_MHP * ChangeValues.Bufs[rev[0]].CPow * 0.01f;
                    ChangeValues.Bufs.RemoveAt(rev[0]);
                }
            }
            else if (SettingValues.AutoRem && ChangeValues.DeathTic >= Mathf.RoundToInt(SettingValues.DeathRemSec * 60))
            {
                Event_Death();
                Runner.Despawn(Object);
            }
        }
        void STUpdate()
        {
            addst++;
            ChangeValues.STUseTime++;

            if (addst >= 10)
            {
                if ((ChangeValues.Ground || ChangeValues.Water) &&
                    ChangeValues.STUseTime >= Mathf.RoundToInt(BaseValues.STRegTime * 60))
                {
                    var str = ValGet(Enum_StateAddsType.STRegene);
                    if (BufHas(Enum_Buf.Kakusei)) str *= 2;
                    ST += str / 6f;
                }
                addst = 0;
            }
            if (ST <= 0) ChangeValues.LowST = true;
            if (ST >= BaseValues.LowSTEnd) ChangeValues.LowST = false;
            ST = Mathf.Min(ST, F_MST);
        }
        void BufUpdate()
        {
            buft++;
            if(buft >= 10)
            {
                buft = 0;
                for (int i = ChangeValues.Bufs.Count - 1; i >= 0; i--)
                {
                    var bufd = ChangeValues.Bufs[i];
                    bufd.CTime-=10;
                    if (bufd.CTime <= 0) ChangeValues.Bufs.RemoveAt(i);
                    else ChangeValues.Bufs[i] = bufd;
                }
            }
        }
        void LastValueUpdate()
        {
            ChangeValues.LastAddDamageTime++;
            ChangeValues.LastTakeDamageTime++;
            ChangeValues.LastAddHealTime++;
            ChangeValues.LastTakeHealTime++;
        }
        protected void DeathMoves()
        {
            if (SettingValues.ModelObj == null) return;
            var TRotX = HP<= 0 || AnimValues.OtherID == -1  ? -90f : 0f;
            SettingValues.ModelObj.transform.rotation = Quaternion.Lerp(SettingValues.ModelObj.transform.rotation, Quaternion.Euler(TRotX, SettingValues.ModelObj.transform.rotation.eulerAngles.y, RotGet.z), SettingValues.DeathMovePer * 0.01f);
        }
    }
}
