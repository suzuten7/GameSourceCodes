using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class Net_Log : NetworkBehaviour
{
    static public Net_Log NetLog;
    [System.Serializable]
    public class Log
    {
        public string time;
        public LogType type;
        [TextArea] public string mes;
    }
    public enum LogType
    {
        PlayerIO,
        ChatTeam,
        ChatOther,
        Kill,
        FFSD,
        UltUse,
        Event,
        System,
    }
    static public List<Log> Logs = new();
    static public bool[] ViewSets = new bool[10];
    [SerializeField] List<Log> editlogs;
    void Start()
    {
        NetLog = this;
    }
    void Update()
    {
        editlogs = Logs;
    }
    static public void LogAdd(float times, int types, string mess)
    {
        int time_i = Mathf.FloorToInt(times);
        int time_s = time_i % 60;
        int time_m = time_i / 60 % 60;
        int time_h = time_i / 3600;
        var tstr = "<size=50%>" + time_h.ToString("D2") + ":" + time_m.ToString("D2") + ":" + time_s.ToString("D2") + "</size>";
        Logs.Add(new Log { time = tstr, type = (LogType)types, mes = mess });
        var logMax = Mathf.FloorToInt(UI_OptionManager.OptionGetFloat("GP_Option 18", 100f));
        if (Logs.Count > logMax) Logs.RemoveAt(0);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_LogAdd(float time,int type,NetworkString<_32> nstr)
    {
        LogAdd(time, type, LocalizSystem.LocailzSCInfo(nstr.Value));
    }
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RPC_LogAdd(float time, int type, NetworkString<_32> nstr, NetworkString<_64> astr)
    {
        LogAdd(time, type, LocalizSystem.LocailzSCInfo(nstr.Value) + "\n" + astr.Value);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_LogAddPl(float time, int type, NetworkString<_64> nstr,Player_Manager pm)
    {
        var colstr = "<color=#" + ColorUtility.ToHtmlStringRGB(Data_Base.TeamColorGet(pm.states.teamID)) + ">";
        LogAdd(time, type, LocalizSystem.LocailzSCInfo(nstr.Value) + "\n" + colstr + pm.states.name + "</color>");
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_LogAddTeamStrPl(float time, int type, int teamid, NetworkString<_64> nstr, Player_Manager pm)
    {
        var colstr1 = "<color=#" + ColorUtility.ToHtmlStringRGB(Data_Base.TeamColorGet(teamid)) + ">";
        var colstr2 = "<color=#" + ColorUtility.ToHtmlStringRGB(Data_Base.TeamColorGet(pm.states.teamID)) + ">";
        LogAdd(time, type, colstr1 + LocalizSystem.LocailzSCInfo(nstr.Value) + "</color>\n" + colstr2 + pm.states.name + "</color>");
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_LogAddFlag(float time, int type, int teamid, NetworkString<_64> nstr)
    {
        var colstr1 = "<color=#" + ColorUtility.ToHtmlStringRGB(Data_Base.TeamColorGet(teamid)) + ">";
        LogAdd(time, type, colstr1 + LocalizSystem.LocailzSCInfo(nstr.Value) + "</color>");
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_LogAddArea(float time, int type, int teamid, NetworkString<_32> nstr, NetworkString<_32> astr, NetworkString<_32> estr)
    {
        var colstr1 = "<color=#" + ColorUtility.ToHtmlStringRGB(Data_Base.TeamColorGet(teamid)) + ">";
        LogAdd(time, type, colstr1 + LocalizSystem.LocailzSCInfo(nstr.Value) + LocalizSystem.LocailzSCInfo(astr.Value) + LocalizSystem.LocailzSCInfo(estr.Value) + "</color>");
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_LogAddKill(float time, int type, NetworkString<_64> nstr,Player_Manager pm1,int atkID, Player_Manager pm2)
    {
        var colstr1 = "<color=#" + ColorUtility.ToHtmlStringRGB(Data_Base.TeamColorGet(pm1.states.teamID)) + ">";
        var colstr2 = "<color=#" + ColorUtility.ToHtmlStringRGB(Data_Base.TeamColorGet(pm2.states.teamID)) + ">";

        var pstr1 = colstr1 + pm1.states.name + "</color>";
        var pstr2 = colstr2 + pm2.states.name + "</color>";
        var astr = "(???)";
        try
        {
            if(atkID < (int)AtkID.Gun)
            {
                switch (atkID)
                {
                    case -10000: astr = $"({LocalizSystem.LocailzSCInfo("窒息死")})"; break;
                    case -10001: astr = $"({LocalizSystem.LocailzSCInfo("地形死")})"; break;
                }
            }
            else if (atkID >= (int)AtkID.Gun && atkID < (int)AtkID.Gun + (int)AtkID.CSize)
            {
                astr = $"({LocalizSystem.LocailzString("GunName", Data_Base.DB.guns[atkID].name)})";
            }
            else if (atkID < (int)AtkID.Melee + (int)AtkID.CSize)
            {
                astr = $"({LocalizSystem.LocailzString("MeleeName",Data_Base.DB.melles[atkID - (int)AtkID.Melee].name)})";
            }
            else if (atkID < (int)AtkID.Gadget + (int)AtkID.CSize)
            {
                astr = $"({LocalizSystem.LocailzString("GadgetName", Data_Base.DB.gadgets[atkID - (int)AtkID.Gadget].name)})";
            }
            else if (atkID < (int)AtkID.Ult + (int)AtkID.CSize)
            {
                astr = $"({LocalizSystem.LocailzString("UltName",Data_Base.DB.ults[atkID - (int)AtkID.Ult].name)})";
            }
        }
        catch { }
        if(pm1 != pm2) LogAdd(time,type, LocalizSystem.LocailzSCInfo(nstr.Value) + "\n" + pstr1 + "\n" + astr + "=>" + pstr2);
        else LogAdd(time,type, LocalizSystem.LocailzSCInfo(nstr.Value) + "\n" + pstr1 + "<=" + astr);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_LogAddUlt(float time, int type, Player_Manager pm, int ultID)
    {
        var colstr1 = "<color=#" + ColorUtility.ToHtmlStringRGB(Data_Base.TeamColorGet(pm.states.teamID)) + ">";
        var pstr1 = colstr1 + pm.states.name + "</color>";
        var ultd = Data_Base.DB.ults[ultID];
        LogAdd(time, type, $"{LocalizSystem.LocailzSCInfo("必殺発動!!!")}\n{pstr1}({LocalizSystem.LocailzString("UltName", ultd.name)})");
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Chat(float time, int type, Player_JoinObj mjoin, NetworkString<_64> cstr)
    {
        LogAdd(time, type, "<Color=#888888>" + mjoin.name + "</color>" + "\n" + cstr);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_Chat(float time, int type, Player_Manager pm, NetworkString<_64> cstr,bool teamOnly)
    {
        if (teamOnly && (Obj_LocalObjects.MyPlayer == null || Obj_LocalObjects.MyPlayer.states.teamID != pm.states.teamID)) return;
        var colstr = "<color=#" + ColorUtility.ToHtmlStringRGB(Data_Base.TeamColorGet(pm.states.teamID)) + ">";
        LogAdd(time, type, colstr + pm.states.name + "</color>" + "\n" + cstr);
    }
}
