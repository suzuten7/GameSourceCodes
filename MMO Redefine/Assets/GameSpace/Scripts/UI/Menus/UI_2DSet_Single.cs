namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_2DSet_Single : MonoBehaviour
    {
        [SerializeField] UI_2DSet_Base loadBase;
        public int ID;
        public TextMeshProUGUI idTx;
        public TMP_Dropdown typeDr;
        public Toggle backTo;
        public RawImage img;
        public TextMeshProUGUI infoTx;
        public void LoadImg()
        {
            loadBase.LoadImage(ID);
        }
        public void Change(bool back)
        {
            loadBase.ChangeData(ID, back);
        }
        public void Rem()
        {
            loadBase.RemData(ID);
        }
        public void TypeChange()
        {
            loadBase.TypeChange(ID, typeDr.value);
        }
        public void BackChange()
        {
            loadBase.BackChange(ID, backTo.isOn);
        }

    }
}

