using UnityEngine;
using ExitGames.Client.Photon;

public static class Net_MyCustomTypes
{
    public static void Register()
    {
        PhotonPeer.RegisterType(typeof(Color), 1, SerializeColor, DeserializeColor);
        PhotonPeer.RegisterType(typeof(Vector2Int), 2, SerializeVector2Int, DeserializeVector2Int);
        PhotonPeer.RegisterType(typeof(Vector3Int), 3, SerializeVector3Int, DeserializeVector3Int);
    }
    #region Color
    private static readonly byte[] bufferColor = new byte[4];
    private static short SerializeColor(StreamBuffer outStream, object customObject)
    {
        Color32 color = (Color)customObject;
        lock (bufferColor)
        {
            bufferColor[0] = color.r;
            bufferColor[1] = color.g;
            bufferColor[2] = color.b;
            bufferColor[3] = color.a;
            outStream.Write(bufferColor, 0, 4);
        }
        return 4; // 書き込んだバイト数を返す
    }

    // 受信データからバイト列を読み込んでColor型に変換するメソッド
    private static object DeserializeColor(StreamBuffer inStream, short length)
    {
        Color32 color = new Color32();
        lock (bufferColor)
        {
            inStream.Read(bufferColor, 0, 4);
            color.r = bufferColor[0];
            color.g = bufferColor[1];
            color.b = bufferColor[2];
            color.a = bufferColor[3];
        }
        return (Color)color;
    }
    #endregion
    #region Vector2Int
    public static readonly byte[] bufferVector2Int = new byte[8];
    private static short SerializeVector2Int(StreamBuffer outStream, object customObject)
    {
        Vector2Int v = (Vector2Int)customObject;
        int index = 0;
        lock (bufferVector2Int)
        {
            Protocol.Serialize(v.x, bufferVector2Int, ref index);
            Protocol.Serialize(v.y, bufferVector2Int, ref index);
            outStream.Write(bufferVector2Int, 0, index);
        }
        return (short)index;
    }

    private static object DeserializeVector2Int(StreamBuffer inStream, short length)
    {
        int x, y;
        int index = 0;
        lock (bufferVector2Int)
        {
            inStream.Read(bufferVector2Int, 0, length);
            Protocol.Deserialize(out x, bufferVector2Int, ref index);
            Protocol.Deserialize(out y, bufferVector2Int, ref index);
        }
        return new Vector2Int(x, y);
    }
    #endregion
    #region Vector3Int
    public static readonly byte[] bufferVector3Int = new byte[12];
    private static short SerializeVector3Int(StreamBuffer outStream, object customObject)
    {
        Vector3Int v = (Vector3Int)customObject;
        int index = 0;
        lock (bufferVector3Int)
        {
            Protocol.Serialize(v.x, bufferVector3Int, ref index);
            Protocol.Serialize(v.y, bufferVector3Int, ref index);
            Protocol.Serialize(v.z, bufferVector3Int, ref index);
            outStream.Write(bufferVector3Int, 0, index);
        }
        return (short)index;
    }

    private static object DeserializeVector3Int(StreamBuffer inStream, short length)
    {
        int x, y, z;
        int index = 0;
        lock (bufferVector3Int)
        {
            inStream.Read(bufferVector3Int, 0, length);
            Protocol.Deserialize(out x, bufferVector3Int, ref index);
            Protocol.Deserialize(out y, bufferVector3Int, ref index);
            Protocol.Deserialize(out z, bufferVector3Int, ref index);
        }
        return new Vector3Int(x, y, z);
    }
    #endregion
}