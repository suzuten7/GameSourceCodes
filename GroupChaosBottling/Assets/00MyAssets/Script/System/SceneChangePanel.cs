
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DataBase;
using static Statics;
using static PlayerValue;
public class SceneChangePanel : MonoBehaviourPunCallbacks
{
    static public SceneChangePanel SChange;
    [SerializeField] Image Panel;
    [SerializeField] float AlphaSpeed;
    [SerializeField] Transform MvPanel;
    [SerializeField]InfoUIC[] InfoUIs;
    [System.Serializable]
    class InfoUIC
    {
        public Image Frame;
        public RawImage Icon;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Info;
    }
    static int LoadSceneID = -1;
    private void Start()
    {
        if (SChange == null)
        {
            SChange = this;
            DontDestroyOnLoad(gameObject);
            Panel.gameObject.SetActive(true);
            var PCol = Panel.color;
            AlphaSet(1);
            Panel.color = PCol;
        }
        else Destroy(gameObject);
    }
    public override void OnJoinedRoom()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        var CRoomCP = CRoom.CustomProperties;
        int RoomSceneID = CRoomCP.TryGetValue("SceneID", out var oSceneID) ? (int)oSceneID : -1;
        PhotonNetwork.IsMessageQueueRunning = RoomSceneID <= 0;
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        var CRoomCP = CRoom.CustomProperties;
        int RoomSceneID = CRoomCP.TryGetValue("SceneID", out var oSceneID) ? (int)oSceneID : -1;
        PhotonNetwork.IsMessageQueueRunning = RoomSceneID <= 0;
    }
    public override void OnLeftRoom()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
    }
    private void LateUpdate()
    {

        var PCol = Panel.color;
        if (!PhotonNetwork.OfflineMode && PhotonNetwork.InRoom)
        {
            var CRoom = PhotonNetwork.CurrentRoom;
            var CRoomCP = CRoom.CustomProperties;
            int RoomSceneID = CRoomCP.TryGetValue("SceneID", out var oSceneID) ? (int)oSceneID : -1;
            int CSceneID = SceneManager.GetActiveScene().buildIndex;
            bool Changes = RoomSceneID != -1 && RoomSceneID != CSceneID;
            PCol.a = Mathf.Clamp01(PCol.a + AlphaSpeed * 0.01f * (Changes ? 1 : -1));
            if (Changes)
            {
                PhotonNetwork.IsMessageQueueRunning = false;
                if(PCol.a == 1 && RoomSceneID != -1)
                {
                    Save();
                    var CPro = new ExitGames.Client.Photon.Hashtable();
                    CPro["OK"] = false;
                    PhotonNetwork.SetPlayerCustomProperties(CPro);
                    SceneManager.LoadScene(RoomSceneID);
                }
            }
            else
            {
                PhotonNetwork.IsMessageQueueRunning = true;
            }
        }
        else
        {
            PCol.a = Mathf.Clamp01(PCol.a + AlphaSpeed * 0.01f * (LoadSceneID != -1 ? 1 : -1));
            if (PCol.a == 1 && LoadSceneID != -1)
            {
                Save();
                SceneManager.LoadScene(LoadSceneID);
                LoadSceneID = -1;
            }
        }
        AlphaSet(PCol.a);
        Panel.gameObject.SetActive(PCol.a > 0);
        var MvRot = MvPanel.eulerAngles;
        var MvScale = MvPanel.localScale;
        MvRot.z = PCol.a * 360;
        MvRot.x = PCol.a * 360;
        MvRot.y = PCol.a * 360;
        MvScale = PCol.a * Vector3.one;
        MvPanel.eulerAngles = MvRot;
        MvPanel.localScale = MvScale;
        Panel.color = PCol;
    }
    void AlphaSet(float a)
    {
        Panel.color = ColChange(Panel.color, 0, 0, 0, a);
        for(int i = 0; i < InfoUIs.Length; i++)
        {
            InfoUIs[i].Frame.color = ColChange(InfoUIs[i].Frame.color, 0, 0, 0, a);
            InfoUIs[i].Icon.color = ColChange(InfoUIs[i].Icon.color, 0, 0, 0, a);
            InfoUIs[i].Name.color = ColChange(InfoUIs[i].Name.color, 0, 0, 0, a);
            InfoUIs[i].Info.color = ColChange(InfoUIs[i].Info.color, 0, 0, 0, a);
        }

    }
    static public void SceneSet(int Scene)
    {
        if (SChange == null)
        {
            SceneManager.LoadScene(Scene);
        }
        else
        {
            if (LoadSceneID != -1) return;
            LoadSceneID = Scene;
        }
    }
}
