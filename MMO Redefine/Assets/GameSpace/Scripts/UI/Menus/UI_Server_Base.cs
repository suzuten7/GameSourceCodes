namespace UIs
{
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalValues;
    using static FNet.Fusion_NetValue;
    using static GmSystem.GS_GlobalState;
    using static Datas.Data_Get;
    using static GmSystem.GS_ChangeSet;
    public class UI_Server_Base : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI RoomInfoTx;
        [SerializeField] List<UI_Server_Player> PlayerUIs;
        [SerializeField] TMP_InputField LvAddIn;
        [SerializeField] TMP_Dropdown DificDr;
        [SerializeField] TextMeshProUGUI DificValTx;
        [SerializeField] TMP_InputField PvPDamMultIn;

        void Update()
        {
            var runner = InsRunner;
            var PCount = 0;
            var BCount = 0;
            for(int i = 0; i < Mathf.Max(PStaList.Count, PlayerUIs.Count); i++)
            {
                if (PlayerUIs.Count <= i) PlayerUIs.Add(Instantiate(PlayerUIs[0], PlayerUIs[0].transform.parent));
                var pui = PlayerUIs[i];
                if (i >= PStaList.Count)
                {
                    ChangeActive(pui.gameObject, false);
                    continue;
                }

                var psta = PStaList[i];
                if (psta == null || !psta.gameObject.activeSelf)
                {
                    ChangeActive(pui.gameObject, false);
                    continue;
                }
                ChangeActive(pui.gameObject, true);
                pui.PSta = psta;
                if (psta.BotID < 0) PCount++;
                else BCount++;
                ChangeText(pui.PlayerID,"プレイヤー" + (psta.BotID < 0 ? PCount : BCount));
                ChangeText(pui.Name,psta.CommonValues.Name);
                TeamGet_Str((int)psta.CommonValues.Team, out var tcol);
                tcol.a = pui.BackUI.color.a;
                ChangeColor(pui.BackUI,tcol);
                ChangeTexture(pui.Icon, psta.ModelSet.PlayerIconGet(out _), true);
                var jobstr = "(";
                for (int k = 0; k < psta.PlayerValues.Jobs.Length; k++)
                {
                    if (k > 0) jobstr += "×";
                    jobstr += DB.JobDatas[psta.PlayerValues.Jobs[k].ID].Name;
                }
                jobstr += ")";
                ChangeText(pui.Job, jobstr);
                ChangeActive(pui.KickButton, runner.IsServer && psta != MyPlayer);
            }
            if (NetValue != null)
            {
                if (runner.GameMode != Fusion.GameMode.Single)
                {
                    var roomstr = "ルームID:" + runner.SessionInfo.Name;
                    roomstr += "\nルーム人数:" + runner.ActivePlayers.Count() + "/" + runner.SessionInfo.MaxPlayers;
                    roomstr += "\n接続状態:";
                    switch (runner.GameMode)
                    {
                        default: roomstr += runner.GameMode; break;
                        case Fusion.GameMode.Server: roomstr += "サーバー"; break;
                        case Fusion.GameMode.Host: roomstr += "ホスト"; break;
                        case Fusion.GameMode.Client: roomstr += "クライアント"; break;
                    }
                    ChangeText(RoomInfoTx,roomstr);
                }
                else ChangeText(RoomInfoTx,"オフライン");

                ChangeText(LvAddIn,NetValue.LvAdd.ToString(),true);
                ChangeValue(DificDr,NetValue.Dific);
                var difStr = "HP" + (DificChange(0, Enum_DifcVal.HP) * 100).ToString("F0") + "%\n";
                difStr += "攻撃力" + (DificChange(0, Enum_DifcVal.Atk) * 100).ToString("F0") + "%\n";
                difStr += "防御力" + (DificChange(0, Enum_DifcVal.Def) * 100).ToString("F0") + "%\n";
                difStr += "EXP" + (DificChange(0, Enum_DifcVal.EXP) * 100).ToString("F0") + "%";
                ChangeText(DificValTx, difStr);

                ChangeText(PvPDamMultIn, NetValue.PvPDamMult.ToString(),true);
            }
        }
        public void DificSet()
        {
            if (!InsRunner.IsServer) return;
            NetValue.Dific = DificDr.value;
        }
        public void LvAddSet()
        {
            if (!InsRunner.IsServer) return;
            NetValue.LvAdd = int.TryParse(LvAddIn.text,out var oLv) ? oLv : NetValue.LvAdd;
        }
        public void PvPDamMultSet()
        {
            if (!InsRunner.IsServer) return;
            NetValue.PvPDamMult = int.TryParse(PvPDamMultIn.text, out var oMult) ? oMult : NetValue.PvPDamMult;
        }
    }
}
