namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    public class UI_Craft_Adds_Single : MonoBehaviour
    {
        [SerializeField]UI_Craft_Adds_Main AddsMain;
        public int ID;
        public TextMeshProUGUI NumTx;
        public TextMeshProUGUI OpTx;
        public TextMeshProUGUI StaTx;
        public TextMeshProUGUI LvTx;
        public TextMeshProUGUI ValTx;
        public TextMeshProUGUI CostTx;
        public Toggle LockTo;
        public void LvAdd()
        {
            AddsMain.LvAdd(ID);
        }
        public void LockSet()
        {
            AddsMain.LockSet(ID,LockTo.isOn);
        }
    }
}
