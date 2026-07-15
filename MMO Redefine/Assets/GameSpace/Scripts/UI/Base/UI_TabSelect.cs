
namespace UIs
{
    using UnityEngine;
    using UnityEngine.UI;
    using static GmSystem.GS_ChangeSet;
    using static UI_System;
    public class UI_TabSelect : MonoBehaviour
    {
        public int Num;
        [SerializeField]GameObject[] UIs;
        [SerializeField] Image[] SelImages;
        [SerializeField] bool OtherUse = false;
        [SerializeField] Color SelectColor = Color.yellow;
        [SerializeField] Color NonColor = Color.red;
        private void Start()
        {
            UISelect(Num);
        }
        public void UISelect(int ID)
        {
            Num = ID;
            for (int i = 0; i < UIs.Length; i++)
            {
                ChangeActive(UIs[i], i == ID);
            }
            for (int i = 0; i < SelImages.Length; i++)
            {
                if(OtherUse)ChangeColor(SelImages[i],i == ID ? SelectColor : NonColor);
                else ChangeColor(SelImages[i],OnColors(i == ID));
            }
        }
    }
}

