namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_CharaSel_Single : MonoBehaviour
    {
        public UI_CharaSel_Make MakeUI;
        public Image SelImg;
        public int ID;
        public TextMeshProUGUI NameTx;
        public RawImage ModelImg;
        public void Click()
        {
            MakeUI.UIOpen(ID);
        }
        public void Change(bool back)
        {
            MakeUI.Change(ID, back);
        }
    }
}
