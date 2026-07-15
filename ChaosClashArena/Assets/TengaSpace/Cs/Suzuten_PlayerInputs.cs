using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static CameraSettings_Gabu;
using static Suzuten_PlayerSets;
using Photon.Pun;
public class Suzuten_PlayerInputs : MonoBehaviourPun
{
    #region 変数
    public Suzuten_PlayerState PS;
    [SerializeField] UICanvasControllerInput MoblieInputs;
    [SerializeField] bool Mobile;
    public Vector2 MoveInput = Vector2.zero;
    [System.NonSerialized] public Vector2 LookInput = Vector2.zero;
    [System.NonSerialized] public bool Look_Stay = false;
    [System.NonSerialized] public bool Boost_Stay = false;
    [System.NonSerialized] public bool Down_Stay = false;
    [System.NonSerialized] public bool[] ACInput_Stay = new bool[5];

    [System.NonSerialized] public bool Jump_Enter = false;
    [System.NonSerialized] public bool Boost_Enter = false;
    [System.NonSerialized] public bool[] ACInput_Enter = new bool[5];


    int AutoAC = 0;

    bool AutoACT = false;
    bool AutoJumpT = false;
    bool AutoBoostT = false;
    int AutoTime = 0;
    int AutoBoost = 0;
    int AutoDown = 0;
    int AutoGuard = 0;

    int AutoJumpCT=0;
    int AutoBoostCT=0;

    bool ShotNearF = false;
    Vector3  ShotNearPos = Vector3.zero;
    bool DogeLefts = false;
    #endregion
    private void Start()
    {
        Mobile = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (!BattleFlag) return;
        #region 基本入力
        MoveInput = PS.PI.actions["Move"].ReadValue<Vector2>();
        LookInput = PS.PI.actions["Look"].ReadValue<Vector2>();
        Look_Stay = false;
        if (PS.PI.actions["LookIn_NoStick"].IsPressed()) Look_Stay = true;
        if (PS.PI.actions["LookIn_UseStick"].IsPressed()) Look_Stay = true;
        Boost_Stay = PS.PI.actions["Boost&Guard"].IsPressed();
        Down_Stay = PS.PI.actions["Down"].IsPressed();

        ACInput_Stay[0] = PS.PI.actions["Action1"].IsPressed();
        ACInput_Stay[1] = PS.PI.actions["Action2"].IsPressed();
        ACInput_Stay[2] = PS.PI.actions["Action3"].IsPressed();
        ACInput_Stay[3] = PS.PI.actions["Action4"].IsPressed();
        ACInput_Stay[4] = PS.PI.actions["ActionSP"].IsPressed();
        #endregion
        #region P2用キーボード操作
        if (PS.PI.playerIndex == 1)
        {
            if (Input.GetKey(KeyCode.UpArrow)) MoveInput.y++;
            if (Input.GetKey(KeyCode.DownArrow)) MoveInput.y--;
            if (Input.GetKey(KeyCode.LeftArrow)) MoveInput.x--;
            if (Input.GetKey(KeyCode.RightArrow)) MoveInput.x++;

            if (Input.GetKey(KeyCode.K)) Boost_Stay = true;
            if (Input.GetKey(KeyCode.J)) Down_Stay = true;

            if (Input.GetKey(KeyCode.P)) ACInput_Stay[0] = true;
            if (Input.GetKey(KeyCode.O)) ACInput_Stay[1] = true;
            if (Input.GetKey(KeyCode.I)) ACInput_Stay[2] = true;
            if (Input.GetKey(KeyCode.U)) ACInput_Stay[3] = true;
            if (Input.GetKey(KeyCode.M)) ACInput_Stay[4] = true;
        }
        #endregion
        #region モバイル入力
        if (MoblieInputs != null && PS.PI.playerIndex == 0 && Mobile)
        {
            if (MoblieInputs.MoveInputs.magnitude > 0.1f) MoveInput = MoblieInputs.MoveInputs;
            if (MoblieInputs.Look_Stay)
            {
                Look_Stay = true;
                LookInput = MoblieInputs.LookInputs;
            }
            if (MoblieInputs.Boost_Stay) Boost_Stay = true;
            if (MoblieInputs.Fall_Stay) Down_Stay = true;
            for (int i = 0; i < 5; i++)
            {
                if (MoblieInputs.AC_Stays[i]) ACInput_Stay[i] = true;
            }
        }
        #endregion
        AutoInputs_Fixed();
    }
    private void LateUpdate()
    {
        #region モバイルUI
        if (MoblieInputs != null)
        {
            if (StartTime>=120||WinNums>=-1) MoblieInputs.gameObject.SetActive(false);
            else MoblieInputs.gameObject.SetActive(PS.PI.playerIndex == 0 && Mobile);
        }
        #endregion
        if (!photonView.IsMine) return;
        if (!BattleFlag) return;
        #region 基本入力
        Jump_Enter = PS.PI.actions["Jump"].triggered;
        Boost_Enter = PS.PI.actions["Boost&Guard"].triggered;

        if (PS.PI.actions["Action1"].triggered) ACInput_Enter[0] = true;
        if (PS.PI.actions["Action2"].triggered) ACInput_Enter[1] = true;
        if (PS.PI.actions["Action3"].triggered) ACInput_Enter[2] = true;
        if (PS.PI.actions["Action4"].triggered) ACInput_Enter[3] = true;
        if (PS.PI.actions["ActionSP"].triggered) ACInput_Enter[4] = true;
        #endregion
        #region P2用キーボード操作
        if (PS.PI.playerIndex == 1)
        {
            if (Input.GetKeyDown(KeyCode.K)) Boost_Enter = true;
            if (Input.GetKeyDown(KeyCode.L)) Jump_Enter = true;

            if (Input.GetKeyDown(KeyCode.P)) ACInput_Enter[0] = true;
            if (Input.GetKeyDown(KeyCode.O)) ACInput_Enter[1] = true;
            if (Input.GetKeyDown(KeyCode.I)) ACInput_Enter[2] = true;
            if (Input.GetKeyDown(KeyCode.U)) ACInput_Enter[3] = true;
            if (Input.GetKeyDown(KeyCode.M)) ACInput_Enter[4] = true;
        }
        #endregion
        #region モバイル入力
        if (MoblieInputs != null && PS.PI.playerIndex == 0 && Mobile)
        {
            if (MoblieInputs.Jump_Enter) Jump_Enter = true;
            if (MoblieInputs.Boost_Enter) Boost_Enter = true;
            for(int i = 0; i < 5; i++)
            {
                if (MoblieInputs.AC_Enters[i]) ACInput_Enter[i] = true;
            }
        }
        #endregion
        #region 自動用
        if (Settings_Auto[PS.PI.playerIndex] <= 0) return;
        if (AutoBoostT)
        {
            Boost_Enter = true;
            AutoBoostT = false;
        }
        if (AutoJumpT)
        {
            Jump_Enter = true;
            AutoJumpT = false;
        }
        if (AutoACT)
        {
            AutoACT = false;
            ACInput_Enter[AutoAC] = true;
        }
        #endregion
    }
    #region 自動用メソッド
    void AutoInputs_Fixed()
    {
        if (Settings_Auto[PS.PI.playerIndex] <= 0) return;
        Vector3 Pos1;
        Vector3 Pos2;
        Vector3 Vect;
        float HDis;
        switch (Settings_Auto[PS.PI.playerIndex])
        {
            default: break;
            #region 常時円形移動
            case 1:
                MoveInput = Quaternion.Euler(0,0, PS.Cam.transform.eulerAngles.y) * new Vector2(Mathf.Sin(AutoTime / 30f), Mathf.Cos(AutoTime / 30f));
                Debug.Log(MoveInput);
                break;
            #endregion

            #region 常時ジャンプ
            case 2:
                if (AutoJumpCT <= 0 || PS.Grounds)
                {
                    AutoJumpT = true;
                    AutoJumpCT = 80;
                }
                break;
            #endregion
            #region 常時ジャンプ落下
            case 3:
                if (AutoJumpCT <= 0 || PS.Grounds)
                {
                    AutoJumpT = true;
                    AutoJumpCT = 80;
                }
                if (AutoJumpCT<=50)Down_Stay = true;
                break;
            #endregion
            #region 常時ガード
            case 4:

                Boost_Stay = true;
                MoveInput = Vector2.zero;

                break;
            #endregion
            #region 常時接近
            case 5:
                MoveInput = new Vector2(0, 1);
                Boost_Stay = true;
                if (AutoJumpCT <= 0)
                {
                    AutoJumpT = true;
                    AutoJumpCT = 60;
                }
                if (AutoBoostCT <= 0)
                {
                    AutoBoostT = true;
                    AutoBoostCT = 60;
                }
                break;
            #endregion
            #region 常時逃走
            case 6:
                MoveInput = new Vector2((Random.value-0.5f)*0.3f, -1);
                Boost_Stay = true;
                if (AutoJumpCT <= 0)
                {
                    AutoJumpT = true;
                    AutoJumpCT = 60;
                }
                if (AutoBoostCT <= 0)
                {
                    AutoBoostT = true;
                    AutoBoostCT = 60;
                }
                break;
            #endregion
            #region てきとう自動戦闘LV1
            case 7:
                AutoTime = (int)Mathf.Repeat(AutoTime, 360);
                MoveInput = new Vector2(Mathf.Sin(AutoTime/30f), Mathf.Cos(AutoTime / 30f)+0.5f);
                if (Random.value >= 0.95f) AutoDown = 15;
                if (Random.value >= 0.95f) AutoJumpT = true;
                if (Random.value >= 0.95f)
                {
                    AutoBoost = 10;
                    AutoBoostT = true;
                }
                if (AutoTime % 70 == 20)
                {
                    AutoAC = Random.Range(0, 5);
                    AutoACT = true;
                }
                break;
            #endregion
            #region てきとう自動戦闘LV2
            case 8:
                AutoTime = (int)Mathf.Repeat(AutoTime, 360);
                AutoShotCheck(4);
                if (!ShotNearF&& (PS.LastDamTime > 25 || PS.LastDamTime % 5 >= 3))
                {
                    MoveInput = new Vector2(Mathf.Sin(AutoTime / 40f), Mathf.Cos(AutoTime / 40f) + 0.6f);
                    if (PS.LastDamTime > 25&&(AutoTime % 45 == 20 || Random.value >= 0.97f))
                    {
                        AutoAC = Random.Range(0, 5);
                        AutoACT = true;
                    }
                }
                else AutoGuard = 6;
                Pos1 = PS.PosGet();
                Pos2 = PS.Target? PS.Target.PosGet() : (PS.PosGet()+ PS.PosGet().normalized*10f);
                HDis = Vector2.Distance(new Vector2(Pos1.x, Pos1.z), new Vector2(Pos2.x, Pos2.z));
                if (HDis<=PS.CD.PhisRange && Random.value >= 0.8f) AutoDown = 15;
                if (Random.value >= 0.95f)
                {
                    AutoBoost = 10;
                    AutoBoostT = true;
                }
                if (ShotNearF && (AutoJumpCT <= 0 && PS.MP / (PS.CD.MMP * (BOP_HMSP[2] * 0.01)) >= 0.5f))
                {
                    Jump_Enter = true;
                    AutoJumpCT = 30;
                }
                break;
            #endregion
            #region てきとう自動戦闘LV3
            case 9:
                AutoTime = (int)Mathf.Repeat(AutoTime, 360);
                AutoShotCheck(8);
                if (!ShotNearF)
                {
                    MoveInput = new Vector2(Mathf.Sin(AutoTime / 40f), Mathf.Cos(AutoTime / 40f) + 0.6f);
                }
                else
                {
                    Vect = PS.PosGet() - ShotNearPos;
                    if (Vect.magnitude<=2.5f)AutoGuard = 6;
                    else
                    {
                        MoveInput = Quaternion.Euler(0, 0, PS.Cam.transform.eulerAngles.y + (DogeLefts ? -45:45)) * new Vector2(Vect.normalized.x, Vect.normalized.z);
                        if (Vect.magnitude <= 5.5f && AutoBoostCT <=0)
                        {
                            AutoBoost = 20;
                            AutoBoostT = true;
                            AutoBoostCT = Random.Range(8,13);
                        }
                    }
                }
                Pos1 = PS.PosGet();
                Pos2 = PS.Target ? PS.Target.PosGet() : (PS.PosGet() + PS.PosGet().normalized * 10f);
                HDis = Vector2.Distance(new Vector2(Pos1.x, Pos1.z), new Vector2(Pos2.x, Pos2.z));
                if (HDis <= PS.CD.PhisRange && Pos1.y>Pos2.y && Random.value >= 0.8f) AutoDown = 15;
                if (AutoTime % 52 == 20 || Random.value >= 0.93f)
                {
                    AutoAC = Random.Range(0, 5);
                    AutoACT = true;
                    DogeLefts = Random.value < 0.5f ? false : true;
                }
                if (ShotNearF || (AutoJumpCT <= 0 && PS.MP / (PS.CD.MMP * (BOP_HMSP[2] * 0.01)) >= 0.5f))
                {
                    Jump_Enter = true;
                    AutoJumpCT = Random.Range(20, 80);
                }
                break;
            #endregion
            #region 全力衝突モード
            case 10:
                AutoTime = (int)Mathf.Repeat(AutoTime, 200);
                AutoShotCheck(8);
                if (!ShotNearF && (PS.LastDamTime > 25 || PS.LastDamTime % 5 >= 3))
                {
                    MoveInput = new Vector2(Mathf.Sin(AutoTime / 40f)*0.2f, 1f);
                    if (PS.LastDamTime > 25&&(AutoTime == 10 || Random.value >= 0.98f))
                    {
                        AutoAC = Random.Range(0, 5);
                        AutoACT = true;
                        DogeLefts = Random.value < 0.5f ? false : true;
                    }
                }
                else
                {
                    Vect = PS.PosGet() - ShotNearPos;
                    if (Vect.magnitude <= 2.5f || (PS.LastDamTime < 25 && PS.LastDamTime % 5 < 3)) AutoGuard = 6;
                    else
                    {
                        MoveInput = Quaternion.Euler(0, 0, PS.Cam.transform.eulerAngles.y + (DogeLefts ? -45 : 45)) * new Vector2(Vect.normalized.x, Vect.normalized.z);
                        if (Vect.magnitude <= 5.5f&&AutoBoostCT <= 0)
                        {
                            AutoBoost = 20;
                            AutoBoostT = true;
                            AutoBoostCT = Random.Range(8, 13);
                        }
                    }
                }
                Pos1 = PS.PosGet();
                Pos2 = PS.Target ? PS.Target.PosGet() : (PS.PosGet() + PS.PosGet().normalized * 10f);
                HDis = Vector2.Distance(new Vector2(Pos1.x, Pos1.z), new Vector2(Pos2.x, Pos2.z));
                if (HDis <= PS.CD.PhisRange && Pos1.y > Pos2.y && Random.value >= 0.8f) AutoDown = 15;
                if (HDis <= PS.CD.PhisRange && Mathf.Abs(Pos1.y - Pos2.y) <= 0.5f && AutoBoostCT <= 0)
                {
                    AutoBoost = 20;
                    AutoBoostT = true;
                    AutoBoostCT = Random.Range(8, 13);
                }
                if (AutoBoostCT <= 0 && PS.MP / (PS.CD.MMP * (BOP_HMSP[2] * 0.01)) >= 0.3f)
                {
                    AutoBoost = 20;
                    AutoBoostT = true;
                    AutoBoostCT = Random.Range(25, 90);
                }
                if (ShotNearF || (AutoJumpCT <= 0&& PS.MP / (PS.CD.MMP*(BOP_HMSP[2]*0.01)) >= 0.5f))
                {
                    Jump_Enter = true;
                    AutoJumpCT = Random.Range(20, 80);
                }
                break;
            #endregion
            #region 全力回避モード
            case 11:
                AutoTime = (int)Mathf.Repeat(AutoTime, 200);
                AutoShotCheck(12);
                if (!ShotNearF && (PS.LastDamTime > 25|| PS.LastDamTime%5>=3))
                {
                    MoveInput = new Vector2(Mathf.Sin(AutoTime / 40f), Mathf.Cos(AutoTime / 40f)*0.3f);
                }
                else
                {
                    Vect = PS.PosGet() - ShotNearPos;
                    if (Vect.magnitude <= 2.5f|| (PS.LastDamTime < 25 && PS.LastDamTime % 5 < 3)) AutoGuard = 6;
                    else
                    {
                        MoveInput = Quaternion.Euler(0, 0, PS.Cam.transform.eulerAngles.y + (DogeLefts ? -45 : 45)) * new Vector2(Vect.normalized.x, Vect.normalized.z);
                        if (Vect.magnitude <= 5.5f && AutoBoostCT <= 0)
                        {
                            AutoBoost = 20;
                            AutoBoostT = true;
                            AutoBoostCT = Random.Range(8, 13);
                            DogeLefts = Random.value < 0.5f ? false : true;
                        }
                    }
                }
                Pos1 = PS.PosGet();
                Pos2 = PS.Target ? PS.Target.PosGet() : (PS.PosGet() + PS.PosGet().normalized * 10f);
                HDis = Vector2.Distance(new Vector2(Pos1.x, Pos1.z), new Vector2(Pos2.x, Pos2.z));
                if (ShotNearF || (AutoJumpCT <= 0 && PS.MP / (PS.CD.MMP * (BOP_HMSP[2] * 0.01)) >= 0.5f))
                {
                    Jump_Enter = true;
                    AutoJumpCT = Random.Range(20,80);
                }
                break;
            #endregion
            #region 常時AC1
            case 12:
                AutoAC = 0;
                if (PS.ActionTime == 0)
                {
                    AutoACT = true;
                }
                break;
            #endregion
            #region 常時AC2
            case 13:
                AutoAC = 1;
                if (PS.ActionTime == 0)
                {
                    AutoACT = true;
                }
                break;
            #endregion
            #region 常時AC3
            case 14:
                AutoAC = 2;
                if (PS.ActionTime == 0)
                {
                    AutoACT = true;
                }
                break;
            #endregion
            #region 常時AC4
            case 15:
                AutoAC = 3;
                if (PS.ActionTime == 0)
                {
                    AutoACT = true;
                }
                break;
            #endregion
            #region 常時ACSP
            case 16:
                AutoAC = 4;
                if (PS.ActionTime == 0)
                {
                    AutoACT = true;
                }
                break;
            #endregion
            #region 常時ランダムAC
            case 17:
                if (PS.ActionTime == 0||AutoTime>=120)
                {
                    AutoAC = Random.Range(0,5);
                    AutoACT = true;
                    AutoTime = 0;
                }
                break;
            #endregion
        }
        #region 後処理
        AutoTime++;
        AutoBoostCT--;
        AutoJumpCT--;
        if (AutoDown > 0)
        {
            Down_Stay = true;
            AutoDown--;
        }
        if (AutoBoost > 0)
        {
            Boost_Stay = true;
            AutoBoost--;
        }
        if (AutoGuard > 0)
        {
            Boost_Stay = true;
            MoveInput = Vector2.zero;
            AutoGuard--;
        }
        ACInput_Stay[AutoAC] = true;
        #endregion
    }
    void AutoShotCheck(float Diss)
    {
        ShotNearF = false;
        float MDis = Diss;
        foreach (var SObj in FindObjectsOfType<Suzuten_ShotObj>())
        {
            if (SObj.UsePS != PS)
            {
                Collider ShotCol = SObj.GetComponent<Collider>();
                if (ShotCol)
                {
                    Vector3 SPos = ShotCol.ClosestPoint(PS.PosGet());
                    float SDis = (PS.PosGet() - SPos).magnitude;
                    //Debug.Log(SObj.name +":"+ SDis);
                    if (MDis > SDis)
                    {
                        //Debug.Log(SObj.name + ":" + SDis);
                        MDis = SDis;
                        ShotNearF = true;
                        ShotNearPos = SPos;
                    }
                }
            }
        }
    }
    #endregion
}
