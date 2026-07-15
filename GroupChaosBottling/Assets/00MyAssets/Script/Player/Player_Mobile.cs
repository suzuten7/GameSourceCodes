using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class Player_Moblie : MonoBehaviour
{
    [SerializeField] PlayerInput PI;
    [SerializeField] float LookMult;

    public Vector2 MoveInputs;
    public Vector2 LookInputs;
    public bool LookT;
    public float Dis;
    public bool DisT;
    public bool Jump_Enter;
    public bool Dash_Enter;
    public bool Target_Enter;

    public bool[] Atk_Enters;
    public bool[] Atk_Stays;
    public bool Change_Enter;

    bool JumpIn;
    bool JumpT;
    bool DashIn;
    bool DashT;
    bool ChangeIn;
    bool ChangeT;
    bool TargetIn;
    bool TargetT;
    bool[] AtksIn = new bool[4];
    bool[] AtksT = new bool[4];

    private void LateUpdate()
    {
        #region 画面ボタン操作
        if (JumpIn)
        {
            Jump_Enter = !JumpT;
            JumpT = true;
        }
        else
        {
            JumpT = false;
            Jump_Enter = false;
        }
        if (DashIn)
        {
            Dash_Enter = !DashT;
            DashT = true;
        }
        else
        {
            DashT = false;
            Dash_Enter = false;
        }
        if (ChangeIn)
        {
            Change_Enter = !ChangeT;
            ChangeT = true;
        }
        else
        {
            Change_Enter = false;
            ChangeT = false;
        }
        if (TargetIn)
        {
            Target_Enter = !TargetT;
            TargetT = true;
        }
        else
        {
            Target_Enter = false;
            TargetT = false;
        }
        for (int i = 0; i < 4; i++)
        {
            Atk_Stays[i] = AtksIn[i];
            if (AtksIn[i])
            {
                Atk_Enters[i] = !AtksT[i];
                AtksT[i] = true;
            }
            else
            {
                AtksT[i] = false;
                Atk_Enters[i] = false;
            }
        }
        #endregion
        #region 画面タップ操作
        LookT = false;
        DisT = false;
        Dis = 0;
        Vector2 lookv = Vector2.zero;

        int touch1 = -1;
        int touch2 = -1;
        for (int i = 0; i <= 4; i++)
        {
            TouchState ts = PI.actions["Touch_" + i].ReadValue<TouchState>();
            if (ts.isInProgress == true)
            {
                List<RaycastResult> results = new List<RaycastResult>();
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = ts.startPosition;
                EventSystem.current.RaycastAll(pointer, results);
                bool CamUI = true;
                foreach (RaycastResult target in results)
                {
                    if (target.gameObject.tag == "CamNoUI")
                    {
                        CamUI = false;
                        break;
                    }
                }
                if (CamUI == true)
                {
                    if (touch1 < 0) touch1 = i;
                    else if (touch2 < 0) touch2 = i;
                }
            }
        }
        if (touch1 >= 0 && touch2 >= 0)
        {
            DisT = true;
            TouchState ts1 = PI.actions["Touch_" + touch1].ReadValue<TouchState>();
            TouchState ts2 = PI.actions["Touch_" + touch2].ReadValue<TouchState>();
            var pos0 = ts1.position;
            var pos1 = ts2.position;

            // 移動量（スクリーン座標）
            var delta0 = ts1.delta;
            var delta1 = ts2.delta;

            // 移動前の位置（スクリーン座標）
            var prevPos0 = pos0 - delta0;
            var prevPos1 = pos1 - delta1;

            // 距離の変化量を求める
            var pinchDelta = Vector3.Distance(pos0, pos1) - Vector3.Distance(prevPos0, prevPos1);
            if (pinchDelta != 0)
            {
                Dis = pinchDelta * 0.05f;
            }
        }
        else if (touch1 >= 0)
        {
            TouchState ts = PI.actions["Touch_" + touch1].ReadValue<TouchState>();
            lookv = ts.delta;
            LookT = true;
        }
        LookInputs = lookv * LookMult;
        #endregion
    }

    public void MoveInput(Vector2 v)
    {
        MoveInputs = v;
    }
    public void JumpInput(bool b)
    {
        JumpIn = b;
    }
    public void DashInput(bool b)
    {
        DashIn = b;
    }
    public void Atk1Input(bool b)
    {
        AtksIn[0] = b;
    }
    public void Atk2Input(bool b)
    {
        AtksIn[1] = b;
    }
    public void Atk3Input(bool b)
    {
        AtksIn[2] = b;
    }
    public void Atk4Input(bool b)
    {
        AtksIn[3] = b;
    }
    public void ChangeInput(bool b)
    {
        ChangeIn = b;
    }
    public void TargetInput(bool b)
    {
        TargetIn = b;
    }
}
