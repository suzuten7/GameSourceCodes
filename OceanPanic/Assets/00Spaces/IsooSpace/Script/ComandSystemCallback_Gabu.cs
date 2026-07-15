using UnityEngine;
using UnityEngine.InputSystem;

namespace MyNamespace
{

    /// <summary>
    /// Player Inputのコールバック
    /// </summary>
    public class ComandSystemCallback_Gabu : MonoBehaviour
    {
        public ComandSystem_Gabu commandSystem;

        private void Start()
        {
            //comandSystem = ;
        }

        #region αβ

        public void OnCommandA(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Debug.Log("Aを取得");
                commandSystem.inputed += "A";
            }
        }

        public void OnCommandB(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "B";
            }
        }

        public void OnCommandC(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "C";
            }
        }

        public void OnCommandD(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "D";
            }
        }

        public void OnCommandE(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "E";
            }
        }

        public void OnCommandF(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "F";
            }
        }

        public void OnCommandG(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "G";
            }
        }

        public void OnCommandH(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "H";
            }
        }

        public void OnCommandI(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "I";
            }
        }

        public void OnCommandJ(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "J";
            }
        }

        public void OnCommandK(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "K";
            }
        }

        public void OnCommandL(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "L";
            }
        }

        public void OnCommandM(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "M";
            }
        }

        public void OnCommandN(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "N";
            }
        }

        public void OnCommandO(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "O";
            }
        }

        public void OnCommandP(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "P";
            }
        }

        public void OnCommandQ(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "Q";
            }
        }

        public void OnCommandR(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "R";
            }
        }

        public void OnCommandS(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "S";
            }
        }

        public void OnCommandT(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "T";
            }
        }

        public void OnCommandU(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "U";
            }
        }

        public void OnCommandV(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "V";
            }
        }

        public void OnCommandW(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "W";
            }
        }

        public void OnCommandX(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "X";
            }
        }

        public void OnCommandY(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "Y";
            }
        }

        public void OnCommandZ(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "Z";
            }
        }

        #endregion

        #region 数字



        public void OnCommand0(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "0";
            }
        }

        public void OnCommand1(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "1";
            }
        }

        public void OnCommand2(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "2";
            }
        }

        public void OnCommand3(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "3";
            }
        }

        public void OnCommand4(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "4";
            }
        }

        public void OnCommand5(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "5";
            }
        }

        public void OnCommand6(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "6";
            }
        }

        public void OnCommand7(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "7";
            }
        }

        public void OnCommand8(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "8";
            }
        }

        public void OnCommand9(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                commandSystem.inputed += "9";
            }
        }


        #endregion

        public void OnCommandComma(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += ",";
            }
        }

        public void OnCommandPeriod(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += ".";
            }
        }

        public void OnCommandSlash(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "/";
            }
        }

        public void OnCommandBackSlash(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "\\";
            }
        }

        public void OnCommandSemiColon(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += ";";
            }
        }

        public void OnCommandColon(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += ":";
            }
        }

        public void OnCommandAt(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "@";
            }
        }

        public void OnCommandHyphen(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "-";
            }
        }

        public void OnCommandTilde(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "~";
            }
        }

        public void OnCommandRshift(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "rshift";
            }
        }

        public void OnCommandLshift(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "lshift";
            }
        }

        public void OnCommandLctrl(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "lctrl";
            }
        }

        public void OnCommandRctrl(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "rctrl";
            }
        }

        public void OnCommandBackSpace(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "backspace";
            }
        }

        public void OnCommandNormalSpace(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "normalspace";
            }
        }

        public void OnCommandEnter(InputAction.CallbackContext context)
        {

            if (context.started)
            {
                commandSystem.inputed += "enter";
            }
        }
    }
}
