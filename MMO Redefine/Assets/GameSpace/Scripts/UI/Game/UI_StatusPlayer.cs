
namespace UIs
{
    using TMPro;
    using UnityEngine;
    using Player;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_SaveValues;
    using static Datas.Data_Get;
    using UnityEngine.UI;
    using static GmSystem.GS_ChangeSet;
    public class UI_StatusPlayer : UI_StatusBase
    {
        [SerializeField] RawImage Icon;
        [SerializeField] TextMeshProUGUI JobTx;
        protected override void NameLVSets()
        {
            var PSta = Sta.GetComponent<Player_State>();
            var LVStr = Sta.CommonValues.LV.ToString();
            var LVCol = Color.white;
            if (CanControl(PSta.Object))
            {
                var lchara = LPlayerCharas[PSta.CharaID];
                if (lchara.SetLV > 0)
                {
                    LVStr += "(固定)";
                    LVCol = Color.magenta;
                }
            }
            ChangeText(LVTx,LVStr);
            ChangeColor(LVTx,LVCol);
            var NameStr = Sta.CommonValues.Name;
            ChangeText(NameTx,NameStr);
            var IconIm = PSta.ModelSet.PlayerIconGet(out _);
            ChangeTexture(Icon,IconIm, true);
            var PJob = PSta.PlayerValues.Jobs;
            var JobStr = DB.JobDatas[PJob[0].ID].Name + "×" + DB.JobDatas[PJob[1].ID].Name;
            ChangeText(JobTx,JobStr);
        }
    }
}

