using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Net_Message : MonoBehaviour,IOnEventCallback
{
    [SerializeField] TMP_InputField TextIn;
    [SerializeField] UI_Sin_Message MessageBase;
    const byte EventID = 1;
    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    private void Start()
    {
        MessageBase.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (TextIn.text.EndsWith("\n")) Net_MessageSend();
    }
    public void Net_MessageSend()
    {
        if (TextIn.text == "") return;
        var raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache,
        };
        PhotonNetwork.RaiseEvent(EventID, PhotonNetwork.NickName + "\n" + TextIn.text, raiseEventOptions, SendOptions.SendReliable);
        TextIn.text = null;
    }
    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == EventID)
        {
           var Message = (string)photonEvent.CustomData;
            var MessCut = Message.Split("\n");
            var MessageUI = Instantiate(MessageBase, MessageBase.transform.parent);
            MessageUI.SendTx.text = MessCut[0];
            MessageUI.MessageTx.text = MessCut[1];
            MessageUI.gameObject.SetActive(true);
        }
    }
}
