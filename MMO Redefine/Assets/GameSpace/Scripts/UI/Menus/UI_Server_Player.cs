
namespace UIs
{
    using TMPro;
    using UnityEngine;
    using Player;
    using static FNet.Fusion_Manager;
    using UnityEngine.UI;

    public class UI_Server_Player : MonoBehaviour
    {
        public Player_State PSta;
        public Image BackUI;
        public TextMeshProUGUI PlayerID;
        public RawImage Icon;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Job;
        public GameObject KickButton;
        public void Kick()
        {
            if (PSta.BotID >= 0)
            {
                Despawn(PSta.Object);
                PSta.gameObject.SetActive(false);
            }
            else InsRunner.Disconnect(PSta.Object.InputAuthority);
        }
    }
}
