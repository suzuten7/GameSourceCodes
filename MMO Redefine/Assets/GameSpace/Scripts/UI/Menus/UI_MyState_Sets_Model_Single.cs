namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static UI_System;
    public class UI_MyState_Sets_Model_Single : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        public int ID;
        public Image Flame;
        public TextMeshProUGUI Name;
        public RawImage Icon;

        public void Select()
        {
            CharaUI.LChara.ModelID = ID;
        }
    }
}
