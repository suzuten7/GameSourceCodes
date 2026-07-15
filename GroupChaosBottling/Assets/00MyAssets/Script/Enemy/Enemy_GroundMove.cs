using Photon.Pun;
using UnityEngine;
using static Statics;
public class Enemy_GroundMove : MonoBehaviourPun
{
    [SerializeField] State_Base Sta;
    [SerializeField] float SpeedRem;
    [SerializeField,Tooltip("距離x～yの間")] Vector2 NearDis;
    [SerializeField, Tooltip("前後移動速度\nx=前進,y=後退")] Vector2 MoveSpeed;
    [SerializeField, Tooltip("振り向き%")] float LerpSpeed;
    [SerializeField,Tooltip("横移動速度\nx=適正距離,y=外速度")] Vector2 SideSpeed;
    [SerializeField, Tooltip("横移動変化%")] float SideChangePer;
    bool SideL;
    private void Start()
    {
        SideL = Random.value >= 0.5f;
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        var RigVect = Sta.Rig.linearVelocity;
        var Rem = 1f - SpeedRem * 0.01f;
        RigVect.x *= Rem;
        RigVect.z *= Rem;
        Sta.Anim_MoveID = 0;
        bool MoveCheck = Sta.HP > 0;
        if (Sta.BreakT > 0) MoveCheck = false;
        if (MoveCheck)
        {
            Sta.TargetSet();
            if (Sta.Target != null)
            {
                Sta.Anim_MoveID = 1;
                var MoveVect = Sta.Target.PosGet() - Sta.PosGet();
                MoveVect.y = 0;
                var Dis = HoriDistance(Sta.PosGet(), Sta.Target.PosGet());
                float SpeedRem = 1f - Sta.SpeedRem * 0.01f;
                if (Dis > NearDis.y) RigVect += MoveVect.normalized * MoveSpeed.x * 0.01f * SpeedRem;
                else if (Dis < NearDis.x) RigVect -= MoveVect.normalized * MoveSpeed.y * 0.01f * SpeedRem;
                if (Random.value <= SideChangePer * 0.01f) SideL = Random.value >= 0.5f;
                if (NearDis.x >= Dis && Dis <= NearDis.y)
                {
                    RigVect += Quaternion.Euler(0, SideL ? 90 : -90, 0) * MoveVect.normalized * SideSpeed.x * 0.01f * SpeedRem;
                }
                else
                {
                    RigVect += Quaternion.Euler(0, SideL ? 90 : -90, 0) * MoveVect.normalized * SideSpeed.y * 0.01f * SpeedRem;
                }
                var LookVect = Sta.Rig.transform.forward;
                LookVect = Vector3.Slerp(LookVect.normalized, MoveVect.normalized, LerpSpeed * 0.01f);
                LookVect.y = 0;
                Sta.Rig.transform.LookAt(Sta.Rig.transform.position + LookVect);
            }
        }

        Sta.Rig.linearVelocity = RigVect;
    }
    private void OnCollisionStay(Collision collision)
    {
        Sta.GroundB = true;
    }
}
