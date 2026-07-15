using TMPro;
using UnityEngine;

public class UI_StringTest : MonoBehaviour
{
    [SerializeField] TMP_InputField InTxs;
    [SerializeField] TextMeshProUGUI InCountTx;
    [SerializeField] TMP_InputField OutTxs;
    [SerializeField] TextMeshProUGUI OutCountTx;
    [SerializeField] TextMeshProUGUI LogTx;

    public void Updates()
    {
        InCountTx.text = InTxs.text.Length + "文字";
        OutCountTx.text = OutTxs.text.Length + "文字";
    }
    public void SChange()
    {
        var InStr = InTxs.text;
        InTxs.text = OutTxs.text;
        OutTxs.text = InStr;
        Updates();
    }
    public void KeyCutSet()
    {
        try
        {
            OutTxs.text = AesExample.JsonKeyCutSet(InTxs.text);
            LogTx.text = "キー削減化成功";
        }
        catch
        {
            LogTx.text = "失敗しました";
        }
        Updates();
    }
    public void KeyCutRev()
    {
        try
        {
            OutTxs.text = AesExample.JsonKeyCutRev(InTxs.text);
            LogTx.text = "キー削減復元成功";
        }
        catch
        {
            LogTx.text = "失敗しました";
        }
        Updates();
    }
    public void B64Set()
    {
        try
        {
            OutTxs.text = AesExample.CompressToBase64(InTxs.text);
            LogTx.text = "Base64化成功";
        }
        catch
        {
            LogTx.text = "失敗しました";
        }
        Updates();
    }
    public void B64Rev()
    {
        try
        {
            OutTxs.text = AesExample.DecompressFromBase64(InTxs.text);
            LogTx.text = "Base64復元成功";
        }
        catch
        {
            LogTx.text = "失敗しました";
        }
        Updates();
    }
    public void AesSet()
    {
        try
        {
            OutTxs.text = AesExample.Encrypt(InTxs.text);
            LogTx.text = "Aes化成功";
        }
        catch
        {
            LogTx.text = "失敗しました";
        }
        Updates();
    }
    public void AesRev()
    {
        try
        {
            OutTxs.text = AesExample.Decrypt(InTxs.text);
            LogTx.text = "Aes復元成功";
        }
        catch
        {
            LogTx.text = "失敗しました";
        }
        Updates();
    }
    public void Copy(bool Out)
    {
        if(!Out)GUIUtility.systemCopyBuffer = InTxs.text;
        else GUIUtility.systemCopyBuffer = OutTxs.text;
        Updates();
    }
    public void Past(bool Out)
    {
        if (!Out) InTxs.text = GUIUtility.systemCopyBuffer;
        else OutTxs.text = GUIUtility.systemCopyBuffer;
        Updates();
    }
}
