using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_CPU_Base : MonoBehaviour
{
    public class CPUSets
    {
        public int team;
        public int mode;
        public int setID;
    }
    static public List<CPUSets> CPUs = new();
    static public bool[] cpuTeamOns = new bool[] { true, true, true, true, false };
    [SerializeField] Data_Base db;
    [SerializeField] TextMeshProUGUI setNametx;
    [SerializeField] List<UI_Sets_Set> setUIs;
    [SerializeField] List<UI_CPU_Add> addUIs;

    [SerializeField] TMP_Dropdown addBaseSetTeamDr;
    [SerializeField] TMP_Dropdown addBaseSetModeDr;
    [SerializeField] Toggle[] teamOnTos;
    [SerializeField] TextMeshProUGUI addBaseSetTx;
    int setid = 0;

    int addsetid = 0;
    private void Update()
    {
        setNametx.text = $"{LocalizSystem.LocailzSCInfo("セット")}{(setid+1)}:{Player_Sets.Sets[setid].name}";
        for (int i = 0; i < Mathf.Max(setUIs.Count, Player_Sets.Sets.Count); i++)
        {
            if (i >= Player_Sets.Sets.Count)
            {
                setUIs[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= setUIs.Count)
            {
                setUIs.Add(Instantiate(setUIs[0], setUIs[0].transform.parent));
            }
            var sui = setUIs[i];
            sui.gameObject.SetActive(true);
            sui.UISet(i, setid);
        }
        for (int i = 0; i < Mathf.Max(CPUs.Count, addUIs.Count); i++)
        {
            if(i >= CPUs.Count)
            {
                addUIs[i].gameObject.SetActive(false);
                continue;
            }
            if(i >= addUIs.Count)
            {
                addUIs.Add(Instantiate(addUIs[0], addUIs[0].transform.parent));
            }
            var cpu = CPUs[i];
            var ui = addUIs[i];
            ui.gameObject.SetActive(true);
            ui.ID = i;
            ui.IDtx.text = "No" + (i + 1).ToString();
            ui.TeamDr.value = cpu.team;
            ui.ModeDr.value = cpu.mode;
            ui.SetTx.text = $"{LocalizSystem.LocailzSCInfo("セット")}{(cpu.setID + 1)}:{Player_Sets.Sets[cpu.setID].name}";
        }

        addBaseSetTx.text = $"{LocalizSystem.LocailzSCInfo("セット")}{(addsetid + 1)}:{Player_Sets.Sets[addsetid].name}";

        for(int i = 0; i < teamOnTos.Length; i++)
        {
            teamOnTos[i].isOn = cpuTeamOns[i];
        }
    }
    public void SetChange(int id)
    {
        setid = id;
    }
    public void Add()
    {
        int _team;
        if (addBaseSetTeamDr.value == 0)
        {
            var teamCount = new int[]{0,0,0,0,0};
            if (Obj_LocalObjects.LocalObjects != null)
            {
                for (int i = 0; i < Obj_LocalObjects.Players.Count; i++)
                {
                    var pm = Obj_LocalObjects.Players[i];
                    if (pm == null) continue;
                    teamCount[pm.states.teamID]++;
                }
            }
            else
            {
                for (int i = 0; i < CPUs.Count; i++)
                {
                    teamCount[CPUs[i].team]++;
                }
            }
            int mt = 0;
            int mc = int.MaxValue;
            for(int i = 0; i < teamCount.Length; i++)
            {
                if (!cpuTeamOns[i]) continue;
                var tc = teamCount[i];
                if (mc > tc)
                {
                    mc = tc;
                    mt = i;
                }
            }
            _team = mt;
        }
        else
        {
            _team = addBaseSetTeamDr.value - 1;
        }
        CPUs.Add
        (
            new()
            {
                setID = addsetid,
                team = _team,
                mode = addBaseSetModeDr.value,
            }
        );
    }
    public void Rem(int i)
    {
        CPUs.RemoveAt(i);
    }
    public void RemAll()
    {
        CPUs.Clear();
    }
    public void Set(int m,int i)
    {
        var cpu = CPUs[i];
        var ui = addUIs[i];
        switch (m)
        {
            case 0:cpu.team = ui.TeamDr.value; break;
            case 1:cpu.mode = ui.ModeDr.value;break;
            case 2:cpu.setID = setid; break;
        }
    }
    public void BaseSet()
    {
        addsetid = setid;
    }
    public void TeamRandom()
    {
        var cpus = CPUs.ToList();
        var count = cpus.Count;
        var team = 0;
        var teams = new List<int>();
        for (int i = 0; i < UI_GameRuleSetting.teamOns.Length; i++)
        {
            if (cpuTeamOns[i])teams.Add(i);
        }
        if (teams.Count <= 0) teams.Add(0);
        for (int i = 0; i < count; i++)
        {
            var id = Random.Range(0, cpus.Count);
            cpus[id].team = teams[team];

            team = (int)Mathf.Repeat(team + 1, teams.Count);
            cpus.RemoveAt(id);
        }
    }

    public void TeamOnSet(int id)
    {
        cpuTeamOns[id] = teamOnTos[id].isOn;
    }
}
