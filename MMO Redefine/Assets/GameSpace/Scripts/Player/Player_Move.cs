
namespace Player
{
    using Fusion;
    using UnityEngine;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalState;
    using static UIs.UI_System;
    public class Player_Move : NetworkBehaviour
    {
        [SerializeField, Tooltip("ステータス")] Player_State Sta;
        [SerializeField, Tooltip("カメラトランスフォーム")] Transform CamTrans;
        [SerializeField] ParticleSystem FlyEffect;
        [SerializeField, Tooltip("水平減速%/f")] float RemSpeedPer;
        [SerializeField, Tooltip("通常速度:x=通常,y=ダッシュ")] Vector2 GroundMoveSpeed;
        [SerializeField, Tooltip("通常ダッシュ持続消費移動力/秒")] float GroundStayDashUseST;
        [SerializeField, Tooltip("ダッシュ開始速度")] float DashInSpeed;
        [SerializeField, Tooltip("ダッシュ開始消費移動力/秒")] float InDashUseST;
        [SerializeField, Tooltip("ジャンプ速度")] float JumpSpeed;
        [SerializeField, Tooltip("ジャンプ時間(秒)")] float JumpTime;
        [SerializeField, Tooltip("ジャンプ空中消費移動力")] float JumpAirUseST;
        [SerializeField, Tooltip("飛行水平速度:x=通常,y=ダッシュ")] Vector2 FlyMoveSpeed;
        [SerializeField, Tooltip("飛行垂直速度:x=通常,y=ダッシュ")] Vector2 FlyUpSpeed;
        [SerializeField, Tooltip("飛行消費移動力/秒")] float FlyUseST;
        [SerializeField, Tooltip("飛行ダッシュ持続消費移動力/秒:x=水平,y=垂直")] Vector2 FlyStayDashUseST;
        [SerializeField, Tooltip("垂直減速%/f")] float RemUpSpeedPer;
        [SerializeField, Tooltip("旋回速度%")] float RotPer;
        [Header("変数")]
        [SerializeField] int JumpT = 0;
        [SerializeField] bool Fly;
        [Networked] bool Net_Fly { get; set; }
        [SerializeField] int FlyInTime = 0;
        bool _bground;
        void FixedUpdate()
        {

            if (!Fly) FlyEffect.Stop();
            else if(!FlyEffect.isPlaying) FlyEffect.Play();
            if (!CanControl(Object))
            {
                Fly = Net_Fly;
                return;
            }

            Sta.AnimValues.MoveID = 0;
            Sta.ChangeValues.Ground = _bground;
            _bground = false;
            JumpT--;
            if (Sta.HP <= 0)Fly = false;
            bool gravty = !Fly;
            if (JumpT > 0) gravty = false;
            Sta.SettingValues.Rig.useGravity = gravty;


            var rigVect = Sta.SettingValues.Rig.linearVelocity;
            rigVect.x *= 1f - RemSpeedPer * 0.01f;
            rigVect.z *= 1f - RemSpeedPer * 0.01f;

            var plcont = !PhotoMode ? Sta.Cont : Player_Controle.PCont;
            var moveVect = Vector3.zero;
            var mspeed = !Fly ? GroundMoveSpeed : FlyMoveSpeed;
            if (Sta.HP > 0 && plcont != null)
            {
                bool dashs = plcont.Stay_Dash && !Sta.ChangeValues.LowST;
                var ctrans = (PhotoMode && Player_PhotoCam.PhotoCamTrans!= null) ? Player_PhotoCam.PhotoCamTrans : CamTrans;
                moveVect = ctrans.forward * plcont.V2_Move.y + ctrans.right * plcont.V2_Move.x;
                moveVect.y = 0;
                if (!photos)
                {

                    if (moveVect.magnitude >= 0.1f)
                    {
                        Sta.AnimValues.MoveID = 1;
                        if (dashs)
                        {
                            Sta.AnimValues.MoveID = 2;
                            Sta.STUse((!Fly ? GroundStayDashUseST : FlyStayDashUseST.x) / 60f);
                        }
                        rigVect += moveVect.normalized * (!dashs ? mspeed.x : mspeed.y) * 0.01f * SpeedMlt;
                        if (plcont.In_Dash && !Sta.ChangeValues.LowST)
                        {
                            Sta.STUse(InDashUseST);
                            rigVect += moveVect.normalized * DashInSpeed * 0.01f * SpeedMlt;
                        }
                    }
                    plcont.In_Dash = false;
                    if (!Fly && !Sta.ChangeValues.Water && (Sta.ChangeValues.Ground || !Sta.ChangeValues.LowST) && plcont.In_Jump && JumpT <= 0)
                    {
                        if (!Sta.ChangeValues.Ground) Sta.STUse(JumpAirUseST);
                        JumpT = Mathf.RoundToInt(JumpTime * 60);
                        rigVect = Vector3.up * JumpSpeed * 0.01f;
                    }
                    plcont.In_Jump = false;
                } 

                if (!Sta.ChangeValues.Water)
                {
                    if (!Sta.ChangeValues.Ground && plcont.Stay_Jump &&!photos) FlyInTime++;
                    else FlyInTime = 0;
                    if (JumpT <= 0 && FlyInTime >= 30 && !Sta.ChangeValues.LowST) Fly = true;
                    if (Fly)
                    {
                        Sta.STUse(FlyUseST / 60f);
                        if (Sta.ST <= 0) Fly = false;
                        var flySpeed = !dashs ? FlyUpSpeed.x : FlyUpSpeed.y;
                        if (plcont.Stay_Jump && !photos)
                        {
                            Sta.STUse(FlyStayDashUseST.y / 60f);
                            rigVect.y = flySpeed * 0.01f;
                        }
                        else if (plcont.Stay_Fall && !photos)
                        {
                            Fly = false;
                        }
                        else rigVect.y *= 1f - RemUpSpeedPer * 0.01f;
                    }
                }
                else
                {
                    FlyInTime = 0;
                    if (plcont.Stay_Jump && !photos) rigVect.y += (!dashs ? mspeed.x : mspeed.y) * 0.01f;
                    if (plcont.Stay_Fall && !photos) rigVect.y -= (!dashs ? mspeed.x : mspeed.y) * 0.01f;
                }
                if(plcont.Tr_LockOn && Sta.TargetStaGet != null)
                {
                    var lockVect = Sta.TargetPosGet - Sta.PosGet;
                    lockVect.y = 0;
                    Sta.SettingValues.Rig.transform.LookAt(Sta.SettingValues.Rig.transform.position + lockVect);
                }
                else if(Sta.ChangeValues.LastLookTime >= 60 && !photos)
                {
                    var rotVect = moveVect;
                    var lerpVect = Vector3.Slerp(Sta.SettingValues.Rig.transform.forward, rotVect, RotPer * 0.01f);
                    lerpVect.y = 0;
                    Sta.SettingValues.Rig.transform.LookAt(Sta.SettingValues.Rig.transform.position + lerpVect);
                }
            }
            Sta.SettingValues.Rig.linearVelocity = rigVect;
            RPC_ServSet(Fly);
            if (PhotoMode)
            {
                var set = PhotoSetGet(Sta);
                switch (set.MotionSet)
                {
                    case 1: Sta.AnimValues.MoveID = 1; break;
                    case 2: Sta.AnimValues.MoveID = 2; break;
                }
            }
        }
        float SpeedMlt
        {
            get
            {
                var bspeed = 1f - Sta.BufPowGet(Enum_Buf.Reiki) * 0.01f;
                bspeed *= Sta.ValGet(Enum_StateAddsType.MoveSpeed) * 0.01f;
                return bspeed;
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            _bground = true;
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_ServSet(bool syFly)
        {
            Net_Fly = syFly;
        }
        bool photos
        {
            get { return PhotoMode &&PhotoContPL !=Sta; }
        }
    }
}
