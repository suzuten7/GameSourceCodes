
namespace FNet
{
    using Fusion;
    using UnityEngine;
    using UnityEngine.UI;
    using static Fusion_Manager;
    using static GmSystem.GS_SaveValues;
    using TMPro;
    using static GmSystem.GS_ChangeSet;

    public class Fusion_Title : MonoBehaviour
    {
        [SerializeField] TMP_InputField NameIn;
        [SerializeField] TMP_InputField RoomIn;
        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            PlayerName = PlayerPrefs.GetString("Name_" + SaveNameGet, "無名" + Random.Range(1000, 10000));
            NameSet();
            RoomSet();
        }
        public void NameSet()
        {
            if (NameIn.text != "")
            {
                PlayerName = NameIn.text;
                PlayerPrefs.SetString("Name_" + SaveNameGet, PlayerName);
            }
            else
            {
                ChangeText(NameIn, PlayerName,true);
            }
        }
        public void RoomSet()
        {
            if (RoomIn.text != "")
            {
                RoomName = RoomIn.text;
            }
            else ChangeText(RoomIn,RoomName,true);
        }

        public void StartOffline()
        {
            NetStart(GameMode.Single);
        }
        public void StartOnline()
        {
            NetStart(GameMode.AutoHostOrClient);
        }

        public void StartAsServer()
        {
            NetStart(GameMode.Server);
        }

        public void GameExits()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
