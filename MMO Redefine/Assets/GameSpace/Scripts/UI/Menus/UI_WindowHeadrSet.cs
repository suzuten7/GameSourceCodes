namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static GmSystem.GS_ChangeSet;

    public class UI_WindowHeadrSet : MonoBehaviour
    {
        [SerializeField] UI_CharaBase CharaUI;
        [SerializeField] string Title;
        [SerializeField] Texture WinIcon;
        [SerializeField] TextMeshProUGUI TitleTx;
        [SerializeField] RawImage WindowIcon;
        [SerializeField] RawImage CharaIcon;
        private void Start()
        {
            if (Title == "") Title = TitleTx.text;
            else TitleTx.text = Title;
            if (WinIcon != null) WindowIcon.texture = WinIcon;
            ChangeActive(CharaIcon.gameObject, CharaUI != null);
        }
        private void LateUpdate()
        {
            if (CharaUI == null) return;
            ChangeText(TitleTx, Title + "{" + CharaUI.LChara.Name + "}");
            ChangeTexture(CharaIcon, CharaUI.LChara.PlayerIconGet(out _),true);
        }
    }
}

