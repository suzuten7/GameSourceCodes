using Fusion;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Obj_ScoreArea : NetworkBehaviour
{
    public string areaName;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] TextMeshProUGUI areaTx;
    public float areaMaxTime;
    [SerializeField] float addScore;
    [SerializeField] float areaScore;
    [Networked] public int nowTeam { get; set; }
    [Networked] public int chTeam { get; set; }
    [Networked] public float areaTime { get; set; }

    float addtime = 0;
    Dictionary<Player_Manager, float> scorePls = new();
    private void Start()
    {
        Obj_LocalObjects.Areas.Add(this);
    }
    private void Update()
    {
        if (Object == null) return;
        if (!Net_Value.NetValue.options[3])
        {
            sr.gameObject.SetActive(false);
            areaTx.text = "";
            Resets();
            return;
        }
        sr.gameObject.SetActive(true);
        var colsr = Data_Base.TeamColorGet(nowTeam);
        colsr.a = sr.color.a;
        sr.color = colsr;
        areaTime = Mathf.Clamp(areaTime, 0, areaMaxTime);
        var coltx = Data_Base.TeamColorGet(chTeam);
        coltx.a = areaTx.color.a;
        areaTx.color = coltx;
        areaTx.text = LocalizSystem.LocailzSCInfo("エリア") + areaName + "\n" + (areaTime / areaMaxTime * 100).ToString("F0") + "%";
        if (!Net_Connect.CanControl(Object)) return;
        if (Obj_LocalObjects.TimeStopd) return;
        if (nowTeam >= 0)
        {
            addtime += Time.deltaTime;
            if (addtime >= 1)
            {
                addtime = 0;
                Net_Value.NetValue.RPC_ScoreChange(nowTeam, addScore * Net_Value.NetValue.scoreMults[9]);
            }
        }
        else addtime = 0;
        var spl = scorePls.Keys.ToArray();
        for(int i=spl.Length-1;i>=0;i--)
        {
            scorePls[spl[i]] -= Time.deltaTime;
            if(scorePls[spl[i]] <= 0)
            {
                scorePls.Remove(spl[i]);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (!Net_Connect.CanControl(Object)) return;
        if (!Net_Value.NetValue.options[3]) return;
        if (Obj_LocalObjects.TimeStopd) return;
        if (col.TryGetComponent<Player_Hit>(out var phit))
        {
            if (phit.pm.hpTotal <= 0) return;
            if(phit.pm.states.teamID != chTeam)
            {
                areaTime -= Time.deltaTime;
                if(areaTime <= 0)
                {
                    chTeam = phit.pm.states.teamID;
                    if (nowTeam >= 0) Net_Log.NetLog.RPC_LogAddArea(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.Event, nowTeam, "エリア", areaName,"消失!!!");
                    nowTeam = -1;
                }
            }
            else
            {
                areaTime += Time.deltaTime;
                if (areaTime >= areaMaxTime)
                {
                    if(nowTeam < 0) Net_Log.NetLog.RPC_LogAddArea(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.Event, chTeam, "エリア",areaName,"占拠!!!");
                    nowTeam = phit.pm.states.teamID;
                }
            }
            if (!scorePls.ContainsKey(phit.pm))
            {
                scorePls.Add(phit.pm, 1);
                phit.pm.ScoreChange(areaScore * Net_Value.NetValue.scoreMults[8]);
            }
        }
    }

    public void Resets()
    {
        nowTeam = -1;
        chTeam = -1;
        areaTime = 0;
    }

}
