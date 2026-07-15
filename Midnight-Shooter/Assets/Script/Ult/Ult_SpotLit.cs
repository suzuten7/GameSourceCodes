using System.Collections.Generic;
using UnityEngine;

public class Ult_SpotLit : Ult_Base
{
    [SerializeField, Tooltip("バフ付与")] BufAdd[] bufAdds;
    bool used = false;
    public override string InfoGet()
    {
        return Data_Base.BufInfoStr(bufAdds);
    }
    public override void Update()
    {
        base.Update();
        if (!Net_Connect.CanControl(Object) || pm == null) return;

        if (!used)
        {
            used = true;
            List<Player_Manager> targets = new();
            for (int i = 0; i < Obj_LocalObjects.Players.Count; i++)
            {
                var pl = Obj_LocalObjects.Players[i];
                if (pl == null) continue;
                if (pl.states.teamID != pm.states.teamID) targets.Add(pl);
            }
            if (targets.Count > 0)
            {
                targets[Random.Range(0, targets.Count)].BufChanges(bufAdds);
            }
        }
    }
}
