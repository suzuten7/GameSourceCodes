

namespace FNet
{
    using Player;
    using UnityEngine;
    using static GmSystem.GS_ChangeSet;
    using static GmSystem.GS_SaveValues;
    using static FNet.Fusion_Manager;
    public class Fusion_ServerMove : MonoBehaviour
    {
        [SerializeField, Tooltip("キャラ物理オブジェクト")] Rigidbody Rig;
        [SerializeField, Tooltip("カメラトランスフォーム")] Transform CamTrans;
        [SerializeField, Tooltip("水平速度:x=通常,y=ダッシュ")] Vector2 MoveSpeed;
        [SerializeField, Tooltip("垂直速度:x=通常,y=ダッシュ")] Vector2 UpSpeed;

        [SerializeField, Tooltip("減速%/f")] float RemSpeedPer;



        static public bool DispMode = false;
        void FixedUpdate()
        {
            ChangeActive(CamTrans.gameObject,InsRunner == null || DispMode);
            if (InsRunner != null && !DispMode)return;
            var plcont = Player_Controle.PCont;
            var rigVect = Rig.linearVelocity;
            rigVect.x *= 1f - RemSpeedPer * 0.01f;
            rigVect.y *= 1f - RemSpeedPer * 0.01f;
            rigVect.z *= 1f - RemSpeedPer * 0.01f;
            var moveVect = CamTrans.forward * plcont.V2_Move.y + CamTrans.right * plcont.V2_Move.x;
            moveVect.y = 0;
            if (moveVect.magnitude >= 0.1f)
            {
                rigVect += moveVect.normalized * (!plcont.Stay_Dash ? MoveSpeed.x : MoveSpeed.y) * 0.01f;
            }
            if (plcont.Stay_Jump) rigVect.y += (!plcont.Stay_Dash ? UpSpeed.x : UpSpeed.y) * 0.01f;
            if (plcont.Stay_Fall) rigVect.y -= (!plcont.Stay_Dash ? UpSpeed.x : UpSpeed.y) * 0.01f;
            Rig.linearVelocity = rigVect;

        }
    }
}
