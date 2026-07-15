
namespace FNet
{
    using Datas;
    using Fusion;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using static GmSystem.GS_GlobalValues;
    using static GmSystem.GS_SaveValues;
    using static Datas.Data_Get;
    using static GmSystem.GS_ChangeSet;
    public class Fusion_Manager : MonoBehaviour
    {
        [SerializeField] NetworkSceneManagerDefault sceneManager;
        [SerializeField] NetworkRunner runnerPrefab;
        [SerializeField] GameObject NetValueObj;
        [SerializeField] int TitleScene;
        [SerializeField] int PlayScene;
        [SerializeField] CanvasGroup LoadFades;
        [SerializeField] UIs.UI_System UISys;
        [SerializeField] GameObject ServerUI;
        [SerializeField] Data_Base DBs;
        public GameObject playerPrefab;
        public bool NetStarts;

        static public Fusion_Manager FMananger;
        static public NetworkRunner InsRunner;
        static public Fusion_NetValue NetValue;
        static public GameMode SelectMode;
        static public string RoomName;


        private void Start()
        {
            if (FMananger != null) Destroy(gameObject);
            else
            {
                DontDestroyOnLoad(gameObject);
                FMananger = this;
                DBSet(DBs);
                NetStarts = false;
                LoadFades.gameObject.SetActive(true);
            }
        }
        private void Update()
        {
            QualitySettings.SetQualityLevel(GetSave_Option.QualityLV);
            Application.targetFrameRate = GetSave_Option.MaxFPS;
            var cSceneID = SceneManager.GetActiveScene().buildIndex;
            Debug.Log("シーンID:" + cSceneID);
            if (TitleScene >= 0 && cSceneID != TitleScene && InsRunner == null && LoadFades.alpha >= 1)
            {
                Fusion_Chat.Messages.Clear();
                SceneManager.LoadScene(TitleScene);
            }

            ChangeActive(UISys.gameObject,cSceneID == PlayScene&& InsRunner != null && InsRunner.GameMode != GameMode.Server);
            ChangeActive(ServerUI.gameObject, cSceneID == PlayScene && InsRunner != null && InsRunner.GameMode == GameMode.Server);

            if (PlayScene >= 0 && cSceneID == PlayScene && NetValue == null)
            {
                InsRunner.TrySpawn(NetValueObj, out var obj);
            }
        }
        private void LateUpdate()
        {
            var cSceneID = SceneManager.GetActiveScene().buildIndex;
            if (cSceneID == TitleScene)
            {
                if (FMananger.NetStarts || (InsRunner != null && InsRunner.IsRunning))
                {
                    LoadFades.blocksRaycasts = true;
                    LoadFades.alpha = Mathf.Clamp01(LoadFades.alpha + 0.06f);
                }
                else
                {
                    LoadFades.blocksRaycasts = false;
                    LoadFades.alpha = Mathf.Clamp01(LoadFades.alpha - 0.03f);
                }
            }
            else
            {
                if (InsRunner == null)
                {
                    LoadFades.blocksRaycasts = true;
                    LoadFades.alpha = Mathf.Clamp01(LoadFades.alpha + 0.06f);
                }
                else
                {
                    LoadFades.blocksRaycasts = false;
                    LoadFades.alpha = Mathf.Clamp01(LoadFades.alpha - 0.03f);
                }
            }
        }
        static public int FixServerTime()
        {
            return Mathf.RoundToInt(InsRunner.Tick * InsRunner.DeltaTime * 60);
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
            if (obj == null || !InsRunner.ActivePlayers.Contains(obj.InputAuthority)) return true;
            return false;

        }
        static public void Despawn(NetworkObject obj)
        {
            FMananger.StartCoroutine(FMananger.WaitDespawn(obj));
        }
        IEnumerator WaitDespawn(NetworkObject obj)
        {
            yield return new WaitForSeconds(5.0f);
            InsRunner.Despawn(obj);
        }
        static public void NetStart(GameMode mode)
        {
            if (InsRunner != null) return;
            SelectMode = mode;
            FMananger.NetConnect();
        }
        async void NetConnect()
        {
            if (NetStarts) return;
            NetStarts = true;
            var runner = Instantiate(runnerPrefab);
            InsRunner = runner;
            Debug.Log("ランナー生成");
            runner.ProvideInput = true;
            var result = await runner.StartGame(new StartGameArgs
            {
                GameMode = SelectMode,
                SessionName = RoomName,
                Scene = SceneRef.FromIndex(PlayScene),
                SceneManager = sceneManager,
                PlayerCount = 20 - (SelectMode == GameMode.Server ? 1 : 0),
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
        public static void ServerExit()
        {
            if (MyPlayer != null) MyPlayer.SaveSet();
            if (InsRunner == null) SceneManager.LoadScene(FMananger.TitleScene);
            else
            {
                InsRunner.Shutdown(true);
            }
        }
    }
}
