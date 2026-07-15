using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Suzuten_PlayerSets;
using static Suzuten_SelectUI;
using Photon.Pun;
public class Suzuten_SelectInput : MonoBehaviour
{
    
    public Suzuten_SelectUI SUI;
    public PlayerInput PI;
    public int ButtonPID;
    float InputCT=0;
    [SerializeField] GameObject NetUI;
    private void OnValidate()
    {
        PI.defaultControlScheme = SUI.DB.KeyBoardUse ? "<Any>" : "Gamepad";
    }
    void Update()
    {
        if (NetUI != null && NetUI.activeSelf) return;
        int PID = PI.playerIndex;
        if (PID >= 0)
        {
            
            if (PI.actions["ActionSP"].triggered ||(PID==1&&Input.GetKeyDown(KeyCode.M)))
            {
                if (PhotonNetwork.OfflineMode)
                {
                    SUI.UIClose(PID);
                    SUI.PLOKs[PID] = !SUI.PLOKs[PID];
                }
            }
            if (SUI.PLOKs[PID]) return;

            if (PI.actions["Action1"].triggered || (PID == 1 && Input.GetKeyDown(KeyCode.P)))
            {
                StateOC(PID);
            }
            if (PI.actions["Action2"].triggered || (PID == 1 && Input.GetKeyDown(KeyCode.O)))
            {
                ACOC(PID);
            }
            bool InputsB = PI.actions["SelNoStick"].triggered;
            Vector2 InputM = PI.actions["SelNoStick"].ReadValue<Vector2>();
            InputCT -= 0.02f;
            Vector2 InputUseStick = PI.actions["SelUseStick"].ReadValue<Vector2>();
            if (InputUseStick.magnitude >= 0.5&&InputCT<=0)
            {
                InputCT = 0.45f;
                InputM = InputUseStick;
                InputsB = true;
            }
            if (PID == 1)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    InputM.y++;
                    InputsB = true;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    InputM.y--;
                    InputsB = true;
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    InputM.x--;
                    InputsB = true;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    InputM.x++;
                    InputsB = true;
                }

            }

            if (InputsB)
            {

                if (InputM.magnitude >= 0.5f)
                {
                    if (SUI.CUIs[PID].ACUI.activeSelf)
                    {
                        int ACID = SUI.SelACID[PID];
                        int ACSy = SUI.SelACSy[PID];
                        if (InputM.x > 0.1f) ACSy += 1;
                        else if (InputM.x < -0.1f) ACSy -= 1;

                        if (!SUI.SelACSetB[PID])
                        {
                            if (InputM.y > 0.1f) ACID += 1;
                            else if (InputM.y < -0.1f) ACID -= 1;
                            ACID = (int)Mathf.Repeat(ACID, 5);
                            SUI.SelACID[PID] = ACID;
                        }
                        else if(CharaID[PID]>=0)
                        {
                            int SetID = SUI.SelACSetSlot[PID];
                            if (InputM.y > 0.1f) SetID -= 1;
                            else if (InputM.y < -0.1f) SetID += 1;
                            SUI.SelACSetSlot[PID] = (int)Mathf.Repeat(SetID+1, SUI.DB.Charas[CharaID[PID]].Actions.Length+1)-1;
                        }
                        ACSy = (int)Mathf.Repeat(ACSy, 2);
                        SUI.SelACSy[PID] = ACSy;
                    }
                    else
                    {

                        Vector2Int SID = SelectSlots[PID];
                        if (InputM.x > 0.1f) SID.x += 1;
                        else if (InputM.x < -0.1f) SID.x -= 1;
                        else if (InputM.y > 0.1f) SID.y -= 1;
                        else if (InputM.y < -0.1f) SID.y += 1;

                        int Charay = (SUI.DB.Charas.Length+ RaCharaCo) / SUI.CharaXcounts;

                        SID.y = (int)Mathf.Repeat(SID.y, Charay + 1);
                        SID.x = (int)Mathf.Repeat(SID.x, SUI.CharaXcounts);

                        SelectSlots[PID] = SID;
                    }
                }
            }
            if (SUI.CUIs[PID].ACUI.activeSelf)
            {
                if (PI.actions["Action3"].triggered || (PID == 1 && Input.GetKeyDown(KeyCode.I)))
                {
                    ACSetsOC(PID);
                }
                if (PI.actions["Action4"].triggered || (PID == 1 && Input.GetKeyDown(KeyCode.U)))
                {
                    ACReset(PID);
                }
            }

        }
    }
    /// <summary>ステータスUI開閉</summary>
    public void StateOC(int PID)
    {
        if (!SUI.CUIs[PID].ParaUI.activeSelf)
        {
            SUI.UIClose(PID);
            SUI.CUIs[PID].ParaUI.SetActive(true);
        }
        else SUI.UIClose(PID);
    }
    /// <summary>アクションUI開閉</summary>
    public void ACOC(int PID)
    {
        if (!SUI.CUIs[PID].ACUI.activeSelf)
        {
            SUI.UIClose(PID);
            SUI.CUIs[PID].ACUI.SetActive(true);
        }
        else SUI.UIClose(PID);
    }
    /// <summary>アクション設定UI開閉</summary>
    public void ACSetsOC(int PID)
    {
        if (!SUI.SelACSetB[PID])
        {
            SUI.SelACSetSlot[PID] = ACSetID[PID, SUI.SelACID[PID]];
        }
        else
        {
            int Used = -1;
            if (SUI.SelACSetSlot[PID] >= 0)
            {
                for (int k = 0; k < 5; k++)
                {
                    if (SUI.SelACSetSlot[PID] == ACSetID[PID, k]) Used = k;
                }
            }
            if (Used < 0) ACSetID[PID, SUI.SelACID[PID]] = SUI.SelACSetSlot[PID];
            else
            {
                int SlotB = ACSetID[PID, SUI.SelACID[PID]];
                ACSetID[PID, SUI.SelACID[PID]] = SUI.SelACSetSlot[PID];
                ACSetID[PID, Used] = SlotB;
            }
            ACIDSave(SUI.DB, PID);
        }
        SUI.SelACSetB[PID] = !SUI.SelACSetB[PID];
    }
    /// <summary>アクション設定リセット</summary>
    public void ACReset(int PID)
    {
        for (int k = 0; k < 5; k++) ACSetID[PID, k] = k;
        ACIDSave(SUI.DB, PID);
        SUI.SelACSetB[PID] = false;

    }
    /// <summary>アクション詳細切り替え</summary>
    public void ACSySet(int ACSy)
    {
        SUI.SelACSy[ButtonPID] = ACSy;
    }
    /// <summary>アクション選択</summary>
    public void ACSelect(int ACID)
    {
        SUI.SelACID[ButtonPID] = ACID;
    }
    /// <summary>アクション設定変更</summary>
    public void ACChange(int Val)
    {
        int PID = ButtonPID;
        if (Val == 0) ACSetsOC(PID);
        else if (CharaID[PID] >= 0)
        {
            int SetID = SUI.SelACSetSlot[PID];
            SetID += Val;
            SUI.SelACSetSlot[PID] = (int)Mathf.Repeat(SetID + 1, SUI.DB.Charas[CharaID[PID]].Actions.Length + 1) - 1;
        }
    }
}
