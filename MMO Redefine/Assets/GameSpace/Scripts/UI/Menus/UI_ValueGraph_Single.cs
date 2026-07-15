namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_ValueGraph_Single : MonoBehaviour
    {
        public UI_ValueGraph_Main GraphMain;
        public Image SelUI;
        public TextMeshProUGUI KeyTx;
        public TextMeshProUGUI ValTx;

        public void ActiveSet()
        {
            GraphMain.OnSet(this);
        }
    }
}

