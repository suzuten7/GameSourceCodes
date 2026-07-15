using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;  //AESを使うために追加
using System.IO.Compression;
using System.Collections.Generic;
using Unity.Collections;
using static Manifesto;
using System.Text.RegularExpressions;

public static class AesExample
{
    // 初期化ベクトル"<半角16文字（1byte=8bit, 8bit*16=128bit>"
    //private const string AES_IV_256 = @"mER5Ve6jZ/F8CY%~";
    private const string AES_IV_256 = @"defrostcomitgcb~";
    // 暗号化鍵<半角32文字（8bit*32文字=256bit）>
    //private const string AES_Key_256 = @"kxvuA&k|WDRkzgG47yAsuhwFzkQZMNf3";
    private const string AES_Key_256 = @"GroupChaosBotting_MultiplayGame3";

    /// <summary>
    /// 対称鍵暗号を使って文字列を暗号化する
    /// </summary>
    /// <param name="text">暗号化する文字列</param>
    /// <param name="iv">対称アルゴリズムの初期ベクター</param>
    /// <param name="key">対称アルゴリズムの共有鍵</param>
    /// <returns>暗号化された文字列</returns>
    public static string Encrypt(string text)
    {
        RijndaelManaged myRijndael = new RijndaelManaged();
        // ブロックサイズ（何文字単位で処理するか）
        myRijndael.BlockSize = 128;
        // 暗号化方式はAES-256を採用
        myRijndael.KeySize = 256;
        // 暗号利用モード
        myRijndael.Mode = CipherMode.CBC;
        // パディング
        myRijndael.Padding = PaddingMode.PKCS7;

        myRijndael.IV = Encoding.UTF8.GetBytes(AES_IV_256);
        myRijndael.Key = Encoding.UTF8.GetBytes(AES_Key_256);

        // 暗号化
        ICryptoTransform encryptor = myRijndael.CreateEncryptor(myRijndael.Key, myRijndael.IV);

        byte[] encrypted;
        using (MemoryStream mStream = new MemoryStream())
        {
            using (CryptoStream ctStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter sw = new StreamWriter(ctStream))
                {
                    sw.Write(text);
                }//using
                encrypted = mStream.ToArray();
            }//using
        }//using
        // Base64形式（64種類の英数字で表現）で返す
        return (System.Convert.ToBase64String(encrypted));
    }//Encrypt

    /// <summary>
    /// 対称鍵暗号を使って暗号文を復号する
    /// </summary>
    /// <param name="cipher">暗号化された文字列</param>
    /// <param name="iv">対称アルゴリズムの初期ベクター</param>
    /// <param name="key">対称アルゴリズムの共有鍵</param>
    /// <returns>復号された文字列</returns>
    public static string Decrypt(string cipher)
    {
        RijndaelManaged rijndael = new RijndaelManaged();
        // ブロックサイズ（何文字単位で処理するか）
        rijndael.BlockSize = 128;
        // 暗号化方式はAES-256を採用
        rijndael.KeySize = 256;
        // 暗号利用モード
        rijndael.Mode = CipherMode.CBC;
        // パディング
        rijndael.Padding = PaddingMode.PKCS7;

        rijndael.IV = Encoding.UTF8.GetBytes(AES_IV_256);
        rijndael.Key = Encoding.UTF8.GetBytes(AES_Key_256);

        ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

        string plain = string.Empty;
        using (MemoryStream mStream = new MemoryStream(System.Convert.FromBase64String(cipher)))
        {
            using (CryptoStream ctStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader sr = new StreamReader(ctStream))
                {
                    plain = sr.ReadLine();
                }//using
            }//using
        }//using
        return plain;
    }//Decrypt

    /// <summary>JSON=>Base64</summary>
    public static string CompressToBase64(string json)
    {
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (var output = new MemoryStream())
        {
            // Deflate圧縮ストリーム
            using (var deflate = new DeflateStream(output, System.IO.Compression.CompressionLevel.Optimal, true))
            {
                deflate.Write(jsonBytes, 0, jsonBytes.Length);
            }

            byte[] compressedBytes = output.ToArray();

            // Base64エンコードして返す
            return Convert.ToBase64String(compressedBytes);
        }
    }

    /// <summary>Base64文字列から元のJSON文字列に復元</summary>
    public static string DecompressFromBase64(string base64)
    {
        byte[] compressedBytes = Convert.FromBase64String(base64);

        using (var input = new MemoryStream(compressedBytes))
        using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
        using (var reader = new StreamReader(deflate, Encoding.UTF8))
        {
            return reader.ReadToEnd();
        }
    }

    [ReadOnly]
    static string[] JsonChangeTable =
{
        //
        "false","#",
        "true","$",
        //
        "PSaves","PSa",
        "MasterVol","MVo",
        "BGMVol","BVo",
        "SEVol","SVo",
        "SystemVol","YVo",
        "QualityLV","QLV",
        "CamDistance","CDs",
        "CamSpeed","Csd",
        "TargetSpeed","Tsd",
        "DamTime","Dtm",
        "DamStack","Dst",
        "AtkUISize","AUS",
        "MoveStickSize","MSS",
        "JumpDashUISize","JUS",

        "PriSets","PSt",
        "PriSetID","PSD",
        "AddSet1","AS1",
        "AddSet2","AS2",
        "AddSet3","AS3",

        //
        "CharaID", "CD",
        "AtkF","AF",
        "AtkB","AB",
        "N_AtkID", "ND",
        "S1_AtkID", "S1D",
        "S2_AtkID", "S2D",
        "E_AtkID", "ED",
        "Passive", "Pas",
        "P1_ID","P1D",
        "P2_ID","P2D",
        "P3_ID","P3D",
        "P4_ID","P4D",

        "Disp","Dp",
        "Memo","Mm",

        "AddAI_F", "AIF",
        "AddAI_B", "AIB",

        "AIMode","AIM",
        "PLTarget","PLT",
        "PLDis","PDS",
        "Range","Ra",

        "ChangePer","CPr",
        "ChangeTime","CTi",

        "Jump","Jp",
        "Dash","Ds",
        "NAtk","NA",
        "S1Atk","S1A",
        "S2Atk","S2A",
        "EAtk","EA",

        "PerBase","PBa",
        "PerOuR","POu",
        "PerPLD","PPL",
        "TimeIn","TIn",
        "TimeStay","TSy",
        "Stays","Sys",
        "TimeWait","TWa",

        "Stages","Stg",
        "SoloStars","Sor",
        "MultStars","Mur",
        "Genes","Gen",
        "Sets","Set",
        "G1_ID","G1D",
        "G2_ID","G2D",
        "G3_ID","G3D",
        "G4_ID","G4D",
        "G5_ID","G5D",
        "Datas","Dat",
        "Type","Tye",
        "Format","For",
        "Name","Ne",
        "Lock","Lk",
        "LV","Lv",
        "Main","Min",
        "Sub1","Sb1",
        "Sub2","Sb2",
        "Sub3","Sb3",
        "Add1","Ad1",
        "Add2","Ad2",
        "Add3","Ad3",

        "Twitter",
    };
    static public string JsonKeyCutSet(string JsonStr)
    {
        for (int i = 1; i < JsonChangeTable.Length; i += 2)
        {
            bool Name = true;
            if (JsonChangeTable[i - 1] == "false") Name = false;
            if (JsonChangeTable[i - 1] == "true") Name = false;
            var BaseStr = JsonChangeTable[i - 1];
            if (Name) BaseStr = "\"" + BaseStr + "\":";
            JsonStr = JsonStr.Replace(BaseStr, JsonChangeTable[i] + "@");
        }
        JsonStr = ConvertAtNumbersToBase64(JsonStr);
        return JsonStr;
    }
    static public string JsonKeyCutRev(string KeyCutStr)
    {
                KeyCutStr = ConvertAtBase64ToNumbers(KeyCutStr);
        for (int i = 1; i < JsonChangeTable.Length; i += 2)
        {
            bool Name = true;
            if (JsonChangeTable[i - 1] == "false") Name = false;
            if (JsonChangeTable[i - 1] == "true") Name = false;
            var BaseStr = JsonChangeTable[i - 1];
            if (Name) BaseStr = "\"" + BaseStr + "\":";
            KeyCutStr = KeyCutStr.Replace(JsonChangeTable[i] + "@", BaseStr);

        }
        return KeyCutStr;
    }

    const string base64Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz&_";

    public static string ConvertAtNumbersToBase64(string input)
    {
        return Regex.Replace(input, @"@(\d+)", match =>
        {
            int number = int.Parse(match.Groups[1].Value);
            return "@" + ToBase64(number);
        });
    }
    private static string ToBase64(int number)
    {

        if (number == 0) return "0";

        string result = "";
        while (number > 0)
        {
            int remainder = number % 64;
            result = base64Chars[remainder] + result;
            number /= 64;
        }
        return result;
    }

    public static string ConvertAtBase64ToNumbers(string input)
    {
        return Regex.Replace(input, @"@([0-9A-Za-z&_]+)", match =>
        {
            string base64 = match.Groups[1].Value;
            int number = FromBase64(base64);
            return "@" + number.ToString();
        });
    }
    private static int FromBase64(string base64)
    {
        int result = 0;

        foreach (char c in base64)
        {
            int value = base64Chars.IndexOf(c);
            if (value < 0)
                throw new ArgumentException($"Invalid base64 character: {c}");

            result = result * 64 + value;
        }

        return result;
    }

}