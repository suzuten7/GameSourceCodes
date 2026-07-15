namespace UIs
{
    using TMPro;
    using UnityEngine;
    using static GmSystem.GS_SaveValues;
    using static UI_System;
    using static GmSystem.GS_ChangeSet;
    using State;
    using static GmSystem.GS_GlobalValues;

    public class UI_Debug_Main : MonoBehaviour
    {
        [SerializeField] TMP_InputField LvIn;
        [SerializeField] TMP_InputField GoldIn;
        private void LateUpdate()
        {
            ChangeText(LvIn,LPlayerVal.LV.ToString(),true);
            ChangeText(GoldIn,ValueStrings(LPlayerVal.Gold),true);
        }
        public void LvChange()
        {
            if (!int.TryParse(LvIn.text, out var cLv))return;
            LPlayerVal.LV = cLv;
            LPlayerVal.EXP = 0;
        }
        public void GoldChange()
        {
            if (!TryStringValues(GoldIn.text,out var cGold)) return;
            LPlayerVal.Gold = cGold;
        }
        public void EnemyDels()
        {
            foreach(var sta in StateList)
            {
                if (sta == null) continue;
                if (sta.CommonValues.Team == State_StateBase.Enum_Team.PLTeamA) continue;
                if (sta.CommonValues.Team == State_StateBase.Enum_Team.PLTeamB) continue;
                if (sta.CommonValues.Team == State_StateBase.Enum_Team.PLTeamC) continue;
                sta.RPC_Dels();
            }
        }
    }
}

