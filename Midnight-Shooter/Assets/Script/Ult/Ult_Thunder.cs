using System.Collections.Generic;
using UnityEngine;

public class Ult_Thunder : Ult_Base
{
    [SerializeField, Tooltip("バフ付与")] BufAdd[] bufAdds;
    [SerializeField] float wait;
    [SerializeField] float damage;
    [SerializeField] float injustPer;
    [SerializeField] float damageScoreMult;
    [SerializeField] float killScoreMult;
    bool used = false;
    public override string InfoGet()
    {
        var istr = $"{LocalizSystem.LocailzSCInfo("ダメージ")}{damage}";
        istr += $"\n{LocalizSystem.LocailzSCInfo("負傷割合")}{injustPer}%";
        if (damageScoreMult != 1) istr += $"\n{LocalizSystem.LocailzSCInfo("ダメージスコア")}{damageScoreMult}{LocalizSystem.LocailzSCInfo("倍")}";
        if (killScoreMult != 1) istr += $"\n{LocalizSystem.LocailzSCInfo("キルスコア")}{killScoreMult}{LocalizSystem.LocailzSCInfo("倍")}";
        istr += "\n" + Data_Base.BufInfoStr(bufAdds);
        return istr;
    }
    public override void Update()
    {
        base.Update();
        if (!Net_Connect.CanControl(Object) || pm == null) return;

        if (!used && time >= wait)
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
                var target = targets[Random.Range(0, targets.Count)];
                target.Damage(damage, injustPer, false, pm, ultid + (int)AtkID.Ult, damageScoreMult, killScoreMult);
                target.BufChanges(bufAdds);
                transform.rotation = Quaternion.identity;
                transform.position = target.PosGet;
            }
        }
    }
}
