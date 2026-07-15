
namespace FNet
{
    using UnityEngine;
    using Fusion;
    using UnityEngine.SceneManagement;
    using System.Linq;
    using static Fusion_Manager;
    using static Fusion_NetValue;
    using static GmSystem.GS_GlobalValues;
    using static Datas.Data_Get;
    using static GmSystem.GS_GlobalState;
    public class Fusion_RoomGUI : MonoBehaviour
    {
        public enum Enum_GUIID
        {
            PhotonFusionルーム情報 = 9999,
            メッセージ,
            プレイヤー移動,
            プレイヤーステータス,
            プレイヤーセット,
            プレイヤーショートカット,
            プレイヤーバフ,
            プレイヤーインベントリ,
            プレイヤーアクション,
        }
        [SerializeField] string TitleName;
        Rect _guiRect = new Rect(0, 0, 200, 290);
        Vector2 _guiScrolle;
        private void OnGUI()
        {
            if (InsRunner.Mode != SimulationModes.Server) return;
            if (!InsRunner.IsServer) return;
            _guiRect = GUI.Window((int)Enum_GUIID.PhotonFusionルーム情報, _guiRect, WindowGUI, "Room");
        }
        void WindowGUI(int windowID)
        {
            var runner = InsRunner;
            GUILayout.Label("ルーム名:" + runner.SessionInfo.Name);
            GUILayout.Space(-6);
            var modeHostStr = "モード:" + (SelectMode != GameMode.Single ? runner.Mode.ToString() : "Offline");
            modeHostStr += ",ホスト:" + runner.IsServer;
            GUILayout.Label(modeHostStr);
            GUILayout.Space(-6);
            var serverTime = FixServerTime();
            var serverSec = serverTime / 60;
            string timeStr = serverTime.ToString() + "(";
            timeStr += (serverSec / 3600).ToString("D2");
            timeStr += ":" + ((serverSec / 60) % 60).ToString("D2");
            timeStr += ":" + (serverSec % 60).ToString("D2") + ")";
            GUILayout.Label("時間:" + timeStr);
            GUILayout.Space(-6);
            GUILayout.Label("人数:" + runner.ActivePlayers.Count() + "/" + runner.SessionInfo.MaxPlayers);
            GUILayout.Space(-6);
            _guiScrolle = GUILayout.BeginScrollView(_guiScrolle, false, true, GUILayout.Width(180), GUILayout.Height(70));
            var playersStr = "";
            foreach (var pl in PStaList)
            {
                if (pl == null) continue;
                if (playersStr != "") playersStr += "\n";
                var teamStr = TeamGet_Str((int)pl.CommonValues.Team, out var TeamCol);
                teamStr = "<color=#" + ColorUtility.ToHtmlStringRGB(TeamCol) + ">" + teamStr + "</color>";
                playersStr += teamStr +":" + pl.CommonValues.Name;
                playersStr += "\nLV" + pl.CommonValues.LV + "(";
                for(int i = 0; i < pl.PlayerValues.Jobs.Length; i++)
                {
                    if (i > 0) playersStr += "×";
                    playersStr += DB.JobDatas[pl.PlayerValues.Jobs[i].ID].Name;
                }
                playersStr += ")";
            }
            GUILayout.Label(playersStr);
            GUILayout.EndScrollView();
            if (NetValue != null)
            {
                GUILayout.Label("世界難易度");
                var difSet = GUILayout.Toolbar(NetValue.Dific, Strs_Dific);
                if (runner.IsServer) NetValue.Dific = difSet;
                var difStr = "HP" + (DificChange(0, Enum_DifcVal.HP) * 100).ToString("F0") + "%,";
                difStr += "St" + (DificChange(0, Enum_DifcVal.Atk) * 100).ToString("F0") + "%,";
                difStr += "EXP" + (DificChange(0, Enum_DifcVal.EXP) * 100).ToString("F0") + "%";
                GUILayout.Label(difStr);
            }

            if (GUILayout.Button("退室"))
            {
                ServerExit();
            }
            GUI.DragWindow();
        }
    }
}
