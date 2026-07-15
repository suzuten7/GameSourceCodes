
namespace Player
{
    using UIs;
    using UnityEngine;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static FNet.Fusion_Manager;
    public class Player_Camera : MonoBehaviour
    {
        [SerializeField] Transform CharaTrans;
        [SerializeField] Transform RotTrans;
        [SerializeField] Transform CamTrans;
        [SerializeField] Camera LongCam;

        [SerializeField] float Hight;
        [SerializeField] Vector2 LookSpeed;
        [SerializeField] Vector2 XFix;
        [SerializeField] float LockSpeed;

        void LateUpdate()
        {
            var mpl = MyPlayer;
            var plcont = Player_Controle.PCont;
            var longUse = GetSave_Option.LongCamUse;
            if(LongCam.enabled != (InsRunner == null || longUse)) LongCam.enabled = (InsRunner == null || longUse);
            RotTrans.position = CharaTrans.position + Vector3.up * (Hight + GetSave_Option.Cam_Pos.y*0.01f);
            var rot = RotTrans.eulerAngles;
            var SMult = GetSave_Option.Cam_MvSpeed * 0.01f;
            if (UI_System.ui_system != null && UI_System.LookStop) SMult = 0f;
            rot.x += plcont.V2_Look.y * LookSpeed.x * 0.01f * SMult;
            rot.y += plcont.V2_Look.x * LookSpeed.y * 0.01f * SMult;

            var locks = plcont.Tr_LockOn && mpl != null && mpl.TargetStaGet != null;
            var lookstop = plcont.Stay_LockOn || plcont.V2_Look.magnitude > 0.1f;
            var rlspeed = LockSpeed * 0.01f * GetSave_Option.Cam_TgSpeed * 0.01f;

            if (locks && !lookstop)
            {
                var trotv = mpl.TargetStaGet.PosGet - mpl.PosGet;
                var lrotv = Vector3.Slerp(RotTrans.forward, trotv, rlspeed);
                rot.y = Quaternion.FromToRotation(Vector3.forward, lrotv).eulerAngles.y;
            }

            rot.x = Mathf.Clamp(Mathf.Repeat(rot.x + 180f, 360f) - 180f, XFix.x, XFix.y);
            RotTrans.eulerAngles = rot;


            var lpos = Vector3.zero;
            var dis = GetSave_Option.Cam_Pos.z * 0.01f;
            lpos.z = -dis;
            lpos.x = -GetSave_Option.Cam_Pos.x * 0.01f * dis;
            CamTrans.localPosition = lpos;

            var camrot = CamTrans.eulerAngles;
            camrot.x = RotTrans.eulerAngles.x;
            camrot.z = RotTrans.eulerAngles.z;
            if (locks)
            {
                if (!lookstop)
                {
                    var ctrotv = mpl.TargetStaGet.PosGet - CamTrans.position;
                    ctrotv.y = 0;
                    var cltrotv = Vector3.Slerp(CamTrans.forward, ctrotv, rlspeed);
                    camrot.y = Quaternion.FromToRotation(Vector3.forward, cltrotv).eulerAngles.y;
                }
            }
            else
            {
                var ctrotv = RotTrans.forward;
                ctrotv.y = 0;
                var cltrotv = Vector3.Slerp(CamTrans.forward, ctrotv, 0.1f);
                camrot.y = Quaternion.FromToRotation(Vector3.forward, cltrotv).eulerAngles.y;
            }
            CamTrans.eulerAngles = camrot;

        }
    }
}
