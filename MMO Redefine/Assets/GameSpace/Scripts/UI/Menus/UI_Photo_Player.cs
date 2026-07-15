namespace UIs
{
    using Player;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static UI_System;
    public class UI_Photo_Player : MonoBehaviour
    {
        public Player_State PSta;
        public TextMeshProUGUI Name;
        public RawImage Icon;
        public Image SelFlame;
        public void Select ()
        {
            PhotoContPL=PSta;
        }
    }
}

