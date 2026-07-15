using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using static Suzuten_PlayerSets;
using static CameraSettings_Gabu;
using TMPro;
using UnityEngine.UI;

public class UICanvasControllerInput : MonoBehaviour
{
    [SerializeField] PlayerInput PI;
    [SerializeField] Suzuten_PlayerState PS;
    [SerializeField] ACsUIC[] ACsUIs;
    [SerializeField] GameObject ACButtons;
    [SerializeField] GameObject ACStick;

    [System.Serializable]
    class ACsUIC
    {
        public TextMeshProUGUI ACNametx;
        public TextMeshProUGUI ACCosttx;
        public Image ACCTs;
        public Image TypeIms;

        public TextMeshProUGUI StACNametx;
        public TextMeshProUGUI StACCosttx;
        public Image StACCTs;
        public Image StTypeIms;
    }
    public Vector2 MoveInputs;
    public Vector2 LookInputs;
    public bool Look_Stay;
    public bool Jump_Enter;
    public bool Boost_Enter;
    public bool Boost_Stay;
    public bool Fall_Stay;

    public bool[] AC_Enters;
    public bool[] AC_Stays;

    public Vector2 ACStickIn;
    public float Rots;


    bool JumpIn;
    bool JumpT;
    bool BoostIn;
    bool BoostT;
    bool[] ACsIn=new bool[5];
    bool[] ACsT = new bool[5];

    private void LateUpdate()
    {
        ACStick.SetActive(MoblieACStick);
        ACButtons.SetActive(!MoblieACStick);
        #region 画面ボタン操作
        if (MoblieACStick)
        {
            if (ACStickIn.magnitude <= 0.3)for (int i = 0; i < 5; i++) ACsIn[i] = false;
            else
            {
                float Rot = Mathf.Atan2(ACStickIn.y, ACStickIn.x) * Mathf.Rad2Deg;
                Rots = Rot;
                int IDs;
                if (Rot >= 60 && Rot <= 132) IDs = 4;
                else if (Rot >= -12 && Rot <= 132) IDs = 3;
                else if (Rot >= -84 && Rot <= 132) IDs = 2;
                else if (Rot >= -156 && Rot <= 132) IDs = 1;
                else IDs = 0;
                for (int i = 0; i < 5; i++) ACsIn[i] = IDs == i;
            }
        }
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
        if (BoostIn)
        {
            Boost_Enter = !BoostT;
            BoostT = true;
        }
        else
        {
            BoostT = false;
            Boost_Enter = false;
        }
        for (int i = 0; i < 5; i++)
        {
            AC_Stays[i] = ACsIn[i];
            if (ACsIn[i])
            {
                AC_Enters[i] = !ACsT[i];
                ACsT[i] = true;
            }
            else
            {
                ACsT[i] = false;
                AC_Enters[i] = false;
            }
        }
        #endregion
        #region 画面タップ操作
        Look_Stay = false;
        //FovV = 0;
        Vector2 lookv = Vector2.zero;

        int touch1 = -1;
        int touch2 = -1;
        for (int i = 0; i <= 4; i++)
        {
            TouchState ts = PI.actions["Touch" + i].ReadValue<TouchState>();
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
            TouchState ts1 = PI.actions["Touch" + touch1].ReadValue<TouchState>();
            TouchState ts2 = PI.actions["Touch" + touch2].ReadValue<TouchState>();
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
                //FovV = pinchDelta * 0.05f;
            }
        }
        else if (touch1 >= 0)
        {
            TouchState ts = PI.actions["Touch" + touch1].ReadValue<TouchState>();
            lookv = ts.delta;
            Look_Stay = true;
        }
        LookInputs = lookv * 0.2f;
        #endregion
        #region アクション表示
        for(int i = 0; i < 5; i++)
        {
            int ACID = ACSetID[PS.PID, i];
            Suzuten_ActionData AD = (ACID >= 0 && ACID < PS.CD.Actions.Length) ? PS.CD.Actions[ACID] : null;
            if (AD != null)
            {
                ACsUIs[i].ACNametx.text = AD.ACName;
                ACsUIs[i].StACNametx.text = AD.ACName;
                if (PS.SP < AD.SPCost)
                {
                    ACsUIs[i].TypeIms.color = new Color(1, 0.6f, 0.3f);
                    ACsUIs[i].StTypeIms.color = new Color(1, 0.6f, 0.3f);
                }
                else
                {
                    if (AD.SPAC)
                    {
                        Color HSVCol = Color.white;
                        Color.RGBToHSV(ACsUIs[i].TypeIms.color, out HSVCol.r, out HSVCol.g, out HSVCol.b);
                        ACsUIs[i].TypeIms.color = Color.HSVToRGB(HSVCol.r, 1f, 1f);
                        Color.RGBToHSV(ACsUIs[i].StTypeIms.color, out HSVCol.r, out HSVCol.g, out HSVCol.b);
                        ACsUIs[i].StTypeIms.color = Color.HSVToRGB(HSVCol.r, 1f, 1f);
                    }
                    else if (AD.SPCost > 0)
                    {
                        ACsUIs[i].TypeIms.color = Color.cyan;
                        ACsUIs[i].StTypeIms.color = Color.cyan;
                    }
                    else
                    {
                        ACsUIs[i].TypeIms.color = Color.white;
                        ACsUIs[i].StTypeIms.color = Color.white;
                    }
                }
                if (AD.SPCost > 0)
                {
                    ACsUIs[i].ACCosttx.text = (AD.SPCost / (PS.CD.MSP * BOP_HMSP[4] * 0.01f) * 100f).ToString("F0") + "%";
                    ACsUIs[i].ACCosttx.color = PS.SP >= AD.SPCost ? Color.green : Color.red;

                    ACsUIs[i].StACCosttx.text = (AD.SPCost / (PS.CD.MSP * BOP_HMSP[4] * 0.01f) * 100f).ToString("F0") + "%";
                    ACsUIs[i].StACCosttx.color = PS.SP >= AD.SPCost ? Color.green : Color.red;
                }
                else
                {
                    ACsUIs[i].ACCosttx.text = "";
                    ACsUIs[i].StACCosttx.text = "";
                }
                ACsUIs[i].ACCTs.fillAmount = PS.ACCTs[i] / Mathf.Max(1,AD.CT * 60f * BOP_AtkStan[7] * 0.01f);
                ACsUIs[i].StACCTs.fillAmount = PS.ACCTs[i] / Mathf.Max(1, AD.CT * 60f * BOP_AtkStan[7] * 0.01f);
            }
            else
            {
                ACsUIs[i].ACNametx.text = "無し";
                ACsUIs[i].StACNametx.text = "無し";
                ACsUIs[i].TypeIms.color = Color.gray;
                ACsUIs[i].StTypeIms.color = Color.gray;
                ACsUIs[i].ACCosttx.text = "";
                ACsUIs[i].StACCosttx.text = "";
                ACsUIs[i].ACCTs.fillAmount = 1;
                ACsUIs[i].StACCTs.fillAmount = 1;

            }
        }
        #endregion
    }

    public void MoveInput(Vector2 v)
    {
        MoveInputs = v;
    }
    public void ACStickInput(Vector2 v)
    {
        ACStickIn = v;
    }
    public void JumpInput(bool b)
    {
        JumpIn = b;
    }


    public void BoostInput(bool b)
    {
        BoostIn = b;
        Boost_Stay = b;
    }
    public void FallInput(bool b)
    {
        Fall_Stay = b;
    }
    public void AC1Input(bool b)
    {
        ACsIn[0] = b;
    }
    public void AC2Input(bool b)
    {
        ACsIn[1] = b;
    }
    public void AC3Input(bool b)
    {
        ACsIn[2] = b;
    }
    public void AC4Input(bool b)
    {
        ACsIn[3] = b;
    }
    public void ACSPInput(bool b)
    {
        ACsIn[4] = b;
    }

}


