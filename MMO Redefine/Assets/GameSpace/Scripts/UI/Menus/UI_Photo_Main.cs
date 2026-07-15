namespace UIs
{
    using Player;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static UI_System;
    using static GmSystem.GS_ChangeSet;
    public class UI_Photo_Main : MonoBehaviour
    {
        [SerializeField]List<UI_Photo_Player> PLUIs;
        [SerializeField] Image CamSelFlame;
        [SerializeField] GameObject SetUI;

        [SerializeField] TMP_Dropdown MotionDr;
        [SerializeField] Toggle AutoLAtkTo;
        [SerializeField] Toggle AutoRAtkTo;
        [SerializeField] Toggle StatusHideTo;
        static public bool StatusHide = false;

        private void LateUpdate()
        {
            ChangeColor(CamSelFlame,OnColors(PhotoContPL == null));
            ChangeOn(StatusHideTo, StatusHide);
            ChangeActive(SetUI, PhotoContPL != null);
            if (PhotoContPL != null)
            {
                var set = PhotoSetGet(PhotoContPL);
                ChangeValue(MotionDr, set.MotionSet);
                ChangeOn(AutoLAtkTo, set.AutoLAtk);
                ChangeOn(AutoRAtkTo, set.AutoRAtk);
            }
            for (int i = 0; i < Mathf.Max(MyPList.Count,PLUIs.Count) ; i++)
            {
                if (i >= PLUIs.Count) PLUIs.Add(Instantiate(PLUIs[0], PLUIs[0].transform.parent));
                var sui = PLUIs[i];
                if (i >= MyPList.Count)
                {
                    ChangeActive(sui.gameObject, false);
                    continue;
                }

                var ps = MyPList[i];
                if (ps == null)
                {
                    ChangeActive(sui.gameObject, false);
                    continue;
                }
                var lc = LPlayerCharas[ps.CharaID];
                ChangeActive(sui.gameObject, true);
                sui.PSta = ps;
                ChangeText(sui.Name, "["+(i+ 1)+"]"+ lc.Name);
                ChangeTexture(sui.Icon, lc.PlayerIconGet(out _),true);
                ChangeColor(sui.SelFlame,OnColors(PhotoContPL == ps));
            }
        }
        public void PhotoSelCam()
        {
            PhotoContPL = null;
        }
        public void SetMotionChange()
        {
            PhotoSetGet(PhotoContPL).MotionSet = MotionDr.value;
        }
        public void AutoLAtkChange()
        {
            PhotoSetGet(PhotoContPL).AutoLAtk = AutoLAtkTo.isOn;
        }
        public void AutoRAtkChange()
        {
            PhotoSetGet(PhotoContPL).AutoRAtk = AutoRAtkTo.isOn;
        }
        public void StatusHideChange()
        {
            StatusHide =  StatusHideTo.isOn;
        }
    }
}
