using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameInfo : MonoBehaviour
{
    public TextMeshProUGUI timetx;
    public TextMeshProUGUI[] teamtxs;
    public Toggle playerSortTeamTo;
    public TMP_Dropdown playerSortDr;
    public List<TextMeshProUGUI> playerTxs;
    public TextMeshProUGUI costOverText;
    public TextMeshProUGUI cheatText;
    int logCount = 0;
    void Update()
    {
        if (!Net_Value.NetCheck) return;
        int time_i = Mathf.FloorToInt(Net_Value.NetValue.gameTime);
        int time_s = time_i % 60;
        int time_m = time_i / 60 % 60;
        int time_h = time_i / 3600;
        timetx.text = time_h.ToString("D2") + ":" + time_m.ToString("D2") + ":" + time_s.ToString("D2");
        if (Obj_LocalObjects.TimeStopd)
        {
            timetx.color = Color.gray;
        }
        else if (Net_Value.NetValue.timeLimit)
        {
            if(Net_Value.NetValue.gameTime > 10f)
            {
                timetx.color = new Color(1f, 0.5f, 0.5f);
            }
            else
            {
                var tch = (Mathf.Sin(Net_Value.NetValue.gameTime * 5f) + 1f) * 0.5f * 0.5f;
                timetx.color = new Color(1f, tch, tch);
            }
        }
        else
        {
            timetx.color = Color.white;
        }
        for (int i = 0; i < teamtxs.Length; i++)
        {
            if (Obj_LocalObjects.TeamUsed[i])
            {
                teamtxs[i].text = Net_Value.NetValue.teamKills[i] + ":" + Net_Value.NetValue.teamScores[i].ToString("F0");
                teamtxs[i].gameObject.SetActive(true);
            }
            else
            {
                teamtxs[i].gameObject.SetActive(false);
            }
        }
        var plList = Obj_LocalObjects.Players.ToList();
        IOrderedEnumerable<Player_Manager> listb;
        if (playerSortTeamTo.isOn) listb = plList.OrderBy(x => x.states.teamID);
        else listb = plList.OrderBy(x => 0);
        switch (playerSortDr.value)
        {
            case 0:
                listb = listb.ThenByDescending(x => x.values.kill);
                break;
            case 1:
                listb = listb.ThenByDescending(x => x.values.score);
                break;
            case 2:
                listb = listb.ThenByDescending(x => x.values.killcons);
                break;
            case 3:
                listb = listb.ThenByDescending(x => x.values.killcmax);
                break;
            case 4:
                listb = listb.ThenBy(x => x.states.name);
                break;
        }
        plList = listb.ThenBy(x => x.Object != null ? x.Object.Id.Raw : uint.MaxValue).ToList();

        var viewList = Obj_LocalObjects.JoinPls.ToList();
        for (int i = viewList.Count - 1; i >= 0; i--)
        {
            var view = viewList[i];
            if (view == null)
            {
                viewList.RemoveAt(i);
                continue;
            }
            for (int k = 0; k < plList.Count; k++)
            {
                var pm = plList[k];
                if (pm == null) continue;
                if (view.Object.InputAuthority != pm.Object.InputAuthority) continue;
                viewList.RemoveAt(i);
                break;
            }
        }
        for (int i = 0; i < Mathf.Max(plList.Count + viewList.Count, playerTxs.Count); i++)
        {
            if (i >= plList.Count + viewList.Count)
            {
                playerTxs[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= playerTxs.Count) playerTxs.Add(Instantiate(playerTxs[0], playerTxs[0].transform.parent));
            var ptx = playerTxs[i];
            if (i < plList.Count)
            {
                var pm = plList[i];
                if (pm == null)
                {
                    ptx.gameObject.SetActive(false);
                    continue;
                }
                ptx.gameObject.SetActive(true);
                ptx.color = Data_Base.TeamColorGet(pm.states.teamID);
                var str = pm.states.name;
                str += "\n<size=70%>";
                str += "Kill" + pm.values.kill;
                str += $"[{LocalizSystem.LocailzSCInfo("連続")}:{pm.values.killcons}Max{pm.values.killcmax}]";
                str += "\nScore" + pm.values.score.ToString("F0");
                str += "</size>";
                ptx.text = str;
            }
            else if (i < plList.Count + viewList.Count)
            {
                var vp = viewList[i - plList.Count];
                if (vp == null)
                {
                    ptx.gameObject.SetActive(false);
                    continue;
                }
                ptx.gameObject.SetActive(true);
                ptx.color = Color.gray;
                ptx.text = vp.name;
                ptx.text += $"\n<size=70%>({LocalizSystem.LocailzSCInfo("観戦")})</size>";
            }


        }
        var costOver = false;
        var cheat = false;
        if (!Net_Value.NetValue.options[2] && Obj_LocalObjects.MyPlayer !=null)
        {
            int cost = 0;
            for(int i = 0; i < Obj_LocalObjects.MyPlayer.states.passives.Count; i++)
            {
                var pss = Obj_LocalObjects.MyPlayer.states.passives[i];
                var psd = Data_Base.PassiveDGet((Passive)pss.x);
                if (psd != null) cost += psd.CostGet(pss.y);
                if (pss.x >= (int)Passive.Cheat)
                {
                    cheat = true;
                    break;
                }
            }
            costOver = cost > Data_Base.DB.CostMax;
        }
        costOverText.gameObject.SetActive(costOver);
        cheatText.gameObject.SetActive(cheat);
        if (cheat)
        {
            var cc = Color.HSVToRGB(Mathf.Repeat(Time.time,1f), 1, 1);
            cc.a = 0.8f;
            cheatText.color = cc;
        }
    }


}
