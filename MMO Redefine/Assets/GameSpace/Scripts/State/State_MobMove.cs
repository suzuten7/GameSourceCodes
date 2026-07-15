
namespace State
{
    using Fusion;
    using UnityEngine;
    using static FNet.Fusion_Manager;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_GlobalState;
    public class State_MobMove : NetworkBehaviour
    {
        [SerializeField] State_StateBase Sta;
        [SerializeField] bool NoAttackIDSet;
        [SerializeField, Tooltip("移動速度x=接近,y=後退")] Vector2 MoveSpeed;
        [SerializeField, Tooltip("適正距離x～y")] Vector2 MoveRange;
        [SerializeField] float RemSpeedPer;
        [SerializeField] float RotPer;

        void FixedUpdate()
        {
            if (!CanControl(Object)) return;
            var rigVect = Sta.SettingValues.Rig.linearVelocity;
            rigVect.x *= 1f - RemSpeedPer * 0.01f;
            rigVect.z *= 1f - RemSpeedPer * 0.01f;
            if (WaterMode)
            {
                rigVect.y *= 1f - RemSpeedPer * 0.01f;
                Sta.SettingValues.Rig.useGravity = false;
            }
            else Sta.SettingValues.Rig.useGravity = true;
            Sta.AnimValues.MoveID = 0;
            if (Sta.HP > 0)
            {
                State_StateBase target = null;
                float ndis = float.MaxValue;
                foreach (var sta in StateList)
                {
                    if (sta == null) continue;
                    if (Sta.TeamCheck(sta.CommonValues.Team) != Enum_TeamCheck.Enemy) continue;
                    if (sta.HP <= 0) continue;
                    var dis = Vector3.Distance(Sta.PosGet, sta.PosGet);
                    if (ndis > dis)
                    {
                        ndis = dis;
                        target = sta;
                    }
                }
                if (target != null)
                {
                    Sta.AnimValues.MoveID = 2;
                    bool input = FixServerTime() % 5 == 0;
                    var moveVect = target.PosGet - Sta.PosGet;
                    if (!WaterMode) moveVect.y = 0;
                    if (ndis > MoveRange.y) rigVect += moveVect.normalized * MoveSpeed.x * 0.01f * SpeedMlt;
                    else if (ndis < MoveRange.x) rigVect -= moveVect.normalized * MoveSpeed.y * 0.01f * SpeedMlt;

                    var rotVect = (Sta.PosGet + moveVect) - Sta.PosGet;
                    if (!WaterMode) rotVect.y = 0;
                    var lerpVect = Vector3.Slerp(Sta.SettingValues.Rig.transform.forward, rotVect, RotPer * 0.01f);
                    if (!WaterMode) lerpVect.y = 0;
                    Sta.SettingValues.Rig.transform.LookAt(Sta.SettingValues.Rig.transform.position + lerpVect);
                }
            }
            Sta.SettingValues.Rig.linearVelocity = rigVect;


        }
        float SpeedMlt
        {
            get
            {
                var bspeed = 1f - Sta.BufPowGet(Enum_Buf.Reiki) * 0.01f;
                bspeed *= Sta.ValGet(Enum_StateAddsType.MoveSpeed)*0.01f;
                return bspeed;
            }
        }
        bool WaterMode
        {
            get
            {
                return Sta.ChangeValues.Water;
            }
        }
    }
}
