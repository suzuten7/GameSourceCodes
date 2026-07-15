// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TabViewManager.cs" company="Exit Games GmbH">
// </copyright>
// <summary>
//  Output detailed information about Pun Current states, using the old Unity UI framework.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Output detailed information about Pun Current states, using the old Unity UI framework.
    /// </summary>
    public class StatesGui : MonoBehaviour
    {
        public Rect GuiOffset = new Rect(250, 0, 300, 300);

        /// <summary>Unity GUI Window ID (must be unique or will cause issues).</summary>
        public int WindowId = 100;

        public bool ServerTimestamp;
        public bool DetailedConnection;
        public bool Server;
        public bool AppVersion;
        public bool UserId;
        public bool Room;
        public bool RoomProps;
        public bool EventsIn;
        public bool LocalPlayer;
        public bool PlayerProps;
        public bool Others;
        public bool Buttons;
        public bool ExpectedUsers;


        private Rect GuiRect = new Rect();

        public bool Opens = false;

        void Awake()
        {
            if (EventsIn)
            {
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = true;
            }
        }

        float native_width = 800;
        float native_height = 480;
        void OnGUI()
        {
            this.GuiOffset = GUILayout.Window(this.WindowId, this.GuiOffset, this.TrafficStatsWindow, "StatesGUI");
        }

        private string PlayerToString(Player player)
        {
            if (PhotonNetwork.NetworkingClient == null)
            {
                Debug.LogError("nwp is null");
                return "";
            }
            return string.Format("#{0:00} '{1}'{5} {4}{2} {3} {6}", player.ActorNumber + "/userId:<" + player.UserId + ">", player.NickName, player.IsMasterClient ? "(master)" : "", this.PlayerProps ? player.CustomProperties.ToStringFull() : "", (PhotonNetwork.LocalPlayer.ActorNumber == player.ActorNumber) ? "(you)" : "", player.UserId, player.IsInactive ? " / Is Inactive" : "");
        }
        public void TrafficStatsWindow(int windowID)
        {
            this.Opens = GUILayout.Toggle(this.Opens, "Open");
            GUI.DragWindow();
            if (!this.Opens)
            {
                GuiOffset.height = 50;
                return;
            }

                GUILayout.BeginHorizontal();
            if (this.ServerTimestamp)
            {
                GUILayout.Label((((double)PhotonNetwork.ServerTimestamp) / 1000d).ToString("F3"));
            }

            if (Server)
            {
                GUILayout.Label(PhotonNetwork.ServerAddress + " " + PhotonNetwork.Server);
            }
            if (DetailedConnection)
            {
                GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
            }
            if (AppVersion)
            {
                GUILayout.Label(PhotonNetwork.NetworkingClient.AppVersion);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (UserId)
            {
                GUILayout.Label("UID: " + ((PhotonNetwork.AuthValues != null) ? PhotonNetwork.AuthValues.UserId : "no UserId"));
                GUILayout.Label("UserId:" + PhotonNetwork.LocalPlayer.UserId);
            }
            GUILayout.EndHorizontal();

            if (Room)
            {
                if (PhotonNetwork.InRoom)
                {
                    GUILayout.Label(this.RoomProps ? PhotonNetwork.CurrentRoom.ToStringFull() : PhotonNetwork.CurrentRoom.ToString());
                }
                else
                {
                    GUILayout.Label("not in room");
                }
            }

            if (EventsIn)
            {
                int fragments = PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsIncoming.FragmentCommandCount;
                GUILayout.Label("Events Received: " + PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsGameLevel.EventCount + " Fragments: " + fragments);
            }


            if (this.LocalPlayer)
            {
                GUILayout.Label(PlayerToString(PhotonNetwork.LocalPlayer));
            }
            if (Others)
            {
                foreach (Player player in PhotonNetwork.PlayerListOthers)
                {
                    GUILayout.Label(PlayerToString(player));
                }
            }
            if (ExpectedUsers)
            {
                if (PhotonNetwork.InRoom)
                {
                    int countExpected = (PhotonNetwork.CurrentRoom.ExpectedUsers != null) ? PhotonNetwork.CurrentRoom.ExpectedUsers.Length : 0;

                    GUILayout.Label("Expected: " + countExpected + " " +
                                   ((PhotonNetwork.CurrentRoom.ExpectedUsers != null) ? string.Join(",", PhotonNetwork.CurrentRoom.ExpectedUsers) : "")
                                    );

                }
            }


            if (Buttons)
            {
                if (!PhotonNetwork.IsConnected && GUILayout.Button("Connect"))
                {
                    PhotonNetwork.ConnectUsingSettings();
                }
                GUILayout.BeginHorizontal();
                if (PhotonNetwork.IsConnected && GUILayout.Button("Disconnect"))
                {
                    PhotonNetwork.Disconnect();
                }
                if (PhotonNetwork.IsConnected && GUILayout.Button("Close Socket"))
                {
                    PhotonNetwork.NetworkingClient.LoadBalancingPeer.StopThread();
                }
                GUILayout.EndHorizontal();
                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && GUILayout.Button("Leave"))
                {
                    PhotonNetwork.LeaveRoom();
                }
                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerTtl > 0 && GUILayout.Button("Leave(abandon)"))
                {
                    PhotonNetwork.LeaveRoom(false);
                }
                if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom && GUILayout.Button("Join Random"))
                {
                    PhotonNetwork.JoinRandomRoom();
                }
                if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom && GUILayout.Button("Create Room"))
                {
                    PhotonNetwork.CreateRoom(null);
                }
            }
        }
    }
}