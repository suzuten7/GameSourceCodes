using UnityEngine;
using System.Collections.Generic;
public class Ult_Warp : Ult_Base
{
    [SerializeField] bool modeRandom;
    bool used;
    public override string InfoGet()
    {
        return "";
    }
    public override void Update()
    {
        base.Update();
        if (!Net_Connect.CanControl(Object)) return;
        if (!used)
        {
            used = true;
            if (!modeRandom)
            {
                pm.TransGet.position = pm.objects.targetPoint.position;
            }
            else
            {
                List<Player_Manager> pls = new();
                for(int i = 0; i < Obj_LocalObjects.Players.Count; i++)
                {
                    var pl = Obj_LocalObjects.Players[i];
                    if (pl == null) continue;
                    if (pl == pm) continue;
                    pls.Add(pl);
                }
                if (pls.Count > 0) pm.TransGet.position = pls[Random.Range(0, pls.Count)].PosGet;
            }
        }
    }
}
