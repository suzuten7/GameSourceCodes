using Photon.Pun;
using TMPro;
using UnityEngine;
using static BattleManager;
using static SoftwareCursorPositionAdjuster;
public class UI_MessageCreate : MonoBehaviour
{
    [SerializeField] TMP_InputField MessageIn;
    [SerializeField] UI_Sin_Message BaseUI;

    void Start()
    {
        BaseUI.gameObject.SetActive(false);
    }
    void Update()
    {
        if (BTManager.End) MouseSets.Chats = true;

        if (BTManager.Messages.Count > 0)
        {
            var InsUI = Instantiate(BaseUI, BaseUI.transform.parent);
            var StrCuts = BTManager.Messages[0].Split("\\");
            InsUI.SendTx.text = StrCuts[0];
            InsUI.MessageTx.text = StrCuts.Length > 1 ? StrCuts[1] : "";
            InsUI.gameObject.SetActive(true);
            InsUI.transform.SetAsLastSibling();
            BTManager.Messages.RemoveAt(0);
        }
    }
    public void MessageSend()
    {
        if (MessageIn.text == "") return;
        BTManager.MessageAdd(PhotonNetwork.NickName + "\\" + MessageIn.text);
        MessageIn.text = "";
    }
    public void ChatInput()
    {
        MouseSets.Chats = !MouseSets.Chats;
        if (!MouseSets.Chats)MessageSend();
    }
}
