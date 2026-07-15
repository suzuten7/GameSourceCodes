
namespace UIs
{
    using TMPro;
    using UnityEngine;
    using static Datas.Data_Get;
    using static Player.Player_Controle;
    using Datas;
    using static GmSystem.GS_SaveValues;
    using UnityEngine.UI;
    using static GmSystem.GS_ChangeSet;
    public class UI_ShortCutInfos : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        [SerializeField] Image ShortCutFBImg;
        [SerializeField] Image WepFBImg;
        [SerializeField] GameObject InfoUIs;
        [SerializeField] TextMeshProUGUI NameTx;
        [SerializeField] TextMeshProUGUI InfoTx;

        void LateUpdate()
        {
            var selID = PCont.Sel_ShortCutSet;
            var LPChara = CharaUI.LChara;
            if (LPChara == null) return;
            ChangeColor( ShortCutFBImg, !CharaUI.Sta.PlayerValues.ShortCutBack ? Color.white : new Color(0.5f, 0.5f, 0.5f));
            ChangeColor( WepFBImg, !CharaUI.Sta.PlayerValues.WepBack ? Color.white : new Color(0.5f, 0.5f, 0.5f));
            if (CharaUI.Sta.PlayerValues.ShortCutBack) selID += 10;
            var GID = LPChara.ShortCutSets[selID];
            var slotData = ItemGIDDataGet(GID);
            if (slotData == null)
            {
                ChangeActive(InfoUIs, false);
            }
            else
            {
                ChangeActive(InfoUIs, true);
                ChangeText( NameTx,slotData.Name);
                var infotx = "";
                switch (slotData)
                {
                    case Data_Consumables:
                        if (!LPlayerVal.ConsumablesDic.ContainsKey(GID)) infotx = "<color=#FF00FF>所持数:0</color>";
                        else infotx = "<color=#FF00FF>所持数:" + LPlayerVal.ConsumablesDic[GID] + "</color>";
                        break;
                    case Data_Attack:
                        var atkData = (Data_Attack)slotData;
                        if (!SkillCheck(atkData, CharaUI.LChara)) infotx += "<color=#FF8888>(※未習得)</color>";
                        infotx += atkData.CostStrs(false);
                        if (infotx != "") infotx += ",";
                        infotx += "<color=#00FF00>CT:" + atkData.CT + "</color>";
                        break;
                }
                ChangeText(InfoTx, infotx);
            }
        }
    }
}

