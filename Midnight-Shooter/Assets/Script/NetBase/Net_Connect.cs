using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Net_Connect : MonoBehaviour
{
    static public Net_Connect NetCon;
    static public NetworkRunner InsRunner;
    static public string PlayerName;
    public NetworkSceneManagerDefault sceneManager;
    public NetworkRunner runnerObj;
    public int TitleID;
    public int stageID;
    public TextMeshProUGUI stageTx;
    public RawImage stageImg;

    public TMP_InputField roomIn;
    public TMP_InputField nameIn;
    public GameObject[] titleUIs;
    public GameObject joinsUI;
    public GameObject lerveUI;

    public GameObject onlineUI;



    bool NetStarts = false;
    private void Start()
    {
        if (NetCon != null)
        {
            Destroy(gameObject);
            return;
        }
        NetCon = this;
        DontDestroyOnLoad(gameObject);
        Player_Sets.CheckLoad();
        nameIn.text = Player_Sets.PlayerName;
        LocalizSystem.LocailzLangLoad();
    }
    private void Update()
    {
        var sceneID = SceneManager.GetActiveScene().buildIndex;
        if (InsRunner == null && Obj_LocalObjects.LocalObjects != null) UI_Fade.ui_Fade.ChangeScene(TitleID);
        if(Player_Sets.PlayerName != nameIn.text)
        {
            Player_Sets.PlayerName = nameIn.text;
            Player_Sets.Save();
        }

        PlayerName = Player_Sets.PlayerName;
        for(int i = 0; i < titleUIs.Length; i++)
        {
            if (titleUIs[i]!=null)titleUIs[i].SetActive(sceneID == TitleID);
        }
        joinsUI.SetActive(InsRunner != null && InsRunner.LobbyInfo != null && InsRunner.IsCloudReady && sceneID == TitleID);
        nameIn.gameObject.SetActive(sceneID == TitleID);
        lerveUI.SetActive(InsRunner != null);
        if (sceneID == TitleID)
        {
            Net_Log.Logs.Clear();
            UI_Statistics.AddValue(UI_Statistics.StatEnum.TitleTime, Time.deltaTime);
        }
        else if(InsRunner != null)
        {
            if (InsRunner.GameMode == GameMode.Single) UI_Statistics.AddValue(UI_Statistics.StatEnum.OfflineTime, Time.deltaTime);
            else UI_Statistics.AddValue(UI_Statistics.StatEnum.OnlineTime, Time.deltaTime);
        }
        if (sceneID == TitleID && onlineUI.activeInHierarchy) RunnerSet(true);
        if(stageID >= 0)
        {
            var stageD = Data_Base.DB.stages[stageID];
            stageTx.text = LocalizSystem.LocailzString("StageName", stageD.name);
            stageImg.texture = stageD.icon;
            stageImg.color = stageD.iconCol;
        }
        else
        {
            stageTx.text = LocalizSystem.LocailzSCInfo("ランダムステージ");
            stageImg.color = Color.clear;
        }

    }
    public void Join()
    {
        Play(GameMode.AutoHostOrClient, roomIn.text + "_" + Application.version);
    }
    public void Offline()
    {
        Play(GameMode.Single, "OffLine");
    }
    public int StageSceneIDGet(int sid)
    {
        int playID;
        if (sid >= 0)
        {
            playID = Data_Base.DB.stages[sid].sceneID;
        }
        else
        {
            var stages = Data_Base.DB.stages.ToList();
            for (int i = stages.Count - 1; i >= 0; i--)
            {
                if (stages[i].randomNo) stages.RemoveAt(i);
            }
            playID = stages[Random.Range(0, stages.Count)].sceneID;
        }
        return playID;
    }
    async public void Play(GameMode gm,string sname)
    {
        if (NetStarts) return;
        NetStarts = true;
        RunnerSet(false);
        var playID = StageSceneIDGet(stageID);
        var result = await InsRunner.StartGame(new StartGameArgs
            {
                GameMode = gm,
                SessionName = sname,
                Scene = SceneRef.FromIndex(playID),
                SceneManager = sceneManager,
                PlayerCount = 20 - (gm == GameMode.Server ? 1 : 0),
            });
        if (result.Ok)
        {
            Debug.Log("StartGame 成功");
        }
        else
        {
            Debug.LogError($"StartGame 失敗: {result.ShutdownReason}");

        }
        NetStarts = false;
    }
    async public void Lerve()
    {
        await InsRunner.Shutdown(true);
    }
    async void RunnerSet(bool online)
    {
        if (InsRunner == null)
        {
            InsRunner = Instantiate(runnerObj);
            InsRunner.ProvideInput = true;
            Debug.Log("ランナー生成");
            if (!online) return;
            var result = await InsRunner.JoinSessionLobby(SessionLobby.Shared);
            Debug.Log(result.Ok ? "Lobby Joined" : result.ShutdownReason.ToString());
        }
    }
    static public bool CanControl(NetworkObject obj)
    {
        if (obj == null) return false;
        return obj.HasInputAuthority ||
               (obj.InputAuthority == PlayerRef.None && obj.HasStateAuthority);
    }
    static public bool NoOwnerCheck(NetworkObject obj)
    {
        if (!InsRunner.IsServer) return false;
        if (obj != null && (obj.InputAuthority.PlayerId > 0 && !InsRunner.ActivePlayers.Contains(obj.InputAuthority))) return true;
        return false;

    }
    static public void Despawn(NetworkObject obj)
    {
        NetCon.StartCoroutine(NetCon.WaitDespawn(obj));
    }
    static public float TimeGet
    {
        get
        {
            return InsRunner.Tick * InsRunner.DeltaTime;
        }
    }
    IEnumerator WaitDespawn(NetworkObject obj)
    {
        yield return new WaitForSeconds(5.0f);
        InsRunner.Despawn(obj);
    }
}
