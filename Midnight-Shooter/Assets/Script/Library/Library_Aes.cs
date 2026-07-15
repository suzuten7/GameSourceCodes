using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Library_Aes : MonoBehaviour
{
    // 初期化ベクトル"<半角16文字（1byte=8bit, 8bit*16=128bit>"
    //private const string AES_IV_256 = @"mER5Ve6jZ/F8CY%~";
    private const string AES_IV_256 = @"midnightshooter~";
    // 暗号化鍵<半角32文字（8bit*32文字=256bit）>
    //private const string AES_Key_256 = @"kxvuA&k|WDRkzgG47yAsuhwFzkQZMNf3";
    private const string AES_Key_256 = @"TomoSuzuSuzuTenKondou12345678900";
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
        try
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
        }
        catch
        {
            return cipher;
        }

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
        try
        {
            byte[] compressedBytes = Convert.FromBase64String(base64);

            using (var input = new MemoryStream(compressedBytes))
            using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
            using (var reader = new StreamReader(deflate, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
        catch
        {
            return base64;
        }
    }
}

