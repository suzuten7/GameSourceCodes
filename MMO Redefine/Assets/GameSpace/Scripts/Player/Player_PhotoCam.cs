
namespace Player
{
    using UIs;
    using UnityEngine;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static UIs.UI_System;
    public class Player_PhotoCam : MonoBehaviour
    {
        public static Transform PhotoCamTrans;
        [SerializeField] Camera LongCam;
        [SerializeField] Camera[] UICams;
        [SerializeField] Rigidbody Rig;
        [SerializeField] float RemSpeed;
        [SerializeField] Vector2 MoveSpeed;
        [SerializeField] float DashMult;
        [SerializeField] Vector2 LookSpeed;
        [SerializeField] Vector2 XFix;
        private void Start()
        {
            PhotoCamTrans = transform;
        }
        void LateUpdate()
        {
            var longUse = GetSave_Option.LongCamUse;
            if(LongCam.enabled != longUse) LongCam.enabled = longUse;
            for (int i = 0; i < UICams.Length; i++)
            {
                if (!UI_Photo_Main.StatusHide) UICams[i].cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
                else UICams[i].cullingMask |= (1 << LayerMask.NameToLayer("UI"));
            }

            var plcont = Player_Controle.PCont;
            var rot = transform.eulerAngles;
            var SMult = GetSave_Option.Cam_MvSpeed * 0.01f;
            rot.x += plcont.V2_Look.y * LookSpeed.x * 0.01f * SMult;
            rot.y += plcont.V2_Look.x * LookSpeed.y * 0.01f * SMult;
            rot.x = Mathf.Clamp(Mathf.Repeat(rot.x + 180f, 360f) - 180f, XFix.x, XFix.y);

            transform.eulerAngles = rot;

            }
        private void FixedUpdate()
        {
            var plcont = Player_Controle.PCont;
            var Mult = (plcont.Stay_Dash ? DashMult : 1) *0.01f;
            var vect = Rig.linearVelocity;
            vect *= 1f - RemSpeed * 0.01f;
            if (PhotoContPL == null)
            {
                var mvect = transform.forward * plcont.V2_Move.y + transform.right * plcont.V2_Move.x;
                mvect.y = 0;
                if (mvect.magnitude >= 0.1f) vect += mvect.normalized * MoveSpeed.x * Mult;

                if (plcont.Stay_Jump) vect += Vector3.up * MoveSpeed.y * Mult;
                if (plcont.Stay_Fall) vect -= Vector3.up * MoveSpeed.y * Mult;
                plcont.In_Jump = false;
                plcont.In_Dash = false;
                plcont.In_AtkFBC = false;
                plcont.In_Atk1 = false;
                plcont.In_Atk2 = false;
            }

            Rig.linearVelocity = vect;
        }
    }
    
}
