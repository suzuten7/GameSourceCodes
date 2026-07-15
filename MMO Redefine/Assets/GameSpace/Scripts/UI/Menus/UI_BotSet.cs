namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static GmSystem.GS_ChangeSet;
    public class UI_BotSet : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        [SerializeField] Slider[] Sliders;
        [SerializeField] TMP_InputField[] Inputs;
        [SerializeField] TMP_Dropdown ShortCutUseDr;
        private void LateUpdate()
        {
            var BOP = CharaUI.LChara.BotOption;

            ChangeValue(Sliders[0],BOP.MoveRange.x);
            ChangeText(Inputs[0],(BOP.MoveRange.x * 0.1f).ToString("F1"),true);
            ChangeValue(Sliders[1],BOP.MoveRange.y);
            ChangeText(Inputs[1],(BOP.MoveRange.y * 0.1f).ToString("F1"),true);

            ChangeValue(Sliders[2],BOP.AtkModeCenterSec);
            ChangeText(Inputs[2],(BOP.AtkModeCenterSec * 0.1f).ToString("F1"),true);
            ChangeValue(Sliders[3],BOP.RandModeMoveSec);
            ChangeText(Inputs[3], (BOP.RandModeMoveSec * 0.1f).ToString("F1"), true);
            ChangeValue(Sliders[4],BOP.RandModeTargetSec);
            ChangeText(Inputs[4], (BOP.RandModeTargetSec * 0.1f).ToString("F1"), true);

            ChangeValue(Sliders[5],BOP.AtkFBTrySec);
            ChangeText(Inputs[5],(BOP.AtkFBTrySec * 0.1f).ToString("F1"), true);
            ChangeValue(Sliders[6],BOP.AtkFBChance);
            ChangeText(Inputs[6],(BOP.AtkFBChance * 0.1f).ToString("F1"), true);

            ChangeValue(Sliders[7], BOP.ShortCutTrySec);
            ChangeText(Inputs[7], (BOP.ShortCutTrySec * 0.1f).ToString("F1"), true);
            ChangeValue(Sliders[8],BOP.ShortCutChance);
            ChangeText(Inputs[8], (BOP.ShortCutChance * 0.1f).ToString("F1"), true);

            ChangeValue(ShortCutUseDr, BOP.ShortCutUse);
        }
        public void SliderSet(int ID)
        {
            var BOP = CharaUI.LChara.BotOption;
            var val = Mathf.RoundToInt(Sliders[ID].value);
            switch (ID)
            {
                case 0: BOP.MoveRange.x = val; break;
                case 1: BOP.MoveRange.y = val; break;

                case 2: BOP.AtkModeCenterSec = val; break;
                case 3: BOP.RandModeMoveSec = val; break;
                case 4: BOP.RandModeTargetSec = val; break;

                case 5: BOP.AtkFBTrySec = val; break;
                case 6: BOP.AtkFBChance = val; break;
                case 7: BOP.ShortCutTrySec = val; break;
                case 8: BOP.ShortCutChance = val; break;

            }
        }
        public void InputSet(int ID)
        {
            var BOP = CharaUI.LChara.BotOption;
            if (!float.TryParse(Inputs[ID].text, out var ov)) return;
            var val = Mathf.RoundToInt(ov * 10);
            switch (ID)
            {
                case 0: BOP.MoveRange.x = val; break;
                case 1: BOP.MoveRange.y = val; break;

                case 2: BOP.AtkModeCenterSec = val; break;
                case 3: BOP.RandModeMoveSec = val; break;
                case 4: BOP.RandModeTargetSec = val; break;

                case 5: BOP.AtkFBTrySec = val; break;
                case 6: BOP.AtkFBChance = val; break;
                case 7: BOP.ShortCutTrySec = val; break;
                case 8: BOP.ShortCutChance = val; break;
            }
        }
        public void ShortCutUseSet()
        {
            var BOP = CharaUI.LChara.BotOption;
            BOP.ShortCutUse = ShortCutUseDr.value;
        }
    }
}

