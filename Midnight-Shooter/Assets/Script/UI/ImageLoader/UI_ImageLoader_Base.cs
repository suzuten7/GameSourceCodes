using SFB;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine.UI;
using TMPro;
public class UI_ImageLoader_Base : MonoBehaviour
{
    [System.Serializable]
    public class ImgBase
    {
        public string name = "";
        public List<ImgData> datas = new();
        public List<float> speeds = new();
        public Texture IconGet
        {
            get
            {
                if (datas.Count > 0)
                {
                    return datas[0].TextureGet;
                }
                else return null;
            }
        }
    }
    [System.Serializable]
    public class ImgData
    {
        public int type;
        public bool dot;
        public string data = "";
        string datab = "";
        Texture2D texture = null;
        public Texture2D TextureGet
        {
            get
            {
                if (datab == data) return texture;
                datab = data;
                try
                {
                    byte[] bytes = Convert.FromBase64String(data);
                    texture = new Texture2D(2, 2);
                    texture.LoadImage(bytes);
                    texture.filterMode = dot ? FilterMode.Point : FilterMode.Bilinear;
                    return texture;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
    static List<ImgBase> imgSets = new ();
    static public List<ImgBase> ImgSets
    {
        get
        {
            if (imgSets.Count <= 0) Load();
            return imgSets;
        }
    }

    [SerializeField] TMP_InputField nameIn;
    [SerializeField] List<UI_ImageLoader_Sets> SetUIs;
    [SerializeField] TMP_Dropdown typeDr;
    [SerializeField] TMP_InputField speedIn;
    [SerializeField] TMP_Dropdown sizeDr;
    [SerializeField] Toggle squareTo;
    [SerializeField] Toggle smoothTo;
    [SerializeField] List<UI_ImageLoader_Img> ImgUIs;
    [SerializeField] GameObject IOUI;
    [SerializeField] TMP_InputField IOTextIn;
    public int selID = 0;
    private void Start()
    {
        IOUI.gameObject.SetActive(false);
    }
    private void Update()
    {
        for (int i = 0; i < Mathf.Max(SetUIs.Count, ImgSets.Count); i++)
        {
            if (i >= ImgSets.Count)
            {
                SetUIs[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= SetUIs.Count)
            {
                SetUIs.Add(Instantiate(SetUIs[0], SetUIs[0].transform.parent));
            }
            var sui = SetUIs[i];
            var set = ImgSets[i];
            sui.gameObject.SetActive(true);
            sui.ID = i;
            sui.backImage.color = i == selID ? Color.yellow : Color.white;
            sui.nameTx.text = $"{LocalizSystem.LocailzSCInfo("外見")}{(i + 1)}:{set.name}";
            sui.img.texture = set.IconGet;
        }
        var csel = ImgSets[selID];
        if (!nameIn.isFocused) nameIn.text = csel.name;
        if (typeDr.value > 0)
        {
            for (int i = csel.speeds.Count; i <= (int)Player_ImgAnims.AnimType.Death; i++) csel.speeds.Add(1);
            if (!speedIn.isFocused) speedIn.text = csel.speeds[typeDr.value - 1].ToString();
            speedIn.interactable = true;
        }
        else
        {
            speedIn.text = LocalizSystem.LocailzSCInfo("全表示不可");
            speedIn.interactable = false;
        }
        for (int i = 0; i < Mathf.Max(ImgUIs.Count, csel.datas.Count); i++)
        {
            if (i >= ImgSets[selID].datas.Count)
            {
                ImgUIs[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= ImgUIs.Count)
            {
                ImgUIs.Add(Instantiate(ImgUIs[0], ImgUIs[0].transform.parent));
            }
            var sui = ImgUIs[i];
            var set = ImgSets[selID].datas[i];
            if (typeDr.value > 0 && set.type != typeDr.value - 1)
            {
                sui.gameObject.SetActive(false);
                continue;
            }
            sui.gameObject.SetActive(true);
            sui.ID = i;
            sui.idTx.text = (i + 1).ToString();
            sui.typeDr.value = set.type;
            var img = set.TextureGet;
            sui.img.texture = img;
            if (img != null) sui.infoTx.text = $"{img.width}×{img.height}({LocalizSystem.LocailzSCInfo(set.dot ? "ドット" : "なめらか")})";
            else sui.infoTx.text = LocalizSystem.LocailzSCInfo("未読み込み");
        }
    }
    public void LoadImage(int s)
    {
        switch (Application.platform)
        {
            default:
                var paths = StandaloneFileBrowser.OpenFilePanel(LocalizSystem.LocailzSCInfo("画像を選択"), "", "*", false);
                if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
                {
                    LoadPath(s,paths[0]);
                }
                break;
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                if (!NativeFilePicker.CheckPermission())
                {
                    NativeFilePicker.RequestPermissionAsync();
                }
                NativeFilePicker.PickFile((paths) => { LoadPath(s, paths); });
                break;
        }
    }
    void LoadPath(int s,string path)
    {
        var set = ImgSets[selID].datas[s];
        byte[] bytes = File.ReadAllBytes(path);
        var tx = new Texture2D(2, 2);
        tx.LoadImage(bytes);
        var size = 16;
        switch (sizeDr.value)
        {
            case 1: size = 32; break;
            case 2: size = 64; break;
            case 3: size = 128; break;
            case 4: size = 256; break;
            case 5: size = 512; break;
            case 6: size = 0; break;
        }
        var tx2 = ProcessTexture(tx, size,squareTo.isOn,smoothTo.isOn);
        var tx2bytes = tx2.EncodeToPNG();
        set.data = Convert.ToBase64String(tx2bytes);
        set.dot = !smoothTo.isOn;
        Save();
    }
    public static Texture2D ProcessTexture(Texture2D src, int size, bool square, bool smooth)
    {
        if (src == null) return null;

        int srcW = src.width;
        int srcH = src.height;

        float scale = 1f;

        // size基準リサイズ（0なら元サイズ）
        if (size > 0)
        {
            float maxSide = Mathf.Max(srcW, srcH);
            scale = size / maxSide;
        }

        int newW = Mathf.Max(1, Mathf.RoundToInt(srcW * scale));
        int newH = Mathf.Max(1, Mathf.RoundToInt(srcH * scale));

        // 補間設定（ここが追加ポイント）
        Texture2D temp = new Texture2D(2, 2);
        temp.filterMode = smooth ? FilterMode.Bilinear : FilterMode.Point;

        RenderTexture rt = RenderTexture.GetTemporary(newW, newH);
        rt.filterMode = temp.filterMode;

        RenderTexture.active = rt;
        Graphics.Blit(src, rt);

        Texture2D resized = new Texture2D(newW, newH, TextureFormat.RGBA32, false);
        resized.filterMode = temp.filterMode;
        resized.ReadPixels(new Rect(0, 0, newW, newH), 0, 0);
        resized.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        // square不要なら終了
        if (!square || size <= 0)
            return resized;

        int canvas = size;
        Texture2D result = new Texture2D(canvas, canvas, TextureFormat.RGBA32, false);
        result.filterMode = temp.filterMode;

        Color[] clear = new Color[canvas * canvas];
        for (int i = 0; i < clear.Length; i++) clear[i] = Color.clear;
        result.SetPixels(clear);

        int offsetX = (canvas - newW) / 2;
        int offsetY = (canvas - newH) / 2;

        result.SetPixels(offsetX, offsetY, newW, newH, resized.GetPixels());
        result.Apply();

        return result;
    }

    public void ImageAdd()
    {
        ImgSets.Add(new());
        Save();
    }
    public void ImageDataAdd()
    {
        ImgSets[selID].datas.Add(new ImgData{type = Mathf.Max(0, typeDr.value - 1)});
        Save();
    }
    public void NameSet()
    {
        ImgSets[selID].name = nameIn.text;
        Save();
    }
    public void SpeedSet()
    {
        if (typeDr.value <= 0) return;
        if (float.TryParse(speedIn.text, out var val))
            ImgSets[selID].speeds[typeDr.value - 1] = val;
    }
    public void ChangeSet(int s,bool back)
    {
        var ci = Mathf.Clamp(s + (!back ? 1 : -1), 0, ImgSets.Count - 1);
        var bset = ImgSets[ci];
        ImgSets[ci] = ImgSets[s];
        ImgSets[s] = bset;
        Save();
    }
    public void ChangeData(int s,bool back)
    {
        var sets = ImgSets[selID];
        var ci = s;
        if (back)
        {
            for (int i = s - 1; i >= 0; i--)
            {
                var d = sets.datas[i];
                if (typeDr.value != 0 && d.type != typeDr.value - 1) continue;
                ci = i;
                break;
            }
        }
        else
        {
            for (int i = s + 1; i < sets.datas.Count; i++)
            {
                var d = sets.datas[i];
                if (typeDr.value != 0 && d.type != typeDr.value - 1) continue;
                ci = i;
                break;
            }
        }
        var dc = sets.datas[ci];
        sets.datas[ci] = sets.datas[s];
        sets.datas[s] = dc;
        Save();
    }
    public void RemData(int s)
    {
        ImgSets[selID].datas.RemoveAt(s);
        Save();
    }
    public void TypeChange(int s,int type)
    {
        var sets = ImgSets[selID];
        sets.datas[s].type = type;
        Save();
    }

    public void Save()
    {
        Library_SaveFiles.SaveFile("ImgSets", "ImgCount", ImgSets.Count.ToString());
        Library_SaveFiles.SaveFile("ImgSets", "Img_"+ selID,JsonUtility.ToJson(ImgSets[selID]));
    }
    static void Load()
    {
        var countVal = Library_SaveFiles.LoadFileInt("ImgSets", "ImgCount", 0);
        imgSets.Clear();
        for (int i = 0; i < countVal; i++)
        {
            var setsStr = Library_SaveFiles.LoadFileStr("ImgSets", "Img_"+ i);
            var sets = JsonUtility.FromJson<ImgBase>(setsStr);
            if(sets != null)imgSets.Add(sets);
        }
        if (imgSets.Count <= 0) imgSets.Add(new());
    }

    public void Inport()
    {
        if (IOTextIn.text == "") return;
        var json = Library_Aes.DecompressFromBase64(IOTextIn.text);
        var imgset = JsonUtility.FromJson<ImgBase>(json);
        ImgSets[selID] = imgset;
        Save();
    }
    public void Export()
    {
        var json = JsonUtility.ToJson(ImgSets[selID]);
        var b64 = Library_Aes.CompressToBase64(json);
        IOTextIn.text = b64;
    }
}
