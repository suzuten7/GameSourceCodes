using Photon.Pun;
using UnityEngine;
using static PlayerValue;
using static DataBase;
using static Manifesto;
using static Statics;
public class Player_Move : MonoBehaviourPun
{
    public State_Base Sta;
    [SerializeField] Player_Cont PCont;

    [SerializeField] Camera Cam;
    [SerializeField] float SpeedRem;
    [SerializeField] float MoveSpeed;
    [SerializeField] float LerpSpeed;
    [SerializeField] int DashTime;
    [SerializeField] float DashSpeed;
    [SerializeField] float DashMPCost;

    [SerializeField] float JumpPow;
    [SerializeField] float AirJumpMPCost;

  
    Vector3 DashVect;
    private void Start()
    {
        if (!photonView.IsMine) return;
        float SpeedAdd = 1f + Sta.PVal_PassiveLVGet(Enum_Passive.速度増加) * 0.1f;
        if (Sta.PVal_GeneSetCo(Enum_GeneTypes.速度) >= 2) SpeedAdd += 0.15f;
        MoveSpeed *= SpeedAdd;
        DashSpeed *= SpeedAdd;
    }
    private void Update()
    {
        if (!photonView.IsMine) return;
        var RigVect = Sta.Rig.linearVelocity;
        #region ジャンプ
        var JPowd = JumpPow;
        if (Sta.PVal_GeneSetCo(Enum_GeneTypes.落下) >= 4) JPowd *= 1.5f;
        if (!Sta.NoJump && PCont.Jump_Enter && Sta.HP > 0 && RigVect.y <= JPowd * 0.005f)
        {
            if (Sta.Ground)RigVect.y = JPowd * 0.02f;
            else if(!Sta.LowMP)
            {
                Sta.MP -= AirJumpMPCost;
                Sta.MPUseTime = 0;
                RigVect.y = JPowd * 0.015f;
            }
        }
        #endregion
        #region ダッシュ
        var MoveInput = PCont.Move;
        var MoveVect = Cam.transform.forward * MoveInput.y + Cam.transform.right * MoveInput.x;
        MoveVect.y = 0;

        if (!Sta.NoDash && Sta.DashTime <= 0 && Sta.HP > 0 && !Sta.LowMP && PCont.Dash_Enter)
        {
            Sta.AtkD = null;
            Sta.MP -= DashMPCost;
            Sta.MPUseTime = 0;
            Sta.DashTime = DashTime;
            DashVect = MoveVect.magnitude >= 0.1f ? MoveVect : Sta.Rig.transform.forward;
        }
        #endregion
        Sta.Rig.linearVelocity = RigVect;
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;


        var RigVect = Sta.Rig.linearVelocity;
        Sta.Anim_MoveID = 0;
        #region 減速
        float Rem = 1f - SpeedRem * 0.01f;
        RigVect.x *= Rem;
        RigVect.z *= Rem;
        #endregion
        #region 移動
        if (Sta.DashTime <= 0)
        {
            var MoveInput = PCont.Move;
            var MoveVect = Cam.transform.forward * MoveInput.y + Cam.transform.right * MoveInput.x;
            MoveVect.y = 0;
            float SpeedMlt = 1f - (Sta.SpeedRem * 0.01f);
            if (Sta.HP <= 0) SpeedMlt = 0.3f;
            if (MoveVect.magnitude > 0.1f)
            {
                Sta.Anim_MoveID = 1;
                RigVect += MoveVect.normalized * MoveSpeed * 0.01f * SpeedMlt;
                var LookVect = Sta.Rig.transform.forward;
                LookVect = Vector3.Slerp(LookVect.normalized, MoveVect.normalized, LerpSpeed * 0.01f);
                LookVect.y = 0;
                Sta.Rig.transform.LookAt(Sta.Rig.transform.position + LookVect);
            }
        }
        else
        {
            RigVect = DashVect.normalized * DashSpeed * 0.01f;
            Sta.Rig.transform.LookAt(Sta.Rig.transform.position + DashVect);
            Sta.Anim_MoveID = 2;
        }
        #endregion
        #region 照準
        if (Sta.HP > 0)
        {
            bool AimMode = Sta.AtkD != null && Sta.Aiming;
            if(Sta.PLValues.SubID <= 0 && PSaves.CamDistance < 1f)AimMode = true;
            if (AimMode)
            {
                var LookRot = Cam.transform.eulerAngles;
                LookRot.x = 0;
                Sta.Rig.transform.eulerAngles = LookRot;
            }
            else
            {
                if (Sta.TargetHit != null)
                {
                    var TargetVect = Sta.TargetHit.PosGet() - Sta.Rig.transform.position;
                    TargetVect.y = 0;
                    Sta.Rig.transform.LookAt(Sta.Rig.transform.position + TargetVect);
                }
                else if (Sta.Target != null)
                {
                    var TargetVect = Sta.Target.PosGet() - Sta.Rig.transform.position;
                    TargetVect.y = 0;
                    Sta.Rig.transform.LookAt(Sta.Rig.transform.position + TargetVect);
                }
            }
        }
        #endregion

        Sta.Rig.linearVelocity = RigVect;
    }
    private void OnCollisionStay(Collision collision)
    {
        Sta.GroundB = true;
    }
}
