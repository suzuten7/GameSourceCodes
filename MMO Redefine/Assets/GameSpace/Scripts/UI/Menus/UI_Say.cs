namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static GmSystem.GS_ChangeSet;
    using static GmSystem.GS_GlobalValues;
    public class UI_Say : MonoBehaviour
    {
        public TextMeshProUGUI NameTx;
        public TextMeshProUGUI MesTx;
        public RawImage IconIm;
        private void LateUpdate()
        {
            ChangeText(NameTx, SayName);
            ChangeText(MesTx, SayMessage);
            ChangeTexture(IconIm, SayIcon, true);
        }
    }
}

