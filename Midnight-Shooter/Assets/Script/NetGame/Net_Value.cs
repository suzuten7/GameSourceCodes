using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Net_Value : NetworkBehaviour
{
    static public Net_Value NetValue;
    static public bool NetCheck
    {
        get
        {
            return NetValue != null && NetValue.Object != null && Net_Connect.InsRunner != null;
        }
    }
    [SerializeField]public GameObject playerUI;
    [Networked]public float gameTime { get; set; }
    [Networked] public bool timeLimit { get; set; }
    /// <summary>0チーム変更,1装備変更,2チート許可,3エリア使用,4旗使用 </summary>
    [Networked, Capacity(10)] public NetworkArray<bool> options => default;
    [Networked] public int killConsLight { get; set; }
    /// <summary>0与ダメ,1与回復,2FFダメ,3敵回復,4通常キル,5連続キル,6自滅,7FF,8エリア滞在,9エリア占拠,10旗,11連続キル阻止 </summary>
    [Networked, Capacity(12)] public NetworkArray<float> scoreMults => default;
    [Networked, Capacity(5)] public NetworkArray<bool> teamOn => default;

    [Networked, Capacity(5)] public NetworkArray<int> teamKills => default;
    [Networked, Capacity(5)] public NetworkArray<int> teamKills_back => default;
    [Networked, Capacity(5)] public NetworkArray<float> teamScores => default;
    [Networked, Capacity(5)] public NetworkArray<float> teamScores_back => default;
    [Networked, Capacity(5)] public NetworkArray<bool> teamUses_back => default;
    bool start = false;
    bool gameEnd = false;
    bool logs = false;
    public void Start()
    {
        NetValue = this;
        playerUI.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Object != null && !logs)
        {
            logs = true;
            Net_Log.LogAdd(gameTime, (int)Net_Log.LogType.System, "ステージ変更");
        }
        if (!Net_Connect.CanControl(Object)) return;
        UI_GameRuleSetting.StartSets();
        if (!start)
        {
            start = true;
            gameTime = 0;
            for (int i = 0; i < teamKills.Length; i++) teamKills.Set(i, 0);
            for (int i = 0; i < teamScores.Length; i++) teamScores.Set(i, 0);
        }
        if (!NetCheck) return;
        for (int i = 0; i < teamOn.Length; i++)
        {
            if (teamOn[i] != UI_GameRuleSetting.teamOns[i]) teamOn.Set(i, UI_GameRuleSetting.teamOns[i]);
        }
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i] != UI_GameRuleSetting.options[i]) options.Set(i, UI_GameRuleSetting.options[i]);
        }
        if(killConsLight != UI_GameRuleSetting.killConsLight) killConsLight = UI_GameRuleSetting.killConsLight;
        for (int i = 0; i < scoreMults.Length; i++)
        {
            if (scoreMults[i] != UI_GameRuleSetting.scoreMults[i]) scoreMults.Set(i, UI_GameRuleSetting.scoreMults[i]);
        }

        if (Obj_LocalObjects.TimeStopd) return;
        if (!timeLimit)
        {
            gameTime += Time.deltaTime;
            gameEnd = false;
        } 
        else
        {
            gameTime -= Time.deltaTime;
            if(gameTime <= 0 && !gameEnd)
            {
                gameEnd = true;
                Net_Log.NetLog.RPC_LogAdd(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.System, "時間切れ!!!");
                for (int i = 0; i < teamKills.Length; i++) teamKills_back.Set(i,teamKills[i]);
                for (int i = 0; i < teamScores.Length; i++) teamScores_back.Set(i, teamScores[i]);
                for (int i = 0; i < teamUses_back.Length; i++) teamUses_back.Set(i, Obj_LocalObjects.TeamUsed[i]);
                for (int i = 0; i < Obj_LocalObjects.Players.Count; i++)
                {
                    var pl = Obj_LocalObjects.Players[i];
                    pl.RPC_GameEnd();
                }
                RPC_ResultOpen();
                timeLimit = false;
            }
        }
    }
    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    public void RPC_KillChange(int teamid,bool rem)
    {
        teamKills.Set(teamid, teamKills[teamid] + (!rem ? 1 : -1));
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ScoreChange(int teamid, float val)
    {
        teamScores.Set(teamid, teamScores[teamid] + val);
    }

    static public void SoundSet(Vector3 pos, Player_Manager pm, float rangeMax, float lastTime,AudioClip se = null,float seVolume = 1f,float sePitch = 1f)
    {
        if (rangeMax <= 0) return;
        var seID = Data_Base.DB.ses.IndexOf(se);
        NetValue.RPC_SoundSet(pos, pm, rangeMax, lastTime,seID,seVolume,sePitch);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_SoundSet(Vector3 pos,Player_Manager pm,float rangeMax,float lastTime,int seID,float seVolue,float sePitch)
    {
        var so = Instantiate(Data_Base.DB.soundObj,pos,Quaternion.identity);
        so.my = pm == Obj_LocalObjects.MyPlayer;
        so.userTeam = pm != null ? pm.states.teamID : -1;
        so.rangeMax = rangeMax;
        so.lastTime = lastTime;
        if (seID >= 0)
        {
            var sed = Data_Base.DB.ses[seID];
            so.se.clip = sed;
            if (so.my) seVolue *= UI_OptionManager.OptionGetFloat("S_Option 05", 50) * 0.01f;
            else if(Obj_LocalObjects.MyPlayer != null && pm.states.teamID == Obj_LocalObjects.MyPlayer.states.teamID) seVolue *= UI_OptionManager.OptionGetFloat("S_Option 06", 30) * 0.01f;
            else seVolue *= UI_OptionManager.OptionGetFloat("S_Option 07", 70) * 0.01f;
            so.se.volume = seVolue;
            so.se.pitch = sePitch;
            so.se.maxDistance = rangeMax;

            so.se.Play();
        }
        else so.se.enabled = false;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ResultOpen()
    {
        if (UI_Results.ResultUI != null) UI_Results.ResultUI.UIs.SetActive(true);
    }
}
