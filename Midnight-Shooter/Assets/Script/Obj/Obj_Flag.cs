using Fusion;
using UnityEngine;

public class Obj_Flag : NetworkBehaviour
{
    [SerializeField] SpriteRenderer sr;
    public Obj_SpawneArea sarea;
    [SerializeField] float addScore;
    [SerializeField] float resetTime;

    [Networked] public Player_Manager pm { get; set; }
    float rtime = 0;
    public bool gets = false;
    public bool flagUse = false;

    private void Start()
    {
        Obj_LocalObjects.Flags.Add(this);
    }

    void Update()
    {
        if (Object == null) return;
        flagUse = Net_Value.NetValue.options[4] && Obj_LocalObjects.TeamUsed[sarea.teamID];
        if (!flagUse)
        {
            sr.gameObject.SetActive(false);
            Resets(false);
            return;
        }
        sr.gameObject.SetActive(true);
        var col = Data_Base.TeamColorGet(sarea.teamID);
        col.a = sr.color.a;
        sr.color = col;
        if (!Net_Connect.CanControl(Object)) return;
        if (pm == null)
        {
            if (Obj_LocalObjects.TimeStopd) return;
            rtime += Time.deltaTime;
            if (rtime >= resetTime) Resets(true);
        }
        else
        {
            transform.position = pm.PosGet;
            var rot = transform.eulerAngles;
            rot.z += Time.deltaTime * 90;
            transform.eulerAngles = rot;
            rtime = 0;
            if (pm.hpTotal <= 0) pm = null;
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!Net_Connect.CanControl(Object)) return;
        if (!flagUse) return;
            if (pm != null)
        {
            if (!col.TryGetComponent<Obj_SpawneArea>(out var hsarea)) return;
            if(pm.states.teamID == hsarea.teamID)
            {
                Net_Log.NetLog.RPC_LogAddTeamStrPl(Net_Value.NetValue.gameTime,(int)Net_Log.LogType.Event,sarea.teamID, "旗持ち帰り!!!", pm);
                pm.ScoreChange(addScore * Net_Value.NetValue.scoreMults[10]);
                Resets(false);
            }
        }
        else
        {
            if (!col.TryGetComponent<Player_Hit>(out var phit)) return;
            if (sarea.teamID == phit.pm.states.teamID)
            {
                Resets(true);
            }
            else
            {
                gets = true;
                pm = phit.pm;
                Net_Log.NetLog.RPC_LogAddTeamStrPl(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.Event, sarea.teamID, "旗略奪!!!", pm);
            }
        }
    }
    public void Resets(bool log)
    {
        if(log &&gets) Net_Log.NetLog.RPC_LogAddFlag(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.Event, sarea.teamID, "旗リセット!!!");
        pm = null;
        gets = false;
        rtime = 0;
        transform.position = sarea.transform.position;
        transform.rotation = Quaternion.identity;
    }
}
