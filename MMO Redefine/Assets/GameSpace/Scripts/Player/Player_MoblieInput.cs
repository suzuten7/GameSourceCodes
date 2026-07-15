
namespace Player
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem.LowLevel;
    using static Player_Controle;
    using static GmSystem.GS_ChangeSet;
    public class Player_MoblieInput : MonoBehaviour
    {
        public bool Moblie;
        public GameObject InputUIs;
        public float LookMult;
        static public Player_MoblieInput MoblieIn;

        public Vector2 V2_Move;
        public Vector2 V2_Look;
        public bool In_Jump;
        public bool Stay_Jump;
        bool bstay_Jump;
        public bool In_Dash;
        public bool Stay_Dash;
        bool bstay_Dash;
        public bool Stay_Fall;

        public bool Stay_LockOn;

        public bool In_AtkFBC;

        public bool In_Atk1;
        public bool Stay_Atk1;
        bool bstay_Atk1;
        public bool In_Atk2;
        public bool Stay_Atk2;
        bool bstay_Atk2;

        public bool In_ShortCutBack;

        public bool In_Menu;
        public bool In_FullMap;

        public bool LookT;
        public float Dis;
        public bool DisT;

        public bool IS_A1;
        public bool IS_AW;
        public bool IS_A2;
        void Start()
        {
            MoblieIn = this;
            switch (Application.platform)
            {
                default: Moblie = false; break;
                case RuntimePlatform.WindowsEditor: break;
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    Moblie = true;
                    break;
            }
        }
        void Update()
        {
            ChangeActive(InputUIs, Moblie);
            if (!Moblie) return;
            LookInputs();

            if (Stay_Dash && !bstay_Dash) In_Dash = true;
            bstay_Dash = Stay_Dash;

            if (Stay_Jump && !bstay_Jump) In_Jump = true;
            bstay_Jump = Stay_Jump;

            Stay_Atk1 = IS_A1 || IS_AW;
            Stay_Atk2 = IS_A2 || IS_AW;

            if(Stay_Atk1 && !bstay_Atk1) In_Atk1 = true;
            bstay_Atk1 = Stay_Atk1;

            if(Stay_Atk2 && !bstay_Atk2) In_Atk2 = true;
            bstay_Atk2 = Stay_Atk2;


        }
        void LookInputs()
        {
            #region 画面タップ操作
            LookT = false;
            DisT = false;
            Dis = 0;
            Vector2 lookv = Vector2.zero;

            int touch1 = -1;
            int touch2 = -1;
            for (int i = 0; i <= 4; i++)
            {
                TouchState ts = PCont.PI.actions["Touch_" + i].ReadValue<TouchState>();
                if (ts.isInProgress == true)
                {
                    var results = new List<RaycastResult>();
                    var pointer = new PointerEventData(EventSystem.current)
                    {
                        position = ts.startPosition
                    };
                    EventSystem.current.RaycastAll(pointer, results);
                    bool CamUI = true;
                    foreach (RaycastResult target in results)
                    {
                        if (target.gameObject.CompareTag("CamNoUI"))
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
                TouchState ts1 = PCont.PI.actions["Touch_" + touch1].ReadValue<TouchState>();
                TouchState ts2 = PCont.PI.actions["Touch_" + touch2].ReadValue<TouchState>();
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
                TouchState ts = PCont.PI.actions["Touch_" + touch1].ReadValue<TouchState>();
                lookv = ts.delta;
                LookT = true;
            }
            V2_Look = lookv * LookMult;

            #endregion
        }
        public void MoveInput(Vector2 vect)
        {
            V2_Move = vect;
        }
        public void LookInput(Vector2 vect)
        {
            V2_Look = vect;
        }
        public void Dash_Input(bool val)
        {
            Stay_Dash = val;
        }
        public void Jump_Input(bool val)
        {
            Stay_Jump = val;
        }
        public void Fall_Input(bool val)
        {
            Stay_Fall = val;
        }
        public void LockOn_Input(bool val)
        {
            Stay_LockOn = val;
        }
        public void AtkFB_Input()
        {
            In_AtkFBC = true;
        }
        public void A1_Input(bool val)
        {
            IS_A1 = val;
        }
        public void AW_Input(bool val)
        {
            IS_AW = val;
        }
        public void A2_Input(bool val)
        {
            IS_A2 = val;
        }
        public void SCBacks()
        {
            In_ShortCutBack = true;
        }

    }
}


