
namespace Player
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using static Player_MoblieInput;
    using static FNet.Fusion_Manager;
    using static UIs.UI_System;
    public class Player_Controle : MonoBehaviour
    {
        static public Player_Controle PCont;
        public bool NoRCheck;
        public PlayerInput PI;
        public Vector2 V2_Move;
        public Vector2 V2_Look;
        public bool In_Jump;
        public bool Stay_Jump;
        public bool In_Dash;
        public bool Tr_LockOn;
        public bool Stay_LockOn;
        public float StayT_LockOn;
        public bool Stay_Dash;
        public bool Stay_Fall;
        public bool In_AtkFBC;
        public bool In_Atk1;
        public bool Stay_Atk1;
        public bool In_Atk2;
        public bool Stay_Atk2;
        public bool In_ShortCutBack;
        public int In_ShortCutSet;
        public int Sel_ShortCutSet;
        public bool In_AcUp;
        public bool In_AcDown;
        public bool In_AcPlay;
        float AcSel = 0;

        public bool In_Menu;
        public bool In_FullMap;


        virtual protected void Start()
        {
            if (PCont == null)PCont = this;
        }
        virtual protected void Update()
        {
            if (!NoRCheck && InsRunner == null)
            {
                InputClear();
            }
            else
            {
                var UIOpen = ui_system != null && (ui_system.MenuSlideUI.isOpen || ui_system.CommentUI.isOpen);
                if (UIOpen && InputsCheck)
                {
                    InputClear();
                    return;
                }
                if (Stay_LockOn) StayT_LockOn += Time.deltaTime;
                else StayT_LockOn = 0;
                InputPlInput();
                if(UIOpen && !NAtkCheck)
                {
                    In_Atk1 = false;
                    Stay_Atk1 = false;
                    In_Atk2 = false;
                    Stay_Atk2 = false;
                }
                InputMoblie();
            }

        }
        public void InputClear()
        {
            V2_Move = Vector2.zero;
            V2_Look = Vector2.zero;
            In_Jump = false;
            Stay_Jump = false;
            In_Dash = false;
            Stay_Dash = false;
            Stay_Fall = false;
            In_AtkFBC = false;
            In_Atk1 = false;
            Stay_Atk1 = false;
            In_Atk2 = false;
            Stay_Atk2 = false;
            In_ShortCutBack = false;
            In_ShortCutSet = 0;
            In_AcUp = false;
            In_AcDown = false;
            In_AcPlay = false;
            Stay_LockOn = false;
            StayT_LockOn = 0;
        }
        void InputPlInput()
        {
            V2_Move = PI.actions["Move"].ReadValue<Vector2>();
            V2_Look = PI.actions["Look"].ReadValue<Vector2>();
            if (PI.actions["Jump"].triggered) In_Jump = true;
            Stay_Jump = PI.actions["Jump"].IsPressed();
            if (PI.actions["Dash"].triggered) In_Dash = true;
            Stay_Dash = PI.actions["Dash"].IsPressed();
            Stay_Fall = PI.actions["Fall"].IsPressed();
            Stay_LockOn = PI.actions["LockOn"].IsPressed();
            if (PI.actions["AtkFBC"].triggered) In_AtkFBC = true;
            if (PI.actions["Atk1"].triggered) In_Atk1 = true;
            Stay_Atk1 = PI.actions["Atk1"].IsPressed();
            if (PI.actions["Atk2"].triggered) In_Atk2 = true;
            Stay_Atk2 = PI.actions["Atk2"].IsPressed();
            if(PI.actions["SCBack"].triggered) In_ShortCutBack = true;
            for (int i = 1; i <= 10; i++)
            {
                if (PI.actions["Sh" + i].triggered)
                {
                    In_ShortCutSet = i;
                    Sel_ShortCutSet = i - 1;
                }
            }
            if (PI.actions["ShAdd"].triggered) Sel_ShortCutSet++;
            if (PI.actions["ShBack"].triggered) Sel_ShortCutSet--;
            Sel_ShortCutSet = (int)Mathf.Repeat(Sel_ShortCutSet, 10);
            if (PI.actions["ShUse"].triggered) In_ShortCutSet = Sel_ShortCutSet+1;
            if (PI.actions["AcUp"].triggered) In_AcUp = true;
            if (PI.actions["AcDown"].triggered) In_AcDown = true;
            if (PI.actions["AcPlay"].triggered) In_AcPlay = true;
            AcSel += PI.actions["AcSel"].ReadValue<float>() * 0.5f;
            if(AcSel <= -1)
            {
                AcSel += 1;
                In_AcDown = true;
            }
            if (AcSel >= 1)
            {
                AcSel -= 1;
                In_AcUp = true;
            }

            if (PI.actions["Menu"].triggered) In_Menu = true;
            if (PI.actions["FullMap"].triggered) In_FullMap = true;

        }
        void InputMoblie()
        {
            if (MoblieIn == null) return;
            if (MoblieIn.V2_Move != Vector2.zero) V2_Move = MoblieIn.V2_Move;
            if (MoblieIn.V2_Look != Vector2.zero) V2_Look = MoblieIn.V2_Look;
            if (MoblieIn.Stay_Dash) Stay_Dash = true;
            if (MoblieIn.In_Dash) In_Dash = true;
            MoblieIn.In_Dash = false;
            if (MoblieIn.Stay_Jump) Stay_Jump = true;
            if (MoblieIn.In_Jump) In_Jump = true;
            MoblieIn.In_Jump = false;
            if (MoblieIn.Stay_Fall) Stay_Fall = true;
            if (MoblieIn.Stay_LockOn) Stay_LockOn = true;
            if (MoblieIn.In_AtkFBC) In_AtkFBC = true;
            MoblieIn.In_AtkFBC = false;
            if (MoblieIn.Stay_Atk1) Stay_Atk1 = true;
            if (MoblieIn.In_Atk1) In_Atk1 = true;
            MoblieIn.In_Atk1 = false;
            if (MoblieIn.Stay_Atk2) Stay_Atk2 = true;
            if (MoblieIn.In_Atk2) In_Atk2 = true;
            MoblieIn.In_Atk2 = false;
            if (MoblieIn.In_ShortCutBack) In_ShortCutBack = true;
            MoblieIn.In_ShortCutBack = false;
        }

    }
}
