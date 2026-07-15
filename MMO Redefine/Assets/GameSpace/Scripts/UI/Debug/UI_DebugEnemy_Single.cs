namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_DebugEnemy_Single : MonoBehaviour
    {
        [SerializeField] UI_DebugEnemy_Create Cre;
        public int ID;
        public Image SelUI;
        public TextMeshProUGUI NameTx;
        public RawImage Icon;
        public void Select()
        {
            Cre.ID = ID;
        }
    }
}

