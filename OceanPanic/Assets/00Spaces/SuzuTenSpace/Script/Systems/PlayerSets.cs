using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using static GameInfos;
using TMPro;
using static DataBase;
using static GlovalValues;
using UnityEngine.UI;

public class PlayerSets : MonoBehaviour
{
    #region エディタ変数
    [SerializeField] Transform[] FugitiveStartTra;
    [SerializeField] GameObject FugitiveObj;

    [SerializeField] Transform OniStartTra;
    [SerializeField] GameObject Oni_VisitObj;
    [SerializeField] GameObject Oni_PilotObj;

    [SerializeField] GameObject ViewObj;

    [SerializeField] GameObject OniWaitCam;
    [SerializeField] TextMeshProUGUI OniWaitstx;
    [SerializeField] GameObject OniDummy;
    [SerializeField] Image LoadBlacks;
    [SerializeField] RawImage StayImage;
    [SerializeField] Texture[] StayImages;
    [SerializeField] GameObject BSObj;
    #endregion
    #region 内部変数
    bool SLoad = false;
    bool Sets = false;
    bool OniStay = false;
    int BSTime = 0;
    #endregion
    private void Awake()
    {
        OniWaitCam.SetActive(false);
        BSObj.SetActive(false);
    }

    void FixedUpdate()
    {
        #region 実行チェック
        BSTime++;
        if (!PhotonNetwork.InRoom) return;
        if (Sets) return;
        if (!GInfo.Connets) return;
        BSTime = 0;
        #endregion
        #region 取得
        var CRoom = PhotonNetwork.CurrentRoom;
        var RoomHashs = CRoom.CustomProperties;
        int IOni_Visit = -1;
        int IOni_Pilot = -1;
        if (RoomHashs.TryGetValue("Oni_Visit", out var GOni_Visit)) IOni_Visit = (int)GOni_Visit;
        if (RoomHashs.TryGetValue("Oni_Pilot", out var GOni_Pilot)) IOni_Pilot = (int)GOni_Pilot;
        int Types = -1;
        if (GInfo.STTime > ViewModeTime||PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Views", out var Viewd) && (bool)Viewd) Types = -1;
        else if (PhotonNetwork.LocalPlayer.ActorNumber == IOni_Pilot) Types = 1;
        else if (PhotonNetwork.LocalPlayer.ActorNumber == IOni_Visit) Types = 2;
        else Types = 0;
        if (Types == 2 && RoomHashs.TryGetValue("GameOption1", out var Op1Val) && (bool)Op1Val) Types = 1;
        #endregion
        #region 鬼待機
        bool QStart = false;
        if (Types == 1 && Pass_OniP.TryGetValue((int)Oni_Pilot_PassE.Debug_クイックスタート,out var PQStartLV)&&PQStartLV>0) QStart = true;
        if (Types == 2 && Pass_OniV.TryGetValue((int)Oni_Visit_PassE.Debug_クイックスタート,out var VQStartLV)&&VQStartLV>0) QStart = true;
        OniStay = Types >= 1 && !QStart;
        var LColor = LoadBlacks.color;
        LColor.a = Mathf.Clamp01(LColor.a - 0.01f);
        LoadBlacks.color = LColor;
        StayImage.texture = StayImages[Mathf.Clamp(Types,0,2)];
        if (Types > 0 && GInfo.STTime < OniWaitTime&&!QStart) return;
        if (GInfo.STTime < 300) return;
        #endregion
        #region プレイヤー生成
        Sets = true;
        var FStartPos = FugitiveStartTra[Random.Range(0,FugitiveStartTra.Length-1)];
        switch (Types)
        {
            default:PhotonNetwork.Instantiate(ViewObj.name, FStartPos.position, FStartPos.rotation); break;
            case 0:PhotonNetwork.Instantiate(FugitiveObj.name, FStartPos.position, FStartPos.rotation);break;
            case 1: PhotonNetwork.Instantiate(Oni_PilotObj.name, OniStartTra.position, OniStartTra.rotation); break;
            case 2: PhotonNetwork.Instantiate(Oni_VisitObj.name, OniStartTra.position, OniStartTra.rotation); break;
        }
        
        #endregion
    }
    private void LateUpdate()
    {
        if (!PhotonNetwork.InRoom)return;
        #region 待機UI
        if (!Sets)
        {
            OniWaitCam.SetActive(true);
            if(!OniStay) OniWaitstx.text = "あと" + ((300 + 60 - GInfo.STTime) / 60).ToString("D1") + "秒";
            else OniWaitstx.text = "あと" + ((OniWaitTime + 60 - GInfo.STTime) /60).ToString("D1") + "秒";
            OniDummy.SetActive(false);
        }
        #endregion
        #region 待機用鬼設置
        else
        {
            OniWaitCam.SetActive(false);
            OniDummy.SetActive(GInfo.STTime < OniWaitTime);
        }
        #endregion
        BSObj.SetActive(BSTime >= 600);
        if (!Sets && BSTime >= 900) SceneManager.LoadScene(0);

    }
    private void Update()
    {
        #region 非ルームチェック
        if (BSTime >= 900 && !PhotonNetwork.InRoom && !SLoad)
        {
            SLoad = true;
            SceneManager.LoadScene(0);
        }
        #endregion
    }
}
