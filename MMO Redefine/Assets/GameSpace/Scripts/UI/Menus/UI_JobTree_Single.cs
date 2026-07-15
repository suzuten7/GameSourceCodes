namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    public class UI_JobTree_Single : MonoBehaviour
    {
        public UI_JobTree_Make JTreeMake;

        public int GID;
        public int SID;
        public Image BackImage;
        public RawImage Icon;
        public TextMeshProUGUI NameTx;
        public TextMeshProUGUI LvTx;

        public void GetTree()
        {
            JTreeMake.GetTree(GID, SID);
        }
    }
}
