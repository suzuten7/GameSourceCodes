namespace UIs
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using static UI_System;
    using static Player.Player_Controle;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_ChangeSet;
    public class UI_Actions : MonoBehaviour
    {
        [SerializeField] GameObject ScrollObj;
        [SerializeField] List<UI_Action_Single> ActionSingles;
        int _selIndex = 0;
        void LateUpdate()
        {
            if (MyPlayer == null) return;
            ActionObjs.RemoveAll(x => x == null);
            var AObjs = ActionObjs;
            ChangeActive(ScrollObj, AObjs.Count > 0);
            if (PCont.In_AcUp) _selIndex--;
            if (PCont.In_AcDown) _selIndex++;
            _selIndex = (int)Mathf.Repeat(_selIndex, Mathf.Max(1, AObjs.Count));
            for (int i = 0; i < Mathf.Max(AObjs.Count, ActionSingles.Count); i++)
            {
                if (ActionSingles.Count <= i) ActionSingles.Add(Instantiate(ActionSingles[0], ActionSingles[0].transform.parent));
                var asi = ActionSingles[i];
                if (i >= AObjs.Count || AObjs[i] == null || AObjs[i].NoDisp)
                {
                    ChangeActive(asi.gameObject, false);
                    continue;
                }
                ChangeActive(asi.gameObject, true);
                asi.Index = i;
                ChangeText(asi.NameTx, AObjs[i].ActionName);
                ChangeColor(asi.FlameImage,OnColors(_selIndex == i));
            }
            if (PCont.In_AcPlay) PlayAction(_selIndex);

            PCont.In_AcUp = false;
            PCont.In_AcDown = false;
            PCont.In_AcPlay = false;
        }
        public void PlayAction(int Index)
        {
            if (ActionObjs.Count <= 0) return;
            ActionObjs[Index].PlayAction(MyPlayer);
        }
    }
}
