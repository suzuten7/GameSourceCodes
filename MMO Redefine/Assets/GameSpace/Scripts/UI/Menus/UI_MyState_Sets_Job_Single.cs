namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_MyState_Sets_Job_Single : MonoBehaviour
    {
        [SerializeField] UI_MyState_Sets_Job_Make MakeUI;
        public int ID;
        public Image BackImage;
        public TextMeshProUGUI NameTx;
        public void Select()
        {
            MakeUI.SelectID = ID;
        }
    }
}
