using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_Results : MonoBehaviour
{
    static public UI_Results ResultUI;
    public GameObject UIs;
    [SerializeField] TextMeshProUGUI winTeamTx;
    [SerializeField] TextMeshProUGUI[] teamScoreTxs;
    [SerializeField] List<TextMeshProUGUI> playerTxs;
    [SerializeField] Toggle playerSortTeamTo;
    [SerializeField] TMP_Dropdown playerSortDr;
    private void Start()
    {
        ResultUI = this;
        UIs.SetActive(false);
    }
    void Update()
    {
        if (!UIs.activeSelf) return;
        var winTeam = -1;
        var maxScore = int.MinValue;
        var drawTeam = -1;
        for (int i = 0; i < teamScoreTxs.Length; i++)
        {
            if (!Net_Value.NetValue.teamUses_back[i])
            {
                teamScoreTxs[i].gameObject.SetActive(false);
                continue;
            }
            teamScoreTxs[i].gameObject.SetActive(true);
            var score = Mathf.RoundToInt(Net_Value.NetValue.teamScores_back[i]);
            teamScoreTxs[i].text = Net_Value.NetValue.teamKills_back[i] + ":" + score;
            if(maxScore == score)
            {
                drawTeam = i;
            }
            else if(maxScore < score)
            {
                maxScore = score;
                winTeam = i;
                drawTeam = -1;
            }
        }
        if (drawTeam >= 0)
        {
            winTeamTx.color = Color.white;
            winTeamTx.text = "<color=#" + ColorUtility.ToHtmlStringRGB(Data_Base.TeamColorGet(winTeam)) + ">";
            winTeamTx.text += $"{LocalizSystem.LocailzSCInfo("チーム")}{(winTeam + 1)}</color>";
            winTeamTx.text += ":<color=#" + ColorUtility.ToHtmlStringRGB(Data_Base.TeamColorGet(drawTeam)) + ">";
            winTeamTx.text += $"{LocalizSystem.LocailzSCInfo("チーム")}{(drawTeam + 1)}</color>";
            winTeamTx.text += $" {LocalizSystem.LocailzSCInfo("引き分け...")}";
        }
        else if(winTeam >= 0)
        {
            winTeamTx.text = $"{LocalizSystem.LocailzSCInfo("チーム")}{(winTeam + 1)}{LocalizSystem.LocailzSCInfo("の勝利!!!")}";
            winTeamTx.color = Data_Base.TeamColorGet(winTeam);
        }
        else
        {
            winTeamTx.text = "";
        }
        var plList = Obj_LocalObjects.Players;
        IOrderedEnumerable<Player_Manager> listb;
        if (playerSortTeamTo.isOn) listb = plList.OrderBy(x => x.states.teamID);
        else listb = plList.OrderBy(x => 0);
        switch (playerSortDr.value)
        {
            case 0:
                listb = listb.ThenByDescending(x => x.values.kill_back);
                break;
            case 1:
                listb = listb.ThenByDescending(x => x.values.score_back);
                break;
            case 2:
                listb = listb.ThenByDescending(x => x.values.killcmax_back);
                break;
            case 3:
                listb = listb.ThenByDescending(x => x.values.addDamages_back);
                break;
            case 4:
                listb = listb.ThenByDescending(x => x.values.takeDamages_back);
                break;
            case 5:
                listb = listb.ThenByDescending(x => x.values.addHeals_back);
                break;
            case 6:
                listb = listb.ThenBy(x => x.states.name);
                break;
        }
        plList = listb.ThenBy(x => x.Object != null ? x.Object.Id.Raw : uint.MaxValue).ToList();
        for (int i = 0; i < Mathf.Max(plList.Count, playerTxs.Count); i++)
        {
            if (i >= plList.Count)
            {
                playerTxs[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= playerTxs.Count) playerTxs.Add(Instantiate(playerTxs[0], playerTxs[0].transform.parent));
            var pm = plList[i];
            var ptx = playerTxs[i];
            if (pm == null || !pm.gameObject.activeSelf || pm.values.noView)
            {
                ptx.gameObject.SetActive(false);
                continue;
            }
            ptx.gameObject.SetActive(true);
            ptx.color = Data_Base.TeamColorGet(pm.values.team_back);
            var str = pm.states.name;
            str += "\n<size=70%>";
            str += "Kill" + pm.values.kill_back;
            str += $"{LocalizSystem.LocailzSCInfo("最大連続")}{pm.values.killcmax_back}";
            str += ",Score" + pm.values.score_back.ToString("F0");
            str += $"\n{LocalizSystem.LocailzSCInfo("与ダメージ")}" + pm.values.addDamages_back.ToString("F0");
            str += $",{LocalizSystem.LocailzSCInfo("被ダメージ")}" + pm.values.takeDamages_back.ToString("F0");
            str += $",{LocalizSystem.LocailzSCInfo("与回復")}" + pm.values.addHeals_back.ToString("F0");
            str += "</size>";
            ptx.text = str;

        }
    }
}
