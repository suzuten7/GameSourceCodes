using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static Suzuten_PlayerSets;
public class Suzuten_TimeAnomaly : MonoBehaviourPun
{
    [SerializeField] Suzuten_ShotObj SObj;
    [SerializeField] int TimeAdd;
    [SerializeField] float HPAdds;
    [SerializeField] int TimeRem;
    [SerializeField] int SpeedAddTime;
    [SerializeField] Vector2 SpeedAddVal;
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        float UseHPPer = SObj.UsePS.HP / Mathf.Max(1, SObj.UsePS.CD.MHP * BOP_HMSP[0] * 0.01f);
        float EnemyHPPer = SObj.UsePS.Target.HP / Mathf.Max(1, SObj.UsePS.Target.CD.MHP * BOP_HMSP[0] * 0.01f);
        if (UseHPPer >= EnemyHPPer)
        {
            TimeChage(-TimeRem);
        }
        else
        {
            SObj.UsePS.HP += HPAdds;
            TimeChage(TimeAdd);
        }
        SpeedAdds(SpeedAddTime, Random.Range(SpeedAddVal.x, SpeedAddVal.y));
        Destroy(this);
    }
}
