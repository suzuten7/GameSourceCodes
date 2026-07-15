namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static GmSystem.GS_SaveValues;
    public class UI_Acive_Single : MonoBehaviour
    {
        public string ID;
        public Image OutIm;
        public TextMeshProUGUI Name;
        public RawImage Icon;
        public void AciveRestart()
        {
            AciveSet(ID, 2);
        }
    }
}
