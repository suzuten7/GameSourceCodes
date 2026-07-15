using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_Logs : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> logTxs;
    [SerializeField] Image[] logViewSets;
    [SerializeField] TMP_InputField chatIn;
    [SerializeField] Toggle teamOnlyTo;
    static bool start = false;
    private void Start()
    {
        if (start) return;
        start = true;
        for (int i = 0; i < Net_Log.ViewSets.Length; i++) Net_Log.ViewSets[i] = true;
    }
    void Update()
    {
        for (int i = 0; i < Mathf.Max(Net_Log.Logs.Count, logTxs.Count); i++)
        {
            if (i >= Net_Log.Logs.Count)
            {
                logTxs[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= logTxs.Count) logTxs.Add(Instantiate(logTxs[0], logTxs[0].transform.parent));
            var log = Net_Log.Logs[i];
            var ltx = logTxs[i];
            if (!Net_Log.ViewSets[(int)log.type])
            {
                ltx.gameObject.SetActive(false);
                continue;
            }
            ltx.gameObject.SetActive(true);
            var str = log.time + "\n";
            str += log.mes;
            ltx.text = str;

        }
        for (int i = 0; i < logViewSets.Length; i++)
        {
            logViewSets[i].color = !Net_Log.ViewSets[i] ? Color.red : Color.green;
        }
    }


    public void Chat()
    {
        if (chatIn.text == "") return;
        if (Obj_LocalObjects.MyPlayer != null) Net_Log.NetLog.RPC_Chat(Net_Value.NetValue.gameTime, (int)(teamOnlyTo.isOn ? Net_Log.LogType.ChatTeam : Net_Log.LogType.ChatOther),Obj_LocalObjects.MyPlayer, chatIn.text, teamOnlyTo.isOn);
        else Net_Log.NetLog.RPC_Chat(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.ChatOther, Obj_LocalObjects.MyJoin, chatIn.text);
        chatIn.text = "";
    }
    public void ViewTypeSet(int id)
    {
        Net_Log.ViewSets[id] = !Net_Log.ViewSets[id];
    }
}
