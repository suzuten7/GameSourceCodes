using TMPro;
using UnityEngine;

public class UI_GameRuleView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI teamsTx;
    [SerializeField] TextMeshProUGUI[] gameOptionTxs;

    void Update()
    {
        var teamStr = "";
        for (int i = 0; i < 5; i++)
        {
            if (!Net_Value.NetCheck || !Net_Value.NetValue.teamOn[i]) continue;
            var col = Data_Base.TeamColorGet(i);
            teamStr += $"<color=#{ColorUtility.ToHtmlStringRGB(col)}>{i + 1}</color>";
        }
        teamsTx.text = $"{LocalizSystem.LocailzSCInfo("ランダムチーム")}:{teamStr}";
        gameOptionTxs[0].text = $"{LocalizSystem.LocailzSCInfo("チーム変更")}:{LocalizSystem.LocailzSCInfo(Net_Value.NetCheck && Net_Value.NetValue.options[0] ? "許可" : "禁止")}";
        gameOptionTxs[1].text = $"{LocalizSystem.LocailzSCInfo("装備変更")}:{LocalizSystem.LocailzSCInfo(Net_Value.NetCheck && Net_Value.NetValue.options[1] ? "許可" : "禁止")}";
        gameOptionTxs[2].text = $"{LocalizSystem.LocailzSCInfo("チート使用")}:{LocalizSystem.LocailzSCInfo(Net_Value.NetCheck && Net_Value.NetValue.options[2] ? "許可" : "禁止")}";
        gameOptionTxs[3].text = $"{LocalizSystem.LocailzSCInfo("スコアエリア")}:{LocalizSystem.LocailzSCInfo(Net_Value.NetCheck && Net_Value.NetValue.options[3] ? "有り" : "無し")}";
        gameOptionTxs[4].text = $"{LocalizSystem.LocailzSCInfo("旗")}:{LocalizSystem.LocailzSCInfo(Net_Value.NetCheck && Net_Value.NetValue.options[4] ? "有り" : "無し")}";
    }
}
