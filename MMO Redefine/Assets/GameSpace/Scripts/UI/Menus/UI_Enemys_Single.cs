namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Enemys_Single : MonoBehaviour
    {
        [SerializeField] UI_Enemys_Make Make;
        public int ID;
        public Image SelIm;
        public TextMeshProUGUI Name;
        public RawImage Icon;
        public void Sel()
        {
            Make.ID = ID;
        }
    }
}

