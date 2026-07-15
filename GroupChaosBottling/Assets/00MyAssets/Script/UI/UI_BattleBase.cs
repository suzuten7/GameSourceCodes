using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static BattleManager;
public class UI_BattleBase : MonoBehaviour
{
    [SerializeField] GameObject InfoUI;
    [SerializeField] TextMeshProUGUI DifeTx;
    [SerializeField] TextMeshProUGUI TimeTx;
    [SerializeField] TextMeshProUGUI DeathTx;

    [SerializeField] TextMeshProUGUI TimeStarTx;
    [SerializeField] TextMeshProUGUI DeathStarTx;

    void LateUpdate()
    {
        if (BTManager.StageD == null)
        {
            if (InfoUI.activeSelf != false) InfoUI.SetActive(false);
            return;
        }
        if (BTManager.StageD.TimeLimSec > 0)
        {
            if (InfoUI.activeSelf != true) InfoUI.SetActive(true);
            var DifeStr = "ノーマル";
            switch (BTManager.Dife)
            {
                case 0:DifeStr = "イージー";break;
                case 2: DifeStr = "ハード"; break;
                case 3: DifeStr = "ベリーハード"; break;
            }
            if (BTManager.Chaos) DifeStr += "<color=#FF00FF>+カオス</color>";
            if (DifeTx.text != DifeStr) DifeTx.text = DifeStr;
            var TimeStr = (BTManager.Time / 3600).ToString("D2") + ":" + (BTManager.Time / 60 % 60).ToString("D2");
            if (TimeTx.text != TimeStr) TimeTx.text = TimeStr;

            var DeathStr = "Death:" + BTManager.DeathCount;
            if (DeathTx.text != DeathStr) DeathTx.text = DeathStr;
            var TStarStr = "";
            if (BTManager.StageD.DifencePer <= 0)
            {
                TStarStr = (BTManager.Time >= BTManager.StageD.TimeStar * 60 ? "★" : "☆");
                TStarStr += (BTManager.StageD.TimeStar / 60).ToString("D2") + ":" + (BTManager.StageD.TimeStar % 60).ToString("D2");
            }
            else
            {
                float DefHPPer = 0;
                for(int i = 0; i < BTManager.BossList.Count; i++)
                {
                    var Boss = BTManager.BossList[i];
                    if (Boss != null && Boss.Team == 0) DefHPPer = Boss.HP / Mathf.Max(1f, Boss.FMHP) * 100f;
                }
                TStarStr = "耐久値" + DefHPPer.ToString("F1") + "%\n";
                TStarStr += (BTManager.StageD.DifencePer < DefHPPer ? "★" : "☆");
                TStarStr += "耐久値=>" + BTManager.StageD.DifencePer.ToString("F1") + "%";
            }
            if (TimeStarTx.text != TStarStr) TimeStarTx.text = TStarStr;

            var DStarStr = (BTManager.DeathCount <= BTManager.FDeathStar ? "★" : "☆");
            DStarStr += "Death:" + BTManager.FDeathStar;
            if (DeathStarTx.text != DStarStr) DeathStarTx.text = DStarStr;
        }
        else if (InfoUI.activeSelf != false) InfoUI.SetActive(false);


    }
}
