namespace UIs
{
    using SFB;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static GmSystem.GS_SaveValues;
    using static GmSystem.GS_ChangeSet;

    public class UI_2DSet_Base : MonoBehaviour
    {
        [SerializeField] TMP_InputField nameIn;
        [SerializeField] TMP_Dropdown typeDr;
        [SerializeField] Slider speedSlider;
        [SerializeField] TMP_InputField speedIn;
        [SerializeField] TMP_Dropdown sizeDr;
        [SerializeField] Toggle squareTo;
        [SerializeField] Toggle smoothTo;
        [SerializeField] List<UI_2DSet_Single> ImgUIs;
        [SerializeField] RectTransform AddUI;
        [SerializeField] GameObject IOUI;
        [SerializeField] TMP_InputField IOTextIn;
        public int selID = 0;
        private void Start()
        {
            IOUI.gameObject.SetActive(false);
        }
        private void Update()
        {
            var csel = GetSave_2DImages[selID];
            if (!nameIn.isFocused) nameIn.text = csel.name;
            if (typeDr.value > 0)
            {
                for (int i = csel.speeds.Count; i <= 15; i++) csel.speeds.Add(1);
                var spd = csel.speeds[typeDr.value - 1];
                speedSlider.interactable = true;
                speedSlider.value = spd;
                if (!speedIn.isFocused) speedIn.text = spd.ToString("F2");
                speedIn.interactable = true;
            }
            else
            {
                speedSlider.interactable = false;
                speedIn.text = "全表示不可";
                speedIn.interactable = false;
            }
            for (int i = 0; i < Mathf.Max(ImgUIs.Count, csel.datas.Count); i++)
            {
                if (i >= csel.datas.Count)
                {
                    ImgUIs[i].gameObject.SetActive(false);
                    continue;
                }
                if (i >= ImgUIs.Count)
                {
                    ImgUIs.Add(Instantiate(ImgUIs[0], ImgUIs[0].transform.parent));
                    AddUI.SetAsLastSibling();
                }
                var sui = ImgUIs[i];
                var set = csel.datas[i];
                if (typeDr.value > 0 && set.type != typeDr.value - 1)
                {
                    sui.gameObject.SetActive(false);
                    continue;
                }
                sui.gameObject.SetActive(true);
                sui.ID = i;
                sui.idTx.text = (i + 1).ToString();
                sui.typeDr.value = set.type;
                sui.backTo.isOn = set.back;
                var img = set.TextureGet;
                sui.img.texture = img;
                if (img != null) sui.infoTx.text = $"{img.width}×{img.height}({(set.dot ? "ドット" : "なめらか")})";
                else sui.infoTx.text = ("未読み込み");
            }
        }
        public void LoadImage(int s)
        {
            switch (Application.platform)
            {
                default:
                    var paths = StandaloneFileBrowser.OpenFilePanel(("画像を選択"), "", "*", false);
                    if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
                    {
                        LoadPath(s, paths[0]);
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
        void LoadPath(int s, string path)
        {
            var set = GetSave_2DImages[selID].datas[s];
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
            var tx2 = ProcessTexture(tx, size, squareTo.isOn, smoothTo.isOn);
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
            GetSave_2DImages.Add(new());
            Save();
        }
        public void ImageDataAdd()
        {
            GetSave_2DImages[selID].datas.Add(new Class_Save_2DImageData { type = Mathf.Max(0, typeDr.value - 1) });
            Save();
        }
        public void NameSet()
        {
            GetSave_2DImages[selID].name = nameIn.text;
        }
        public void SpeedSetSlider()
        {
            if (typeDr.value <= 0) return;
            GetSave_2DImages[selID].speeds[typeDr.value - 1] = speedSlider.value;
        }
        public void SpeedSetIn()
        {
            if (typeDr.value <= 0) return;
            if (float.TryParse(speedIn.text, out var val))
                GetSave_2DImages[selID].speeds[typeDr.value - 1] = val;
        }
        public void ChangeData(int s, bool back)
        {
            var sets = GetSave_2DImages[selID];
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
            GetSave_2DImages[selID].datas.RemoveAt(s);
        }
        public void TypeChange(int s, int type)
        {
            var sets = GetSave_2DImages[selID];
            sets.datas[s].type = type;
        }
        public void BackChange(int s,bool b)
        {
            var sets = GetSave_2DImages[selID];
            sets.datas[s].back = b;
        }
        public void Inport()
        {
            if (IOTextIn.text == "") return;
            //var json = Library_Aes.DecompressFromBase64(IOTextIn.text);
            //var imgset = JsonUtility.FromJson<ImgBase>(json);
            //ImgSets[selID] = imgset;
            //Save();
        }
        public void Export()
        {
            //var json = JsonUtility.ToJson(ImgSets[selID]);
            //var b64 = Library_Aes.CompressToBase64(json);
            //IOTextIn.text = b64;
        }
    }
}

