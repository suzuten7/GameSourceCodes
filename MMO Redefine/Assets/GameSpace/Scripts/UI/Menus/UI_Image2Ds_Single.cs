namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Image2Ds_Single : MonoBehaviour
    {
        [SerializeField] UI_Image2Ds_Make MakeUI;
        public int ID;
        public TextMeshProUGUI NameTx;
        public Image SelUI;
        public RawImage Images;
        public string bdata = "";
        public void Select()
        {
            MakeUI.Select(ID);
        }
        public void Change(bool back)
        {
            MakeUI.ChangeSet(ID, back);
        }
    }
}

