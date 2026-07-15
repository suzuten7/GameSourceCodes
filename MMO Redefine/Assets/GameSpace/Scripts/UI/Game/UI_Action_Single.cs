namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Action_Single : MonoBehaviour
    {
        [SerializeField] UI_Actions ActionsUI;
        public int Index;
        public TextMeshProUGUI NameTx;
        public Image FlameImage;

        public void Sets()
        {
            ActionsUI.PlayAction(Index);
        }
    }
}
