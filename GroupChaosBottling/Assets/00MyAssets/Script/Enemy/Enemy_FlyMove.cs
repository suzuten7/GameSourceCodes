using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using static DataBase;
using static Statics;
public class Enemy_FlyMove : MonoBehaviourPun
{
    [SerializeField] State_Base Sta;
    [SerializeField] float HightRem;
    [SerializeField] float SpeedRem;
    [SerializeField,Tooltip("地面高度x～yの間")] Vector2 Hights;
    [SerializeField, Tooltip("垂直移動速度\nx=上昇,y=下降,z=適正速度")] Vector3 HightSpeed;
    [SerializeField, Tooltip("垂直適正距離移動変化%")] float HightChangePer;

    [SerializeField, Tooltip("水平距離x～yの間")] Vector2 NearDis;
    [SerializeField, Tooltip("前後移動速度\nx=前進,y=後退")] Vector2 MoveSpeed;
    [SerializeField, Tooltip("振り向き%")] float LerpSpeed;
    [SerializeField, Tooltip("横移動速度\nx=適正距離,y=外速度")] Vector2 SideSpeed;
    [SerializeField, Tooltip("横移動変化%")] float SideChangePer;
    bool HightU;
    bool SideL;
    private void Start()
    {
        HightU= Random.value >= 0.5f;
        SideL = Random.value >= 0.5f;
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        var RigVect = Sta.Rig.linearVelocity;
        var HRem = 1f - HightRem * 0.01f;
        var Rem = 1f - SpeedRem * 0.01f;
        RigVect.y *= HRem;
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
                var Dis = HoriDistance(Sta.Target.PosGet(),Sta.PosGet());
                float SpeedRem = 1f - Sta.SpeedRem * 0.01f;

                float Hight = Hights.y;
                foreach (var Hit in Physics.RaycastAll(Sta.PosGet(), Vector3.down, Hights.y, DB.CamLayer))
                {
                    if (Hight > Hit.distance) Hight = Hit.distance;
                }
                if (Random.value <= HightChangePer * 0.01f) HightU = Random.value >= 0.5f;
                if (Hight <= Hights.x) RigVect.y += HightSpeed.x * 0.01f * SpeedRem;
                else if (Hight >= Hights.y) RigVect.y -= HightSpeed.y * 0.01f * SpeedRem;
                else
                {
                    RigVect.y += (HightU ? 1 : -1) * HightSpeed.z * 0.01f * SpeedRem;
                }

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
