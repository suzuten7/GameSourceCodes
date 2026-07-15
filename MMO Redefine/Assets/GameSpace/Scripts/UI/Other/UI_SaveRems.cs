namespace UIs
{
    using TMPro;
    using UnityEngine;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_ChangeSet;
    public class UI_SaveRems : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI LVTx;
        [SerializeField] string DelKey;
        [SerializeField] TMP_InputField DelIn;
        private void LateUpdate()
        {
            var PlayFSec = GetSave_State.PlayTimes / 60;
            var PlayHour = PlayFSec / 3600;
            var PlayMin = PlayFSec / 60 % 60;
            var PlaySec = PlayFSec % 60;
            var PlayStr = PlayHour.ToString("D2") + ":" + PlayMin.ToString("D2") + ":" + PlaySec.ToString("D2");
            ChangeText(LVTx,
                "プレイ時間" + PlayStr + "\n" + GetSave_State.PlayTimes + "f\nLV:" + GetSave_State.LV + "(EXP:" + GetSave_State.EXP + ")");
        }
        public void Delete()
        {
            if(DelIn.text == DelKey)
            {
                SaveClear();
                DelIn.text = "セーブ削除完了";
            }
            else
            {
                DelIn.text = "入力が違います";
            }
        }
    }
}
