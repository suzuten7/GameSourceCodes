using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_GameRuleSetting : MonoBehaviour
{
    static public bool start = false;
    /// <summary>0チーム変更,1装備変更,2チート許可,3エリア使用,4旗使用 </summary>
    static public bool[] options = new bool[10];
    static public int killConsLight = 5;
    /// <summary>0与ダメ,1与回復,2FFダメ,3敵回復,4通常キル,5連続キル,6自滅,7FF,8エリア滞在,9エリア占拠,10旗,11連続キル阻止 </summary>
    static public float[] scoreMults = new float[12];
    /// <summary>使用チーム</summary>
    static public bool[] teamOns = new bool[]{true,true,true,true,false};
    static public int timeLimit = 0;
    [SerializeField] Toggle[] optionTos;
    [SerializeField] TMP_InputField killConsLightIn;
    [SerializeField] TMP_InputField[] scoreMultsIn;
    [SerializeField] Toggle[] teamOnTos;
    [SerializeField] TMP_InputField timeLimitIn;
    [SerializeField] TMP_Dropdown templateDr;
    static public void StartSets()
    {
        if (start) return;
        start = true;
        options[0] = true;
        options[1] = true;
        options[2] = Net_Connect.InsRunner == null || Net_Connect.InsRunner.GameMode == GameMode.Single;
        options[3] = false;
        options[4] = false;
        for (int i = 0; i < scoreMults.Length; i++) scoreMults[i] = 1f;
    }
    void Update()
    {
        StartSets();
        for (int i = 0; i < optionTos.Length; i++)
        {
            optionTos[i].isOn = options[i];
        }
        if(!killConsLightIn.isFocused)killConsLightIn.text = killConsLight.ToString();
        for(int i = 0; i < scoreMultsIn.Length; i++)
        {
            if(!scoreMultsIn[i].isFocused) scoreMultsIn[i].text = scoreMults[i].ToString("F2");
        }
        for (int i = 0; i < teamOnTos.Length; i++)
        {
            if (teamOnTos[i] != null)  teamOnTos[i].isOn = teamOns[i];
        }
        if (timeLimitIn != null && !timeLimitIn.isFocused) timeLimitIn.text = timeLimit.ToString();
    }
    public void Resets()
    {
        if (!Net_Connect.CanControl(Net_Value.NetValue.Object)) return;
        Net_Log.NetLog.RPC_LogAdd(Net_Value.NetValue.gameTime, (int)Net_Log.LogType.System, "リセットされました");
        if (timeLimit > 0)
        {
            Net_Value.NetValue.timeLimit = true;
            Net_Value.NetValue.gameTime = timeLimit;
        }
        else
        {
            Net_Value.NetValue.timeLimit = false;
            Net_Value.NetValue.gameTime = 0;
        } 
        for (int i = 0; i < Net_Value.NetValue.teamKills.Length; i++)
            Net_Value.NetValue.teamKills.Set(i, 0);
        for (int i = 0; i < Net_Value.NetValue.teamScores.Length; i++)
            Net_Value.NetValue.teamScores.Set(i, 0);
        var bhits = Obj_LocalObjects.Bullets;
        for (int i = 0; i < bhits.Count; i++)
        {
            if (bhits[i] == null) continue;
            Destroy(bhits[i].gameObject);
        }
        var ggs = Obj_LocalObjects.Gadgets;
        for (int i = 0; i < ggs.Count; i++)
        {
            if (ggs[i] == null) continue;
            Destroy(ggs[i].gameObject);
        }
        var ults = Obj_LocalObjects.Ults;
        for (int i = 0; i < ults.Count; i++)
        {
            if (ults[i] == null) continue;
            Destroy(ults[i].gameObject);
        }
        for (int i = 0; i < Obj_LocalObjects.Players.Count; i++)
        {
            var pm = Obj_LocalObjects.Players[i];
            if (pm == null) continue;
            pm.RPC_Resets();
        }
        var walls = Obj_LocalObjects.Walls;
        for (int i = 0; i < walls.Count; i++)
        {
            if (walls[i] == null) continue;
            walls[i].Resets();
        }
        var drags = Obj_LocalObjects.Draggers;
        for(int i = 0; i < drags.Count; i++)
        {
            if (drags[i] == null) continue;
            drags[i].Resets();
        }
        var areas = Obj_LocalObjects.Areas;
        for(int i=0;i<areas.Count;i++)
        {
            if (areas[i] == null) continue;
            areas[i].Resets();
        }
        var flags = Obj_LocalObjects.Flags;
        for (int i = 0; i < flags.Count; i++)
        {
            if (flags[i] == null) continue;
            flags[i].Resets(false);
        }
    }
    public void RandomTeamSet()
    {
        List<Player_Manager> players = new();
        for(int i = 0; i < Obj_LocalObjects.Players.Count; i++)
        {
            var pl = Obj_LocalObjects.Players[i];
            if (pl == null) continue;
            if (pl.Object.InputAuthority == PlayerRef.None) continue;
            players.Add(pl);
        }
        int team = 0;
        int count = players.Count;
        var teams = new List<int>();
        for (int i = 0; i < teamOns.Length; i++)
        {
            if (teamOns[i]) teams.Add(i);
        }
        if (teams.Count <= 0) teams.Add(0);
        for (int i = 0; i < count; i++)
        {
            var id = Random.Range(0, players.Count);
            var pl = players[id];
            pl.RPC_TeamSet(teams[team]);
            team = (int)Mathf.Repeat(team + 1, teams.Count);
            players.RemoveAt(id);
        }
        Resets();
    }
    public void TeamOnSet(int id)
    {
        teamOns[id] = teamOnTos[id].isOn;
    }
    public void TimeLimitSet()
    {
        if(int.TryParse(timeLimitIn.text, out var t)) timeLimit = t;
    }
    public void OptionToSet(int id)
    {
        options[id] = optionTos[id].isOn;
    }
    public void ScoreInSet(int id)
    {
        if(float.TryParse(scoreMultsIn[id].text, out var val)) scoreMults[id] = val;
    }
    public void KillConsLightInSet()
    {
        if (int.TryParse(killConsLightIn.text, out var val)) killConsLight = val;
    }
    public void TemplateSet()
    {
        for (int i = 0; i < scoreMults.Length; i++) scoreMults[i] = 1f;
        killConsLight = 5;
        switch (templateDr.value)
        {
            default:
                break;
            case 1:
                options[3] = false;
                options[4] = false;
                killConsLight = 3;
                scoreMults[5] = 4f;
                scoreMults[11] = 2f;
                break;
            case 2:
                options[3] = true;
                options[4] = false;
                scoreMults[4] = 0.5f;
                break;
            case 3:
                options[4] = true;
                options[3] = false;
                scoreMults[4] = 0.5f;
                break;
            case 4:
                scoreMults[2] = -1f;
                scoreMults[3] = -1f;
                scoreMults[7] = -1f;
                break;
        }
    }

}
