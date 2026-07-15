namespace Player
{
    using Fusion;
    using UnityEngine;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static FNet.Fusion_Reliable;
    using static Datas.Data_Get;
    using static Datas.Data_Equips;
    using static GmSystem.GS_GlobalState;
    using static UIs.UI_System;
    public partial class Player_State
    {

        #region 基本処理
        protected override void Start()
        {
            ParentStrage(gameObject, "Player");
            StateList.Add(this);
            PStaList.Add(this);
            if (CanControl(Object))
            {
                MyPList.Add(this);
                if (BotID < 0)
                {
                    MyPlayer = this;
                    _autoSaveT = Mathf.RoundToInt(AutoSaveCT * 60);
                    SaveSetLocal();
                }
                LocalSetChara();
            }
            HP = F_MHP;
            MP = F_MMP;
            ST = F_MST;
            EX = EXMax;
            ChangeValues.STUseTime = Mathf.RoundToInt(BaseValues.STRegTime * 60);
            ChangeValues.LastAtkTime = 9999;
        }
        public void FixedUpdate()
        {
            if (NoOwnerCheck(Object) && BotID < 0)
            {
                ChatMessage(Enum_MesID.System, CommonValues.Name + "が切断しました", "");
                Runner.Despawn(Object);
                return;
            }
            DeathMoves();
            if (!CanControl(Object))
            {
                NetsLocalSet();
                ObjectNameSet();
                return;
            }
            if (!_starts && BotID < 0)
            {
                ChatMessage(Enum_MesID.System, PlayerName + "が参加しました", "");
                Respawne(false);
            }
            _starts = true;
            LocalSetChara();
            PStateSet();
            ObjectNameSet();
            EquipTriggerStart(Enum_EquipTrigger.Non, this);
            BaseUpdate();
            if (PhotoMode)
            {
                var set = PhotoSetGet(this);
                switch (set.MotionSet)
                {
                    case 3: AnimValues.OtherID = -1; break;
                }
            }
            if (BotID >= 0 && HP <= 0)
            {
                if (ChangeValues.DeathTic >= RevTime * 60) Respawne(true);
            }
            if (Cont != null)
            {
                if (Cont.In_ShortCutBack) PlayerValues.ShortCutBack = !PlayerValues.ShortCutBack;
                Cont.In_ShortCutBack = false;
            }
            if (BotID < 0)
            {
                LPlayerVal.PlayTimes++;
                if (LPlayerVal.EXP >= NextEXPGet(LPlayerVal.LV))
                {
                    LPlayerVal.EXP -= NextEXPGet(LPlayerVal.LV);
                    LPlayerVal.LV++;
                    var DObj = Instantiate(DB.DamObj, PosGet + Vector3.up * 0.8f, Quaternion.identity);
                    TeamGet_Str((int)CommonValues.Team, out var oTeamCol);
                    DObj.TextSet(this,this,125,"LVUP!!!", oTeamCol, new Color(1, 0.5f, 0.25f));
                }
                _autoSaveT--;
                if (_autoSaveT <= 0)
                {
                    _autoSaveT = Mathf.RoundToInt(AutoSaveCT * 60);
                    SaveSet();
                }
            }

        }

        #endregion
    }
}
