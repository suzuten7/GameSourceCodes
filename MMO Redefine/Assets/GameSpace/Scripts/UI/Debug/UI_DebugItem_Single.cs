namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    public class UI_DebugItem_Single : MonoBehaviour
    {
        [SerializeField] UI_DebugItem_Create Cre;
        public int GID;
        public Image Flame;
        public RawImage Icon;
        public TextMeshProUGUI Name;

        public void Select()
        {
            Cre.SelectGID = GID;
        }
    }
}
