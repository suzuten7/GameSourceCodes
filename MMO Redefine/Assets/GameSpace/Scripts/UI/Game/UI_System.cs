namespace UIs
{
    using FNet;
    using Player;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.UI;
    using static GmSystem.GS_ChangeSet;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static Player.Player_Controle;
    using static Player.Player_MoblieInput;
    public class UI_System : MonoBehaviour
    {
        static public UI_System ui_system;
        static public bool IsDrag = false;
        static public bool LookStop
        {
            get{return ui_system.MenuSlideUI.isOpen || ui_system.FullMapFadeUI.isActive || PhotoMode ; }
        }
        static public bool PhotoMode = false;
        static public bool NAtkCheck = false;
        static public Player_State PhotoContPL;
        [SerializeField] List<Class_PhotoSet> PhotoSets = new();
        public enum Enum_ItemSlotType
        {
            [InspectorName("UIスロット")] UISlot = -9999,
            [InspectorName("ボットセット")] BotSet = -10,
            [InspectorName("ショップ")] Shop = -4,
            [InspectorName("クラフト")] Craft = -3,
            [InspectorName("武器UI")] WeponUI = -2,
            [InspectorName("ショートカット")] ShortCut = -1,
            [InspectorName("素材")] Material,
            [InspectorName("消耗品")] Consumables,

            [InspectorName("スキル_全")] Skill_All = 1000,
            [InspectorName("スキル_共通")] Skill_Common,
            [InspectorName("スキル_ジョブ1")] Skill_Job1,
            [InspectorName("スキル_ジョブ2")] Skill_Job2,

            [InspectorName("所持_武器")]Has_Wepon = 10,
            [InspectorName("所持_防具")] Has_Armor,
            [InspectorName("所持_アクセサリー")] Has_Akuse,

            [InspectorName("装備_武器")] Set_Wepon = 110,
            [InspectorName("武器外見")] Wepon_Skin = 120,
            [InspectorName("装備_防具")] Set_Armor= 111,
            [InspectorName("装備_アクセサリー")] Set_Akuse,


        }

        public Color OnColor;
        public Color OffColor;
        public TextMeshProUGUI GoldTx;
        public UI_Comment_System CommentUI;
        public UI_FadeActive DeathUI;
        public UI_SlideActive MenuSlideUI;
        public UI_FadeActive WindowsFadeUI;
        public UI_FadeActive WindowsNScaleFadeUI;
        public Transform WindowsNoHide;
        public UI_FadeActive MySetUI;
        public UI_FadeActive MyPlayerStateUI;
        public UI_FadeActive ShopUI;
        public UI_FadeActive SayUI;
        public UI_FadeActive FullMapFadeUI;
        public UI_CharaBase CharaUIBase;

        public Canvas DragCanvas;
        public UI_FadeActive[] OpensUIs;
        public Image[] OpensSelUIs;
        public List<UI_CharaBase> CharaUIs;
        public GameObject[] NoPhotoModeUIs;
        public GameObject PhotoModeUI;
        public GameObject[] PhotoHideUIs;
        public GameObject PhotoHideBack;
        bool photoHide= true;
        [System.Serializable]
        public class Class_PhotoSet
        {
            public Player_State PSta;
            public int MotionSet;
            public bool AutoLAtk;
            public bool AutoRAtk;
        }
        private void Start()
        {
            ui_system = this;
            PhotoMode = true;
            PhotoUIChange();
            PhotoHidChange();
        }
        private void Update()
        {
            if (Fusion_Manager.InsRunner == null) Cursor.lockState = CursorLockMode.None;
            if(MyPlayer != null)
            {
                if(MyPlayer.HP <= 0 != DeathUI.isActive && !DeathUI.isMoving) DeathUI.OpenClose();
            }
            if (PCont.PI.actions["Menu"].triggered)
            {
                if(!MenuSlideUI.isOpen || !InputsCheck)
                {
                    if (FullMapFadeUI.isActive) FullMapFadeUI.OpenClose();
                    else MenuOpenClose();
                }
            }
            if (WindowsFadeUI.isActive != MenuSlideUI.isOpen) WindowsFadeUI.OpenClose();
            if (WindowsNScaleFadeUI.isActive != MenuSlideUI.isOpen) WindowsNScaleFadeUI.OpenClose();
            if (!MenuSlideUI.isOpen)
            {
                MySetUI.Close();
                ShopUI.Close();
                SayUI.Close();
            }

            if (PCont.PI.actions["FullMap"].triggered && !InputsCheck) MapOpenClose();
            if (MenuSlideUI.isOpen || FullMapFadeUI.isActive || MoblieIn.Moblie || DeathUI.isActive || CommentUI.isOpen || PhotoMode)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            for (int i = 0; i < LPlayerCharas.Count; i++)
            {
                if (i < CharaUIs.Count) continue;
                var InsUI = Instantiate(CharaUIBase, WindowsFadeUI.transform);
                InsUI.CharaID = i;
                CharaUIs.Add(InsUI);
            }
            for(int i = 0; i < OpensUIs.Length; i++)
            {
                ChangeColor(OpensSelUIs[i],OnColors(OpensUIs[i].isActive));
            }
            ChangeText(GoldTx, ValueStrings(LPlayerVal.Gold) + "ゴールド");

        }
        public void MenuOpenClose()
        {
            MenuSlideUI.OpenClose();
        }
        public void MapOpenClose()
        {
            FullMapFadeUI.OpenClose();
        }
        static public void ShopOpen()
        {
            ui_system.MenuSlideUI.Opens();
            ui_system.ShopUI.Opens();
        }
        static public void SayOpen()
        {
            ui_system.MenuSlideUI.Opens();
            ui_system.SayUI.Opens();
        }
        public static string ValueStrings(float val,bool noSyou=false)
        {
            if (val == 0) return val.ToString("F0");
            string addstr = "";
            int p = 3;
            int T = Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(val))) / p;
            switch (T)
            {
                case 1:
                    addstr = "K";
                    val /= Mathf.Pow(10, p * 1);
                    break;
                case 2:
                    addstr = "M";
                    val /= Mathf.Pow(10, p * 2);
                    break;
                case 3:
                    addstr = "G";
                    val /= Mathf.Pow(10, p * 3);
                    break;
                case 4:
                    addstr = "T";
                    val /= Mathf.Pow(10, p * 4);
                    break;
            }
            if (T <= 4)
            {
                if (T <= 0 && noSyou) return val.ToString("F0");
                else
                {
                    int ketad = Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(val)));
                    if (ketad <= 0 && addstr == "")return val.ToString("G" + (-ketad + 2));
                    return val.ToString("F" + (3 - ketad)) + addstr;
                }
            }
            else return val.ToString("E3");
        }
        public static bool TryStringValues(string str,out float val)
        {
            int T = 0;
            switch(str.Substring(str.Length - 1, 1))
            {
                case "K":
                case "k":
                    T = 1;
                    break;
                case "M":
                case "m":
                    T = 2;
                    break;
                case "G":
                case "g":
                    T = 3;
                    break;
                case "T":
                case "t":
                    T = 4;
                    break;
            }
            val = 0;
            try
            {
                if (T > 0)
                {
                    val = float.Parse(str.Substring(0, str.Length - 1));
                    val *= Mathf.Pow(10, T * 3);
                }
                else val = float.Parse(str);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool BackPerCheck(float CVal,float BVal,float ChangeP)
        {
            return Mathf.Abs(CVal - BVal) >= (ChangeP / 100f) || (CVal <= 0 && BVal > 0) || (CVal >= 1 && BVal < 1);
        }

        public static bool InputsCheck
        {
            get
            {
                var evsystem = EventSystem.current;
                if (evsystem == null) return false;
                var sel = evsystem.currentSelectedGameObject;
                if (sel == null) return false;
                if (!sel.TryGetComponent<TMP_InputField>(out var input)) return false;
                return input.isFocused;
            }
        }

        public void PhotoUIChange()
        {
            PhotoMode = !PhotoMode;
            for (int i = 0; i < NoPhotoModeUIs.Length; i++)
            {
                ChangeActive(NoPhotoModeUIs[i], !PhotoMode);
            }
            ChangeActive(PhotoModeUI, PhotoMode);
        }
        static public Class_PhotoSet PhotoSetGet(Player_State psta)
        {
            var set = ui_system.PhotoSets.Find(x => x.PSta == psta);
            if (set == null)
            {
                set = new Class_PhotoSet { PSta = psta };
                ui_system.PhotoSets.Add(set);
                ui_system.PhotoSets.RemoveAll(x => x == null);
            }
            return set;
        }
        public void PhotoHidChange()
        {
            photoHide = !photoHide;
            ChangeActive(PhotoHideBack, photoHide);
            for (int i = 0; i < PhotoHideUIs.Length; i++)
            {
                ChangeActive(PhotoHideUIs[i], !photoHide);
            }

        }

        static public Color OnColors(bool On)
        {
            if (ui_system == null) return On ? Color.yellow : Color.magenta;
            return On ? ui_system.OnColor : ui_system.OffColor;
        }

        public void CheckSet(bool v)
        {
            NAtkCheck = v;
        }
    }
}

