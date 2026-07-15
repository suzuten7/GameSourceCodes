
namespace Player
{
    using UnityEngine;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_SaveValues;
    public class Player_Skill : MonoBehaviour
    {
        [SerializeField] Player_State PSta;

        void FixedUpdate()
        {
            if (!CanControl(PSta.Object)) return;
            var Cont = PSta.Cont;
            if (PSta.HP <= 0)
            {
                Cont.In_ShortCutSet = 0;
                return;
            }

            foreach(var aval in PSta.AttackVals)
            {
                if (aval.Key >= 1000) PSta.AttackTrans(aval.Key, PSta.PosGet, PSta.SettingValues.Rig.transform.eulerAngles);
            }
            var useGID = -1;
            if (Cont == null) return;
            if (Cont.In_ShortCutSet > 0)
            {
                var LPlayer = LPlayerCharas[PSta.CharaID];
                if (PSta.BotID < 0)
                {
                    var SInput = Cont.In_ShortCutSet + (!PSta.PlayerValues.ShortCutBack ? 0 : 10) - 1;
                    if(SInput < LPlayer.ShortCutSets.Count)useGID = LPlayer.ShortCutSets[SInput];
                }
                else useGID = LPlayer.BotSets[Cont.In_ShortCutSet - 1];
            }
            Cont.In_ShortCutSet = 0;

            if (useGID < 0) return;
            PSta.ItemGIDUse(useGID,-1);
            PSta.AttackInput(useGID, false, true);
            PSta.AttackInput(useGID, true, true);
        }
    }
}
