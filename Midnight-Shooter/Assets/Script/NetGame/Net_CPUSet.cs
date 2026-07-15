using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Net_CPUSet : MonoBehaviour
{
    [SerializeField] Player_Manager Pl;
    [SerializeField] List<Player_Manager> CPU_Pls;
    private void Update()
    {
        if (!Net_Connect.InsRunner.IsServer) return;
        CPU_Pls.RemoveAll(x => x == null);
        for (int i = 0; i < Mathf.Max(CPU_Pls.Count, UI_CPU_Base.CPUs.Count); i++)
        {
            if(i >= UI_CPU_Base.CPUs.Count)
            {
                if (CPU_Pls[i]!=null)Destroy(CPU_Pls[i].gameObject);
                continue;
            }
            if(i >= CPU_Pls.Count)
            {
                var cpls = Net_Connect.InsRunner.Spawn(Pl,Vector3.zero,Quaternion.identity,PlayerRef.None);
                CPU_Pls.Add(cpls);
            }
            var cpu = UI_CPU_Base.CPUs[i];
            var cpl = CPU_Pls[i];
            var sets = Player_Sets.Sets[cpu.setID];
            cpl.states.name = "CPU" + (i + 1);
            if (sets.name != "") cpl.states.name += ":" + sets.name;
            if (cpl.states.teamID != cpu.team)
            {
                cpl.states.teamID = cpu.team;
                cpl.Respawne();
            }
            cpl.states.cpuMode = cpu.mode;

            cpl.states.gun_IndexNum = sets.gunID;
            cpl.states.melee_IndexNum = sets.meleeID;
            cpl.states.gadget_IndexNum = sets.gadgetID;
            cpl.states.ult_IndexNum = sets.ultID;
            cpl.states.passives = sets.passives;
            cpl.states.charaImgID = sets.charaImgID;
            cpl.states.loadImgID = sets.loadImgID;
        }
    }
}
