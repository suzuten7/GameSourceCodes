namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_JobTree_Select : MonoBehaviour
    {
        [SerializeField] UI_JobTree_Make JTreeMake;
        public int JobID;
        public TextMeshProUGUI NameTx;
        public Image Flame;
        public void Select()
        {
            JTreeMake.JobID = JobID;
            JTreeMake.TreeUpdate();
        }

    }
}
