using Photon.Pun;
using UnityEngine;
using static GameInfos;
using static GlovalValues;
using static DataBase;
public class Player_Move : MonoBehaviourPun
{
    #region エディタ変数
    public Player_States PSta;
    public Player_ActionBase ACs;
    [SerializeField] Player_Inputd PInd;
    [SerializeField] bool HPChecks;

    [SerializeField] Transform CRotateBase;
    [SerializeField] Transform CamTrans;
    [SerializeField] Camera CameraObj;
    [SerializeField] LayerMask CameraNearLayer;
    [SerializeField] float GroundSpeed=5;
    [SerializeField] float WaterSpeed=15;
    [SerializeField] float DashMultPer=120;
    [SerializeField] float Gost_GroundSpeed = 20;
    [SerializeField] float Gost_WaterSpeed = 30;
    [SerializeField] float Gost_DashMultPer = 300;
    [SerializeField] bool LooksRot;
    #endregion
    #region 内部変数
    Vector3 CamLoacls;
    Vector3 BaseRot;

    #endregion
    void Start()
    {
        BaseRot = CRotateBase.eulerAngles;
        CamLoacls = CamTrans.localPosition;
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        #region 水判定
        PSta.Water = PSta.WaterCheck;
        PSta.WaterCheck = false;
        #endregion
        Vector3 RigVect = PSta.Rig.linearVelocity;
        float SpeedMulPer = 100;
        bool LGrav = false;
        #region パッシブ処理
        switch (PSta.Types)
        {
            case TypesE.逃げ:
                if (PSta.PassL.TryGetValue((int)Fugi_PassE.低重力,out var LGrLV)&&LGrLV>0) LGrav = true;
                switch (PSta.PassL.TryGetValue((int)Fugi_PassE.加速, out var OPassPcSLV) ? OPassPcSLV : 0)
                {
                    default:break;
                    case 1: SpeedMulPer += 3f; break;
                    case 2: SpeedMulPer += 6f; break;
                    case 3: SpeedMulPer += 9f; break;
                    case 4: SpeedMulPer += 15f; break;
                }
                if(PSta.AddCTs.ContainsKey(AddCTsE.パニック)) SpeedMulPer *= 1.5f;
                if (PSta.PassL.TryGetValue((int)Fugi_PassE.Debug_ジェットスピード,out var JSpeedLV)&&JSpeedLV>0) SpeedMulPer *= 10f;
                break;
            case TypesE.鬼操縦者:
                switch (PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.加速, out var OPassPlSLV) ? OPassPlSLV : 0)
                {
                    default:break;
                    case 1: SpeedMulPer += 6f; break;
                    case 2: SpeedMulPer += 12f; break;
                    case 3: SpeedMulPer += 18f; break;
                }
                if (GInfo.Ramps)
                {
                    if (PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.極限暴走, out var URampLV) && URampLV > 0) SpeedMulPer *= 1.6f;
                    else SpeedMulPer *= 1.3f;
                }
                if (PSta.PassL.TryGetValue((int)Oni_Pilot_PassE.Debug_音速移動,out var SSpLV)&&SSpLV>0) SpeedMulPer *= 10f;
                break;
        }
        var RoomProp = PhotonNetwork.CurrentRoom.CustomProperties;
        if (RoomProp.TryGetValue("GameOption0", out var Op0Val) && (bool)Op0Val) SpeedMulPer *= 3f;
        #endregion
        #region 重力・抵抗
        if (!PSta.Water)
        {
            RigVect *= 0.99f;
            if (LGrav) RigVect += Physics.gravity * 0.01f;
            else RigVect += Physics.gravity * 0.02f;
        }
        else
        {
            RigVect *= 0.95f;
            if (!LGrav) RigVect += Physics.gravity * 0.002f;
        }
        #endregion
        if (!HPChecks || PSta.HP > 0||PSta.GostModes)
        {
            if (PSta.Spakes <= 0)
            {
                #region 移動
                Vector3 MIn3 = PInd.Move.normalized;
                if (PInd.Up) MIn3.z++;
                if (PInd.Down) MIn3.z--;
                if (MIn3.magnitude >= 0.1f)
                {
                    var MoveIn = MIn3;
                    var MoveVect = CamTrans.forward * MoveIn.y + CamTrans.right * MoveIn.x + CamTrans.up * MoveIn.z;
                    float MSpeed = PSta.Water ? WaterSpeed : GroundSpeed;
                    if(PSta.GostModes)MSpeed = PSta.Water? Gost_WaterSpeed : Gost_GroundSpeed;
                    float ACSpeedP = 1f;
                    if (ACs != null)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (ACs.CTs[i] > 0) MSpeed *= 1f + ACs.ACMoveSpeedPerCh[i] * 0.01f;
                        }
                    }
                    MSpeed *= ACSpeedP;


                    if (PInd.Dash && !PSta.LowSP)
                    {
                        if (!PSta.GostModes)
                        {
                            PSta.SP--;
                            PSta.LastUseSPT = 0;
                            MSpeed *= DashMultPer * 0.01f;
                        }
                        else MSpeed *= Gost_DashMultPer * 0.01f;
                    }
                    MSpeed *= SpeedMulPer * 0.01f;
                    RigVect += MoveVect.normalized * MSpeed * 0.01f;
                }
                #endregion
            }
        }
        PSta.Rig.linearVelocity = RigVect;
    }
    private void LateUpdate()
    {
        if (!photonView.IsMine) return;
        #region 視点回転
        CRotateBase.transform.position = transform.position;
        if (PInd.Look.magnitude >= 0.1f)
        {
            Vector2 LookIn = PInd.Look * (DispOptions[0] * 0.01f) * 0.5f;
            BaseRot.x -= LookIn.y;
            BaseRot.x = Mathf.Clamp(BaseRot.x, -89f, 89f);
            BaseRot.y += LookIn.x;
        }
        CRotateBase.eulerAngles = BaseRot;

        CamTrans.localPosition = CamLoacls;
        float Dis = CamTrans.localPosition.magnitude;
        var RayCheck = Physics.SphereCastAll(transform.position, 0.2f, CamTrans.position - CRotateBase.position, Dis, CameraNearLayer);
        foreach (var RHit in RayCheck)
        {
            if (RHit.collider.isTrigger) continue;
            if (Dis > RHit.distance) Dis = RHit.distance;
        }
        CamTrans.localPosition = CamTrans.localPosition.normalized * Dis;
        #endregion
        #region 方向変化
        if (LooksRot) transform.LookAt(transform.position - (transform.position - CamTrans.position).normalized);
        #endregion
    }
    private void OnTriggerStay(Collider other)
    {
        #region 水
        if (other.tag == "Water") PSta.WaterCheck = true;
        #endregion
    }
}
