
namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_ChangeSet;
    public class UI_Option_Base : MonoBehaviour
    {
        public Slider MaxFPSSlider;
        public TMP_InputField MaxFPSIn;
        public TMP_Dropdown QualityDr;
        public Toggle LongCamUseTo;
        public Slider[] UISizeSlider;
        public TMP_InputField[] UISizeIn;

        public Slider[] DSizeSlider;
        public TMP_InputField[] DSizeIn;

        public Slider MvCameraSpeedSlider;
        public TMP_InputField MvCameraSpeedIn;
        public Slider TgCameraSpeedSlider;
        public TMP_InputField TgCameraSpeedIn;

        public Slider[] PosSlider;
        public TMP_InputField[] PosIn;

        void Update()
        {
            var Option = GetSave_Option;
            ChangeValue(MaxFPSSlider, Option.MaxFPS);
            ChangeText(MaxFPSIn,Option.MaxFPS.ToString(),true);
            ChangeValue(QualityDr,Option.QualityLV);
            ChangeOn(LongCamUseTo, Option.LongCamUse);
            Debug.Log("UISize" + Option.UISizes.Count);
            for(int i = 0; i < UISizeSlider.Length; i++)
            {
                ChangeValue(UISizeSlider[i], Option.UISizes[i]);
                ChangeText(UISizeIn[i], Option.UISizes[i].ToString(), true);
            }


            for (int i = 0; i < DSizeSlider.Length; i++)
            {
                ChangeValue(DSizeSlider[i], Option.DSizes[i]);
                ChangeText(DSizeIn[i],Option.DSizes[i].ToString(), true);
            }
            ChangeValue(MvCameraSpeedSlider, Option.Cam_MvSpeed);
            ChangeText(MvCameraSpeedIn, Option.Cam_MvSpeed.ToString(),true);
            ChangeValue(TgCameraSpeedSlider, Option.Cam_TgSpeed);
            ChangeText(TgCameraSpeedIn, Option.Cam_TgSpeed.ToString(), true);

            ChangeValue(PosSlider[0], Option.Cam_Pos.x);
            ChangeText(PosIn[0], Option.Cam_Pos.x.ToString(), true);
            ChangeValue(PosSlider[1], Option.Cam_Pos.y);
            ChangeText(PosIn[1], Option.Cam_Pos.y.ToString(), true);
            ChangeValue(PosSlider[2], Option.Cam_Pos.z);
            ChangeText(PosIn[2], (Option.Cam_Pos.z * 0.01f).ToString("F2"), true);
        }
        public void MaxFPSSliderSet()
        {
            GetSave_Option.MaxFPS = Mathf.RoundToInt(MaxFPSSlider.value);
        }
        public void MaxFPSInSet()
        {
            if(int.TryParse(MaxFPSIn.text, out var ov))GetSave_Option.MaxFPS = ov;
        }

        public void QualitySet()
        {
            GetSave_Option.QualityLV = QualityDr.value;
        }
        public void LongCamSet()
        {
            GetSave_Option.LongCamUse = LongCamUseTo.isOn;
        }
        public void MvCamSpeedSliderSet()
        {
            GetSave_Option.Cam_MvSpeed = Mathf.RoundToInt(MvCameraSpeedSlider.value);
        }
        public void MvCamSpeedInSet()
        {
            if (int.TryParse(MvCameraSpeedIn.text, out var ov)) GetSave_Option.Cam_MvSpeed = ov;
        }
        public void TgCamSpeedSliderSet()
        {
            GetSave_Option.Cam_TgSpeed = Mathf.RoundToInt(TgCameraSpeedSlider.value);
        }
        public void TgCamSpeedInSet()
        {
            if (int.TryParse(TgCameraSpeedIn.text, out var ov)) GetSave_Option.Cam_TgSpeed = ov;
        }
        public void CamPosSliderSet(int Index)
        {
            var val = Mathf.RoundToInt(PosSlider[Index].value);
            switch (Index)
            {
                case 0: GetSave_Option.Cam_Pos.x = val; break;
                case 1: GetSave_Option.Cam_Pos.y = val; break;
                case 2: GetSave_Option.Cam_Pos.z = val; break;
            }
        }
        public void CamPosInSet(int Index)
        {
            var ov = 0;
            if (Index == 2 || int.TryParse(PosIn[Index].text, out ov))
            switch (Index)
            {
                case 0: GetSave_Option.Cam_Pos.x = ov; break;
                case 1: GetSave_Option.Cam_Pos.y = ov; break;
                case 2: if (float.TryParse(PosIn[Index].text, out var ovf)) GetSave_Option.Cam_Pos.z = Mathf.RoundToInt(ovf * 100); break;
            }
        }

        public void UISizeSliderSet(int id)
        {
            GetSave_Option.UISizes[id] = Mathf.RoundToInt(UISizeSlider[id].value);
        }
        public void UISizeInSet(int id)
        {
            if (int.TryParse(UISizeIn[id].text, out var oval)) GetSave_Option.UISizes[id] = oval;
        }
        public void DSizeSliderSet(int id)
        {
            GetSave_Option.DSizes[id] = Mathf.RoundToInt(DSizeSlider[id].value);
        }
        public void DSizeInSet(int id)
        {
            if(int.TryParse(DSizeIn[id].text, out var ov)) GetSave_Option.DSizes[id] = ov;
        }
    }
}
