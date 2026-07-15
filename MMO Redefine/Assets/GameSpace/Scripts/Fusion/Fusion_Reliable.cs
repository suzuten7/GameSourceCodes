
namespace FNet
{
    using Fusion;
    using Fusion.Sockets;
    using State;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using static Datas.Data_Get;
    using static Fusion_Manager;

    public class Fusion_Reliable : MonoBehaviour, INetworkRunnerCallbacks
    {
        const int Int_SIZE = 4;
        const int Float_SIZE = 4;
        const int V3_SIZE = 12;
        const int Col32_SIZE = 4;
        const int S64_SIZE = 64;
        const int S256_SIZE = 256;

        public enum Enum_TypeID
        {
            Message,
            Damage,
            EXPDrop,
            TextDisp,
        }
        public enum Enum_MesID
        {
            System,
            Death,
            Chat,
        }
        public class Class_MessageData
        {
            public Enum_MesID Type;
            public string Base;
            public string Add;
        }

        static public void ChatMessage(Enum_MesID mesID, string name, string message)
        {
            var runner = InsRunner;
            runner.SendReliableDataToServer(new ReliableKey(), Message_Pack((byte)mesID, FixServerTime(), name, message));
        }
        static byte[] Message_Pack(byte id,int time, string name, string message)
        {
            byte[] data = new byte[1 + 1 + Int_SIZE + S64_SIZE + S256_SIZE];

            int offset = 0;

            // Type（1バイト）
            data[offset++] = (byte)Enum_TypeID.Message;

            data[offset++] = id;
            // ID（4バイト）
            byte[] timeBytes = BitConverter.GetBytes(time);
            Array.Copy(timeBytes, 0, data, offset, Int_SIZE);
            offset += Int_SIZE;

            // UTF8エンコード
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            int nameLength = Math.Min(nameBytes.Length, S64_SIZE);
            int messageLength = Math.Min(messageBytes.Length, S256_SIZE);

            // 名前
            Array.Copy(nameBytes, 0, data, offset, nameLength);
            offset += S64_SIZE;

            // メッセージ
            Array.Copy(messageBytes, 0, data, offset, messageLength);

            return data;
        }
        static void Message_Unpack(byte[] data, out byte id,out int time, out string name, out string message)
        {
            if (data.Length < 1 + 1 + Int_SIZE + S64_SIZE + S256_SIZE)
                throw new ArgumentException("データ長が不足しています");

            int offset = 1;
            id = data[offset++];
            // ID（4バイト）
            time = BitConverter.ToInt32(data, offset);
            offset += Int_SIZE;

            // 名前
            int nameLength = 0;
            for (; nameLength < S64_SIZE; nameLength++)
            {
                if (data[offset + nameLength] == 0) break;
            }
            name = Encoding.UTF8.GetString(data, offset, nameLength);
            offset += S64_SIZE;

            // メッセージ
            int messageLength = 0;
            for (; messageLength < S256_SIZE; messageLength++)
            {
                if (data[offset + messageLength] == 0) break;
            }
            message = Encoding.UTF8.GetString(data, offset, messageLength);
        }

        static public void DamageDisp(uint hnetID , uint anetID, Vector3 pos, float val, bool crit,byte element, byte regHit, byte regEle)
        {
            var runner = InsRunner;
            runner.SendReliableDataToServer(new ReliableKey(), Damage_Pack(hnetID, anetID, pos, val, crit, element, regHit, regEle));
        }
        static byte[] Damage_Pack(uint hnetID, uint anetID, Vector3 pos, float val,bool crit,byte element, byte regHit, byte regEle)
        {
            byte[] data = new byte[1 + Int_SIZE + Int_SIZE + V3_SIZE + Float_SIZE + 1 + 1 + 1 + 1];
            int offset = 0;

            // type (1バイト)
            data[offset++] = (byte)Enum_TypeID.Damage;

            BitConverter.GetBytes(hnetID).CopyTo(data, offset); offset += Int_SIZE;
            BitConverter.GetBytes(anetID).CopyTo(data, offset); offset += Int_SIZE;
            // Vector3 (12バイト)
            BitConverter.GetBytes(pos.x).CopyTo(data, offset); offset += Float_SIZE;
            BitConverter.GetBytes(pos.y).CopyTo(data, offset); offset += Float_SIZE;
            BitConverter.GetBytes(pos.z).CopyTo(data, offset); offset += Float_SIZE;

            // float (4バイト)
            BitConverter.GetBytes(val).CopyTo(data, offset); offset += Float_SIZE;
            //
            BitConverter.GetBytes(crit).CopyTo(data, offset); offset += 1;

            data[offset++] = element;
            data[offset++] = regHit;
            data[offset++] = regEle;

            return data;
        }
        static void Dam_Unpack(byte[] data,out uint hnetID, out uint anetID, out Vector3 pos, out float val,out bool crit,out byte element,out byte regHit,out byte regEle)
        {
            if (data.Length < 1 + Int_SIZE + Int_SIZE + V3_SIZE + Float_SIZE + 1 + 1 + 1 + 1)
                throw new ArgumentException("データ長が不足しています");

            int offset = 1;
            hnetID = BitConverter.ToUInt32(data, offset); offset += Int_SIZE;
            anetID = BitConverter.ToUInt32(data, offset); offset += Int_SIZE;
            // Vector3
            float x = BitConverter.ToSingle(data, offset); offset += Float_SIZE;
            float y = BitConverter.ToSingle(data, offset); offset += Float_SIZE;
            float z = BitConverter.ToSingle(data, offset); offset += Float_SIZE;
            pos = new Vector3(x, y, z);
            // int
            val = BitConverter.ToSingle(data, offset); offset += Float_SIZE;
            //bool
            crit = BitConverter.ToBoolean(data, offset);offset++;

            element = data[offset++];
            regHit = data[offset++];
            regEle = data[offset++];
        }

        static public void EXPDrop(Vector3 pos,byte count, int val,List<PlayerRef> TPls)
        {
            var runner = InsRunner;
            var pack = EXP_Pack(pos, count, val);
            foreach (var player in TPls)
            {
                runner.SendReliableDataToPlayer(player, new ReliableKey(), pack);
            }
        }
        static byte[] EXP_Pack(Vector3 pos,byte count, int val)
        {
            byte[] data = new byte[1 + V3_SIZE + 1 + Int_SIZE];
            int offset = 0;

            // type (1バイト)
            data[offset++] = (byte)Enum_TypeID.EXPDrop;

            // Vector3 (12バイト)
            BitConverter.GetBytes(pos.x).CopyTo(data, offset); offset += Float_SIZE;
            BitConverter.GetBytes(pos.y).CopyTo(data, offset); offset += Float_SIZE;
            BitConverter.GetBytes(pos.z).CopyTo(data, offset); offset += Float_SIZE;
            data[offset++] = count;
            // int (4バイト)
            BitConverter.GetBytes(val).CopyTo(data, offset);

            return data;
        }
        static void EXP_Unpack(byte[] data, out Vector3 pos,out byte count, out int val)
        {
            if (data.Length < 1 + V3_SIZE + Int_SIZE)
                throw new ArgumentException("データ長が不足しています");

            int offset = 1;

            // Vector3
            float x = BitConverter.ToSingle(data, offset); offset += Float_SIZE;
            float y = BitConverter.ToSingle(data, offset); offset += Float_SIZE;
            float z = BitConverter.ToSingle(data, offset); offset += Float_SIZE;
            pos = new Vector3(x, y, z);
            count = data[offset++];
            // int
            val = BitConverter.ToInt32(data, offset);
        }

        static public void TextDisp(uint hnetID, uint anetID, Vector3 pos, string tex, Color colIn, Color colOut)
        {
            var runner = InsRunner;
            runner.SendReliableDataToServer(new ReliableKey(), Text_Pack(hnetID, anetID, pos, tex, colIn, colOut));
        }
        static byte[] Text_Pack(uint hnetID, uint anetID, Vector3 pos, string tex, Color32 colIn, Color32 colOut)
        {
            byte[] data = new byte[1 + Int_SIZE + Int_SIZE + V3_SIZE + S64_SIZE + Col32_SIZE + Col32_SIZE];
            int offset = 0;

            // type (1バイト)
            data[offset++] = (byte)Enum_TypeID.TextDisp;
            BitConverter.GetBytes(hnetID).CopyTo(data, offset); offset += Int_SIZE;
            BitConverter.GetBytes(anetID).CopyTo(data, offset); offset += Int_SIZE;
            // Vector3 (12バイト)
            BitConverter.GetBytes(pos.x).CopyTo(data, offset); offset += Float_SIZE;
            BitConverter.GetBytes(pos.y).CopyTo(data, offset); offset += Float_SIZE;
            BitConverter.GetBytes(pos.z).CopyTo(data, offset); offset += Float_SIZE;

            // UTF8エンコード
            byte[] texBytes = Encoding.UTF8.GetBytes(tex);
            int texLength = Math.Min(texBytes.Length, S64_SIZE);
            //
            Array.Copy(texBytes, 0, data, offset, texLength);
            offset += S64_SIZE;
            // Color32 (4バイト)
            data[offset++] = colIn.r;
            data[offset++] = colIn.g;
            data[offset++] = colIn.b;
            data[offset++] = colIn.a;

            data[offset++] = colOut.r;
            data[offset++] = colOut.g;
            data[offset++] = colOut.b;
            data[offset++] = colOut.a;
            return data;
        }
        static void Text_Unpack(byte[] data,out uint hnetID, out uint anetID, out Vector3 pos, out string tex, out Color32 colIn, out Color32 colOut)
        {
            if (data.Length < 1 + Int_SIZE + Int_SIZE + V3_SIZE + Float_SIZE + 1 + Col32_SIZE + Col32_SIZE)
                throw new ArgumentException("データ長が不足しています");

            int offset = 1;
            hnetID = BitConverter.ToUInt32(data, offset); offset += Int_SIZE;
            anetID = BitConverter.ToUInt32(data, offset); offset += Int_SIZE;
            // Vector3
            float x = BitConverter.ToSingle(data, offset); offset += Float_SIZE;
            float y = BitConverter.ToSingle(data, offset); offset += Float_SIZE;
            float z = BitConverter.ToSingle(data, offset); offset += Float_SIZE;
            pos = new Vector3(x, y, z);
            // tex
            int texLength = 0;
            for (; texLength < S64_SIZE; texLength++)
            {
                if (data[offset + texLength] == 0) break;
            }
            tex = Encoding.UTF8.GetString(data, offset, texLength);
            offset += S64_SIZE;
            // Color32
            colIn = Color.white;
            colIn.r = data[offset++];
            colIn.g = data[offset++];
            colIn.b = data[offset++];
            colIn.a = data[offset++];
            // Color32
            colOut = Color.white;
            colOut.r = data[offset++];
            colOut.g = data[offset++];
            colOut.b = data[offset++];
            colOut.a = data[offset++];
        }


        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
        {
            //Debug.Log($"[Fusion] 信頼性付きデータを受信（新）: {player}, キー: {key}, サイズ: {data.Count}バイト,Type{data[0]}");
            switch ((Enum_TypeID)data[0])
            {
                case Enum_TypeID.Message:
                    if (runner.IsServer)
                    {
                        foreach (var p in runner.ActivePlayers)
                        {
                            if (runner.LocalPlayer == p) continue;
                            runner.SendReliableDataToPlayer(p, new ReliableKey(), data.Array);
                        }
                    }
                    Message_Unpack(data.Array, out var mes_ID, out var mes_Time, out var mes_name, out var mes_message);
                    Fusion_Chat.LocalMessage((Enum_MesID)mes_ID, mes_name , mes_message, mes_Time);
                    break;
                case Enum_TypeID.Damage:
                    if (runner.IsServer)
                    {
                        foreach (var p in runner.ActivePlayers)
                        {
                            if (runner.LocalPlayer == p) continue;
                            runner.SendReliableDataToPlayer(p, new ReliableKey(), data.Array);
                        }
                    }
                    Dam_Unpack(data.Array,out var dam_hnetID, out var dam_anetID, out var dam_Pos, out var dam_Val, out var dam_Crit, out var dam_ColElement,out var dam_RegHit,out var dam_RegEle);
                    dam_Pos.x += UnityEngine.Random.Range(-1.2f, 1.2f);
                    dam_Pos.y += UnityEngine.Random.Range(-1.2f, 1.2f);
                    dam_Pos.z += UnityEngine.Random.Range(-1.2f, 1.2f);

                    var dam_hnetobj = runner.FindObject(new NetworkId { Raw = dam_hnetID });
                    var dam_htsta = dam_hnetobj != null ? dam_hnetobj.GetComponent<State_StateBase>() : null;

                    var dam_anetobj = runner.FindObject(new NetworkId { Raw = dam_anetID });
                    var dam_atsta = dam_anetobj != null ? dam_anetobj.GetComponent<State_StateBase>() : null;

                    var dobj = Instantiate(DB.DamObj, dam_Pos, Quaternion.identity);
                    dobj.DamSet(dam_htsta,dam_atsta, dam_Val, dam_Crit,DB.ElementColors[dam_ColElement],dam_RegHit,dam_RegEle);
                    break;
                case Enum_TypeID.EXPDrop:
                    EXP_Unpack(data.Array, out var exp_pos, out var exp_count, out var exp_val);
                    for (int i = 0; i < Mathf.Max(1, exp_count); i++)
                    {
                        var expobj = Instantiate(DB.EXPObj, exp_pos, Quaternion.identity);
                        expobj.EXP = Mathf.RoundToInt(exp_val / Mathf.Max(1f, exp_count));
                        var exprig = expobj.GetComponent<Rigidbody>();
                        var vect = new Vector3(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value / 2f, UnityEngine.Random.value - 0.5f);
                        exprig.linearVelocity = vect.normalized * UnityEngine.Random.Range(200f, 1000f) * 0.01f;
                    }
                    break;
                case Enum_TypeID.TextDisp:
                    if (runner.IsServer)
                    {
                        foreach (var p in runner.ActivePlayers)
                        {
                            if (runner.LocalPlayer == p) continue;
                            runner.SendReliableDataToPlayer(p, new ReliableKey(), data.Array);
                        }
                    }
                    Text_Unpack(data.Array,out var tex_hnetID,out var tex_anetID, out var tex_Pos, out var tex_Tex, out var tex_ColTeam, out var tex_ColElement);
                    tex_Pos.x += UnityEngine.Random.Range(-1.2f, 1.2f);
                    tex_Pos.y += UnityEngine.Random.Range(-1.2f, 1.2f);
                    tex_Pos.z += UnityEngine.Random.Range(-1.2f, 1.2f);

                    var tex_hnetobj = runner.FindObject(new NetworkId { Raw = tex_hnetID });
                    var tex_htsta = tex_hnetobj != null ? tex_hnetobj.GetComponent<State_StateBase>() : null;

                    var tex_anetobj = runner.FindObject(new NetworkId { Raw = tex_anetID });
                    var tex_atsta = tex_anetobj != null ? tex_anetobj.GetComponent<State_StateBase>() : null;

                    var tobj = Instantiate(DB.DamObj, tex_Pos, Quaternion.identity);
                    tobj.TextSet(tex_htsta,tex_atsta,100,tex_Tex, tex_ColTeam, tex_ColElement);
                    break;
            }

        }

        #region 未使用コールバック
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }
        public void OnConnectedToServer(NetworkRunner runner)
        {
        }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }
        //public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data){}
        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }
        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }
        #endregion

    }
}
