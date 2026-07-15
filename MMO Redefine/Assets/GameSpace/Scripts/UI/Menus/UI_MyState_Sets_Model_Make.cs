namespace UIs
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static Datas.Data_Get;
    using static GmSystem.GS_VroidDictionary;
    using static GmSystem.GS_ChangeSet;
    using static UIs.UI_System;
    public class UI_MyState_Sets_Model_Make : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        public List<UI_MyState_Sets_Model_Single> Singles;
        public TextMeshProUGUI InfoTx;
        public UI_TabSelect Tabs;
        public TextMeshProUGUI VroidName;
        public RawImage VroidIcon;

        [SerializeField] Slider[] ScaleSliders;
        [SerializeField] TMP_InputField[] ScaleIns;

        void Update()
        {
            ChangeText(InfoTx,DB.Models[CharaUI.LChara.ModelID].Name + "\n" + DB.Models[CharaUI.LChara.ModelID].Info);
            for (int i = 0; i < DB.Models.Length; i++)
            {
                if (i >= Singles.Count) Singles.Add(Instantiate(Singles[0], Singles[0].transform.parent));
                var sui = Singles[i];
                sui.ID = i;
                ChangeColor(sui.Flame,OnColors(i == CharaUI.LChara.ModelID));
                ChangeText(sui.Name, DB.Models[i].Name);
                ChangeTexture(sui.Icon, DB.Models[i].Icon, true);
            }
            Tabs.UISelect(CharaUI.LChara.ModelMode);
            var vroid = VroidGet(CharaUI.LChara.ModelVrm);
            if (vroid != null)
            {
                ChangeText(VroidName, vroid.Value.Item1);
                ChangeTexture(VroidIcon, vroid.Value.Item2, true);
            }
            else
            {
                ChangeText(VroidName, "未読み込み");
                ChangeColor(VroidIcon, Color.clear);
            }

            ChangeValue(ScaleSliders[0], CharaUI.LChara.ScaleVrm);
            ChangeText(ScaleIns[0], CharaUI.LChara.ScaleVrm.ToString(), true);
            ChangeValue(ScaleSliders[1], CharaUI.LChara.Scale2D.x);
            ChangeText(ScaleIns[1], CharaUI.LChara.Scale2D.x.ToString(), true);
            ChangeValue(ScaleSliders[2], CharaUI.LChara.Scale2D.y);
            ChangeText(ScaleIns[2], CharaUI.LChara.Scale2D.y.ToString(), true);
        }
        public void TypeChange(int ID)
        {
            CharaUI.LChara.ModelMode = (byte)ID;
        }
        public void VroidSelect()
        {
            UI_Vroid_Set.VroidSelects(CharaUI);
        }

        public void ScaleSliderSet(int i)
        {
            var val = Mathf.RoundToInt(ScaleSliders[i].value);
            switch (i)
            {
                case 0: CharaUI.LChara.ScaleVrm = val;break;
                case 1: CharaUI.LChara.Scale2D.x = val; break;
                case 2: CharaUI.LChara.Scale2D.y = val; break;
            }
        }
        public void ScaleInSet(int i)
        {
            if (!int.TryParse(ScaleIns[i].text, out var val)) return;
            switch (i)
            {
                case 0: CharaUI.LChara.ScaleVrm = val; break;
                case 1: CharaUI.LChara.Scale2D.x = val; break;
                case 2: CharaUI.LChara.Scale2D.y = val; break;
            }
        }
    }
}
