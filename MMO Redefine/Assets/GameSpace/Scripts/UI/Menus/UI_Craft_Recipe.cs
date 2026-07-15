namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    public class UI_Craft_Recipe : MonoBehaviour
    {
        [SerializeField] UI_Craft_Make CraftMake;
        public int ID;
        public Image OutImage;
        public TextMeshProUGUI NameTx;
        public RawImage Icon;
        public TextMeshProUGUI NoIconName;
        public void Select()
        {
            CraftMake.CraftID = ID;
        }
    }
}
