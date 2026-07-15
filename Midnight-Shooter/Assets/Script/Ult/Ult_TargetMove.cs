using Fusion;
using UnityEngine;

public class Ult_TargetMove : NetworkBehaviour
{
    [SerializeField] Ult_Base ultb;
    [SerializeField] Rigidbody2D rig;
    [SerializeField] float Speed;
    [SerializeField] float changeCT;
    [SerializeField] float targetRange;

    [SerializeField] bool selfTarget;
    [SerializeField] bool teamTarget;
    [SerializeField] bool enemyTarget;
    [SerializeField] bool backNoTarget;
    [SerializeField] bool noDeathRem;
    float ct;
    Player_Manager target;
    void Update()
    {
        if (!Net_Connect.CanControl(Object)) return;
        if (Obj_LocalObjects.TimeStopd) return;
        ct -= Time.deltaTime;
        if (target != null && !noDeathRem && target.hpTotal <= 0) target = null;
        if (ct <= 0)
        {
            Player_Manager ctarget = null;
            var ndis = targetRange > 0 ? targetRange : float.MaxValue;
            for (int i = 0; i < Obj_LocalObjects.Players.Count; i++)
            {
                var pms = Obj_LocalObjects.Players[i];
                if (pms == null) continue;
                if (!noDeathRem && pms.hpTotal <= 0) continue;
                if (backNoTarget && target == pms) continue;
                var check = false;
                if (selfTarget && pms == ultb.pm) check = true;
                if (teamTarget && pms != ultb.pm && pms.states.teamID == ultb.pm.states.teamID) check = true;
                if (enemyTarget && pms.states.teamID != ultb.pm.states.teamID) check = true;
                if (!check) continue;
                var dis = Vector2.Distance(transform.position, pms.PosGet);
                if (ndis > dis)
                {
                    ctarget = pms;
                    ndis = dis;
                }
            }
            if (ctarget != null)
            {
                target = ctarget;
                ct = changeCT;
            }
        }
        if(target != null)
        {
            var vect = target.PosGet - transform.position;
            rig.AddForce(vect.normalized * Speed);
        }
    }
}
