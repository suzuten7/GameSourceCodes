using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;
using static Suzuten_ActionData;
using static Suzuten_DataBase;
using static Suzuten_PlayerSets;
using static CameraSettings_Gabu;
using static Suzuten_SEPlays;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using System.Linq;

public class Suzuten_SelectUI : MonoBehaviourPunCallbacks
{
    #region 変数
    public Suzuten_DataBase DB;
    public int CharaXcounts;
    public CharaUIs[] CUIs = new CharaUIs[2];
    public GameObject MouseUIs;
    public TextMeshProUGUI CountDownTx;
    public GameObject OPUI;
    public Toggle BOPStartVST;
    public TMP_InputField[] BOPSetsI_Battle;
    public TMP_InputField[] BOPSetsI_HMSP;
    public TMP_InputField[] BOPSetsI_Moves;
    public TMP_InputField[] BOPSetsI_AtkStan;
    public TMP_Dropdown BOPSetD;
    public TMP_Dropdown StageDr;
    public TextMeshProUGUI StageInfo;
    [SerializeField] GameObject NetUI;
    static public Vector2Int[] SelectSlots = new Vector2Int[2];
    [System.NonSerialized]public bool[] PLOKs = new bool[2];
    [System.NonSerialized] public int[] SelACID = new int[2];
    [System.NonSerialized] public int[] SelACSy = new int[2];
    [System.NonSerialized] public bool[] SelACSetB = new bool[2];
    [System.NonSerialized] public int[] SelACSetSlot = new int[2];
    static int[] BSelectSlots = new int[2];
    GameObject[] CharaModels = new GameObject[2];
    int CountDown;
    int SceneIDs;
    public const int RaCharaCo=3;
    #endregion
    [System.Serializable]
    public class CharaUIs
    {
        public Transform ModelPoss;
        public RectTransform SelectUIs;
        public List<Suzuten_CharaSinUI> CSUIs;
        public RawImage BackIm;

        public GameObject OKObj;

        public TextMeshProUGUI CharaName;
        public TextMeshProUGUI CharaInfo;
        public GameObject ParaUI;
        public TextMeshProUGUI CharaPaName;
        public TextMeshProUGUI[] CharaParaTxs;
        public Image[] CharaParaBars;

        public GameObject ACUI;
        public TextMeshProUGUI[] ACNames;
        public Image[] ACBacks;
        public Image[] ACBaseSy;
        public TextMeshProUGUI ACName;
        public GameObject[] ACUIss;
        public TextMeshProUGUI ACBInfo;
        public VideoPlayer VP;
        public RawImage VDImage;
        public TextMeshProUGUI ACSyInfo;
        public TextMeshProUGUI ACSetSelIDTxs;
        public TextMeshProUGUI ACSetSelNaTxs;
        public Image[] ACSetImages;
        public TextMeshProUGUI[] ACSetIDTxs;
        public TextMeshProUGUI[] ACSetNaTxs;

        public Texture[] VDTextures;

    }

    void Awake()
    {
        StartOPSets();

    }
    void Start()
    {
        if(NetUI!=null) NetUI.SetActive(false);
        for (int i = 0; i < 2; i++)
        {
            ACIDLoad(DB,i);
        }
        StageDr.options.Clear();
        for (int i = 0; i < DB.StageDatas.Length+1; i++)
        {
            StageDr.options.Add(new TMP_Dropdown.OptionData());
            if (i <= 0)
            {
                StageDr.options[i].text = "ランダム";
                StageDr.options[i].image = DB.RandomStageImage;
            }
            else
            {
                Suzuten_StageData StageDa = DB.StageDatas[i-1];
                StageDr.options[i].text = StageDa.StageName;
                StageDr.options[i].image = StageDa.DrImage;
            }

        }
        StageDr.value = StageDr.value == 0 ? 1:0;
        StageDr.value = StageID;

        BOPSetD.value = PlayerPrefs.GetInt("BOPSet", 0);

        WinResets();
        OPUI_OC(false);
        for (int i = 0; i < 2; i++)
        {
            BSelectSlots[i] = -2;
            UIClose(i);
        }


    }
    void LateUpdate()
    {
        Time.timeScale = 1;
        bool OKs = true;
        #region 各プレイヤー処理

        for (int i = 0; i < 2; i++)
        {
            CharaUIs CUI = CUIs[i];
            #region キャラ一覧
            int SelectID = SelectSlots[i].x + SelectSlots[i].y * CharaXcounts - RaCharaCo;
            if (SelectID >= DB.Charas.Length) SelectID = -3;
            CharaID[i] = SelectID;
            for (int j = 0; j < CharaXcounts*3; j++)
            {
                if (CUI.CSUIs.Count <= j) CUI.CSUIs.Add(Instantiate(CUI.CSUIs[0], CUI.CSUIs[0].transform.parent));
                int Y = SelectSlots[i].y;
                if (j < CharaXcounts) Y -= 1;
                else if (j >= CharaXcounts*2) Y += 1;
                Y = (int)Mathf.Repeat(Y, ((DB.Charas.Length+ RaCharaCo) / CharaXcounts)+ 1);
                int CharaID = Y* CharaXcounts + (j % CharaXcounts) - RaCharaCo;
                if (CharaID >= DB.Charas.Length) CharaID = -RaCharaCo;
                CUI.CSUIs[j].PID = i;
                CUI.CSUIs[j].IDs = j;
                if (CharaID >= 0)
                {
                    Suzuten_CharaData CD = DB.Charas[CharaID];
                    string CName = CD.CharaName;
                    if (CD.CharaNameEnter > 0)
                    {
                        int Cent = CName.Length - CD.CharaNameEnter;
                        CName = CName.Substring(0, CD.CharaNameEnter) + "\n" + CName.Substring(CD.CharaNameEnter, Cent);
                    }
                    CUI.CSUIs[j].CharaNameTx.text = CName;
                    CUI.CSUIs[j].CharaImage.texture = CD.CharaImage;
                    if (CD.UseBandChara) CUI.CSUIs[j].BackImage.color = new Color(0.3f, 0, 0.3f);
                    else if (CD.RandomNoSelects) CUI.CSUIs[j].BackImage.color = new Color(1f, 0.4f, 0.4f);
                    else CUI.CSUIs[j].BackImage.color = new Color(0.8f, 0.8f, 0.8f);
                }
                else
                {

                    CUI.CSUIs[j].CharaImage.texture = DB.RandomCharaImage;
                    switch (CharaID)
                    {
                        default: CUI.CSUIs[j].BackImage.color = new Color(0.8f, 0.8f, 0.8f);
                            CUI.CSUIs[j].CharaNameTx.text = "ランダム";
                            break;
                        case -2: CUI.CSUIs[j].BackImage.color = new Color(1f, 0.4f, 0.4f);
                            CUI.CSUIs[j].CharaNameTx.text = "ランダム(除外込み)";
                            break;
                        case -1: CUI.CSUIs[j].BackImage.color = new Color(0.3f, 0, 0.3f);
                            CUI.CSUIs[j].CharaNameTx.text = "ランダム(禁止込み)";
                            break;
                    }

                    
                }

            }
            Vector2 UIPos = CUI.CSUIs[SelectSlots[i].x+ CharaXcounts].transform.position;
            UIPos += new Vector2(i == 0 ? -20f : 20f, 30f);
            CUI.SelectUIs.transform.position = UIPos;
            if(BSelectSlots[i] != SelectID)
            {
                BSelectSlots[i] = SelectID;
                if (CharaModels[i] != null) Destroy(CharaModels[i]);
                Vector3 ModelPos = CUI.ModelPoss.transform.position;
                ModelPos.y += 2;
                if (SelectID >= 0)
                {
                    GameObject InsObj = Instantiate(DB.Charas[SelectID].ModelObj, ModelPos, Quaternion.identity);
                    CharaModels[i] = InsObj;
                    SEPlays(DB.Charas[SelectID].SelectSE, i);
                    ACIDLoad(DB,i);
                }
            }
            if(CharaModels[i]) CharaModels[i].transform.eulerAngles += new Vector3(0, 1, 0);
            #endregion
            #region OK表示
            CUI.OKObj.SetActive(PLOKs[i]);
            if (!PLOKs[i]) OKs = false;
            #endregion
            #region 背景
            if (CUI.BackIm != null)
            {
                if (StageDr.value > 0)
                {
                    CUI.BackIm.color = Color.white;
                    CUI.BackIm.texture = DB.StageDatas[StageDr.value-1].BackImage;
                }
                else CUI.BackIm.color = Color.clear;
            }
            #endregion
            #region キャラステータス表示
            if (SelectID >= 0)
            {
                Suzuten_CharaData CD = DB.Charas[SelectID];
                #region ステータス
                CUI.CharaName.text = CD.CharaName;
                CUI.CharaInfo.text = CD.CharaInfo;
                CUI.CharaPaName.text = CD.CharaName;

                CUI.CharaParaTxs[0].text = CD.MHP +"<size=60%>("+ (CD.HPRegene).ToString("F1")+"/秒)</size>";
                CUI.CharaParaTxs[1].text = CD.StanRegist + "<size=60%>(" + (CD.STRegene).ToString("F1") + "/秒)</size>";
                CUI.CharaParaTxs[2].text = CD.MSP + "<size=60%>(" + (CD.SPRegene).ToString("F1") + "/秒)</size>";
                CUI.CharaParaTxs[3].text = (CD.DamageSPPer).ToString("F1") + "%";
                CUI.CharaParaTxs[4].text = CD.Atk.ToString();
                CUI.CharaParaTxs[5].text = CD.MMP + "<size=60%>(" + (CD.MPRegene).ToString("F1") + "/秒)</size>";
                CUI.CharaParaTxs[6].text = CD.PhisPow.ToString();
                CUI.CharaParaTxs[7].text = CD.PhisRange.ToString();
                CUI.CharaParaTxs[8].text = CD.GroundSpeed.ToString();
                CUI.CharaParaTxs[9].text = CD.AirSpeed.ToString();
                CUI.CharaParaTxs[10].text = CD.BoostSpeed.ToString();
                CUI.CharaParaTxs[11].text = CD.JumpPower.ToString();
                CUI.CharaParaTxs[12].text = CD.DownPower.ToString();

                float[] Vals = new float[13];

                Vals[0] = 0.5f * CD.MHP / DB.CParaBase.MHP;
                Vals[1] = 0.5f * CD.StanRegist / DB.CParaBase.StanRegist;
                Vals[2] = 0.5f * CD.MSP / DB.CParaBase.MSP;
                Vals[3] = 0.5f * CD.DamageSPPer / DB.CParaBase.DamageSPPer;
                Vals[4] = 0.5f * CD.Atk / DB.CParaBase.Atk;
                Vals[5] = 0.5f * CD.MMP / DB.CParaBase.MMP;
                Vals[6] = 0.5f * CD.PhisPow / DB.CParaBase.PhisPow;
                Vals[7] = 0.5f * CD.PhisRange / DB.CParaBase.PhisRange;
                Vals[8] = 0.5f * CD.GroundSpeed / DB.CParaBase.GroundSpeed;
                Vals[9] = 0.5f * CD.AirSpeed / DB.CParaBase.AirSpeed;
                Vals[10] = 0.5f * CD.BoostSpeed / DB.CParaBase.BoostSpeed;
                Vals[11] = 0.5f * CD.JumpPower / DB.CParaBase.JumpPower;
                Vals[12] = 0.5f * CD.DownPower / DB.CParaBase.DownPower;
                for (int k = 0; k < 13; k++)
                {
                    if (CUI.CharaParaBars[k].fillAmount < Vals[k]) CUI.CharaParaBars[k].fillAmount += 0.01f;
                    if (CUI.CharaParaBars[k].fillAmount > Vals[k]) CUI.CharaParaBars[k].fillAmount -= 0.01f;
                }
                #endregion
                #region アクション
                int SetID = Mathf.Clamp(ACSetID[i, SelACID[i]], -1, CD.Actions.Length);

                int ID = !SelACSetB[i] ? SetID : SelACSetSlot[i];
                Suzuten_ActionData AD = ID >= 0 ? CD.Actions[ID] : null;
                if (AD != null)
                {
                    CUI.ACName.text = AD.ACName;
                    if (SelACSy[i] == 0)
                    {
                        CUI.ACBInfo.text = AD.ACInfo;
                        if (AD.SPCost > 0)CUI.ACBInfo.text += "\nSPコスト:" + (100f * AD.SPCost / CD.MSP).ToString("F0") + "%(" + AD.SPCost + ")";
                        CUI.ACBInfo.text += "\nCT:" + AD.CT.ToString("F1") + "秒";
                        VideoClip VCp = AD.Video;
                        if (CUI.VP.clip != VCp)
                        {
                            CUI.VP.clip = VCp;
                            CUI.VP.time = 0;
                        }
                        CUI.VDImage.texture = CUI.VDTextures[VCp != null ? 0 : 1];
                    }
                    else
                    {
                        CUI.ACSyInfo.text = "効果詳細";

                        if (AD.Shots != null)
                        {
                            for (int j = 0; j < AD.Shots.Length; j++)
                            {
                                CUI.ACSyInfo.text += "\n" + ShotTx(AD.Shots[j], j+1);
                            }
                        }
                        if (AD.Moves != null)
                        {
                            for (int j = 0; j < AD.Moves.Length; j++)
                            {
                                CUI.ACSyInfo.text += "\n" + MoveTx(AD.Moves[j],j+1);
                            }
                        }
                        if (AD.MyBufs != null)
                        {
                            for (int j = 0; j < AD.MyBufs.Length; j++)
                            {
                                MyBufsC MyBuf = AD.MyBufs[j];
                                if (MyBuf.Bufs != null)
                                {
                                    for (int k = 0; k < MyBuf.Bufs.Length; k++)
                                    {
                                        CUI.ACSyInfo.text += "\n" + BufTx(MyBuf.Bufs[k]);
                                    }
                                }
                            }
                        }
                        CUI.ACSyInfo.text += "\n数値詳細";
                        if (AD.SPCost > 0) CUI.ACSyInfo.text += "\nSPコスト:" +(100f * AD.SPCost/CD.MSP).ToString("F0")+"%(" + AD.SPCost+")";
                        CUI.ACSyInfo.text += "\nスタン耐性:" + AD.StanRegist;
                        CUI.ACSyInfo.text += "\n時間:" + (AD.EndTime / 60f).ToString("F2") + "秒"; ;
                        CUI.ACSyInfo.text += "\nCT:" + AD.CT.ToString("F1") + "秒";
                    }
                }
                else
                {
                    CUI.ACName.text = "無し";
                    CUI.ACBInfo.text = "";
                    CUI.ACSyInfo.text = "";
                    CUI.VDImage.texture = CUI.VDTextures[1];

                }
                if (SelACSetB[i])
                {
                    int SlotID = SelACSetSlot[i];
                    Suzuten_ActionData ADSlot = SlotID >= 0 ? CD.Actions[SlotID] : null;

                    CUI.ACSetSelIDTxs.text = (SlotID>=0 ? (SlotID + 1):"-") + "/" + CD.Actions.Length;
                    CUI.ACSetSelNaTxs.text = ADSlot !=null ? ADSlot.ACName : "無し";
                    for (int j = 0; j < 5; j++)
                    {
                        int SlotIDs = (int)Mathf.Repeat(SlotID + j - 2+1, CD.Actions.Length+1)-1;
                        bool Used = false;
                        for(int k = 0; k < 5; k++)
                        {
                            if (SlotIDs == ACSetID[i, k]&& ACSetID[i,SelACID[i]] != ACSetID[i, k]) Used = true;
                        }
                        Suzuten_ActionData ADss = SlotIDs >= 0 ? CD.Actions[SlotIDs] : null;
                        if (ADss!=null)
                        {

                            if (j == 2) CUI.ACSetImages[j].color = !Used ? Color.yellow : new Color(1, 0.5f, 0);
                            else if (Used) CUI.ACSetImages[j].color = Color.red;
                            else if (ADss.SPAC)
                            {
                                Color Col = CUI.ACSetImages[j].color;
                                Color.RGBToHSV(Col, out Col.r, out Col.g, out Col.b);
                                Col = Color.HSVToRGB(Mathf.Repeat(Col.r + 0.01f, 1f), 1, 1);
                                CUI.ACSetImages[j].color = Col;
                            }
                            else if (ADss.SPCost > 0) CUI.ACSetImages[j].color = Color.cyan;
                            else CUI.ACSetImages[j].color = Color.white;
                            CUI.ACSetIDTxs[j].text = (SlotIDs + 1).ToString();
                            CUI.ACSetNaTxs[j].text = ADss.ACName;
                        }
                        else
                        {
                            CUI.ACSetImages[j].color = Color.gray;
                            CUI.ACSetIDTxs[j].text = "-";
                            CUI.ACSetNaTxs[j].text = "無し";
                        }
                    }
                }
                for (int j = 0; j < 5; j++)
                {
                    int IDss = ACSetID[i, j];
                    Suzuten_ActionData ADs = IDss>=0 ? CD.Actions[IDss] : null;
                    if (ADs)
                    {
                        CUI.ACNames[j].text = ADs.ACName;
                    }
                    else CUI.ACNames[j].text = "無し";
                }
                #endregion
            }
            else
            {
                #region ランダム用
                switch (SelectID)
                {
                    default:
                        CUI.CharaName.text = "ランダム";
                        CUI.CharaInfo.text = "ランダムに選ばれる\nどれでもいい人におすすめ!!!";
                        CUI.CharaPaName.text = "ランダム";
                        break;
                    case -2:
                        CUI.CharaName.text = "ランダム(除外込み)";
                        CUI.CharaInfo.text = "ランダム除外を\n含んだキャラから選ばれる\n性能差がより大きい";
                        CUI.CharaPaName.text = "ランダム(除外込み)";
                        break;
                    case -1:
                        CUI.CharaName.text = "ランダム(禁止込み)";
                        CUI.CharaInfo.text = "全キャラから\n選ばれる\nちーたー引けば\n勝ちのようなもの";
                        CUI.CharaPaName.text = "ランダム(禁止込み)";
                        break;
                }

                for(int k = 0; k < 13; k++)
                {
                    CUI.CharaParaTxs[k].text = "???";
                    CUI.CharaParaBars[k].fillAmount = 0.5f + Mathf.Sin(Time.time*5f+(k*1.5f))/2f;
                }
                CUI.ACName.text = "???";
                CUI.ACBInfo.text = "???";
                CUI.ACSyInfo.text = "効果詳細\n???\n数値詳細\n???";
                for (int j = 0; j < 5; j++)
                {
                    CUI.ACNames[j].text = "???";
                }
                CUI.VDImage.texture = CUI.VDTextures[1];
                CUI.VP.clip = null;
                for (int j = 0; j < 5; j++)
                {
                    CUI.ACSetSelIDTxs.text = "???/???";
                    CUI.ACSetSelNaTxs.text = "???";
                    CUI.ACSetImages[j].color = j == 2 ? Color.yellow : Color.white;
                    CUI.ACSetIDTxs[j].text = "???";
                    CUI.ACSetNaTxs[j].text = "???";
                }
                #endregion
            }
            #endregion
            #region アクション選択
            for (int j = 0; j < 5; j++)
            {

                if (SelectID >= 0)
                {
                    int IDss = ACSetID[i, j];
                    Suzuten_ActionData ADs = IDss >= 0 ? DB.Charas[SelectID].Actions[IDss] : null;
                    if (ADs)
                    {
                        if (SelACID[i] == j) CUI.ACBacks[j].color = Color.yellow;
                        else if (ADs.SPAC)
                        {
                            Color Col = CUI.ACBacks[j].color;
                            Color.RGBToHSV(Col, out Col.r, out Col.g, out Col.b);
                            Col = Color.HSVToRGB(Mathf.Repeat(Col.r + 0.01f, 1f), 1, 1);
                            CUI.ACBacks[j].color = Col;
                        }
                        else if (ADs.SPCost > 0) CUI.ACBacks[j].color = Color.cyan;
                        else CUI.ACBacks[j].color = Color.white;
                    }
                    else CUI.ACBacks[j].color = SelACID[i] == j ? Color.yellow : Color.gray;
                }
                else CUI.ACBacks[j].color = SelACID[i] == j ? Color.yellow : Color.white;
            }
            for (int j = 0; j < 2; j++)
            {
                CUI.ACBaseSy[j].color = SelACSy[i] == j ? Color.yellow : Color.white;
            }
            for (int j = 0; j < 2; j++)
            {
                CUI.ACUIss[j].SetActive(j== SelACSy[i]);
            }
            CUI.ACUIss[2].SetActive(SelACSetB[i]);
            #endregion
        }
        #endregion
        #region ローカルカウント
        if (OKs&& !SettingOpen)
        {
            MouseUIs.SetActive(false);
            if (CountDown > 120) CountDownTx.text = "III";
            else if (CountDown > 60) CountDownTx.text = "10<size=25%>(2)";
            else if (CountDown >= 0) CountDownTx.text = "壱";
            else if (CountDown < 0)
            {
                CountDownTx.text = "Start!!!";
                BattleStartB();
            }
        }
        else
        {
            MouseUIs.SetActive(true);
            CountDown = 180;
            CountDownTx.text = "";
        }
        #endregion
        #region バトル設定
        if (OPUI.activeSelf)
        {
            BOPStartVST.isOn = BOPC_Battle[0] == 1 ? true : false;
            for (int i = 0; i < BOPSetsI_Battle.Length; i++)
            {
                if (!BOPSetsI_Battle[i].isFocused) BOPSetsI_Battle[i].text = BOPC_Battle[i + 1].ToString();
            }
            for (int i = 0; i < BOPSetsI_HMSP.Length; i++)
            {
                if (!BOPSetsI_HMSP[i].isFocused) BOPSetsI_HMSP[i].text = BOPC_HMSP[i].ToString();
            }
            for (int i = 0; i < BOPSetsI_Moves.Length; i++)
            {
                if (!BOPSetsI_Moves[i].isFocused) BOPSetsI_Moves[i].text = BOPC_Moves[i].ToString();
            }
            for (int i = 0; i < BOPSetsI_AtkStan.Length; i++)
            {
                if (!BOPSetsI_AtkStan[i].isFocused) BOPSetsI_AtkStan[i].text = BOPC_AtkStan[i].ToString();
            }
        }
        #endregion
        #region ステージ
        if (StageDr.value <= 0) StageInfo.text = "ランダムな\nステージが\n選ばれる";
        else StageInfo.text = DB.StageDatas[StageDr.value - 1].StageInfo;
        StageID = StageDr.value;
        #endregion
        #region ディスプレイ
        if (MultiDisplayMode)
        {
            int count = Mathf.Min(Display.displays.Length, 2);
            for (int i = 0; i < count; ++i)
            {
                Display.displays[i].Activate();
            }
        }
        #endregion
    }
    private void FixedUpdate()
    {
        CountDown--;
    }
    #region アクションテキスト用
    string ShotTx(ShotsC Shot, int Co = 0)
    {
        string Tx = "";
        
        var SDams = Shot.Damages;
       
        Tx += "攻撃" + Co + ":威力" + SDams.Damage;
        int ShotCos = 0;
        foreach(var Firing in Shot.Firings)
        {
            int SCo = Mathf.Max(Firing.ShotCount, 1);
            SCo *= (Firing.ShotTimes.y - Firing.ShotTimes.x + 1) / Mathf.Max(1, Firing.ShotTimes.z);
            ShotCos += SCo;
        }
        if (ShotCos > 1) Tx += "×" + ShotCos;
        Tx += "/スタン:" + SDams.StanPow.x + "," + (SDams.StanPow.y / 60f).ToString("F1") + "秒";
        if (Shot.AddBufs != null)
        {
            for (int k = 0; k < Shot.AddBufs.Length; k++)
            {
                BufAddsC BufAdd = Shot.AddBufs[k];
                Tx += "\n" + BufTx(BufAdd);
            }
        }
        return Tx;
    }
    string MoveTx(MovesC Move, int Co = 0)
    {
        string Tx = "";
        Tx += "移動" + Co + ":方向[" + Move.RotBase + "]";
        if (Move.Pows.z != 0) Tx += "前後:" + Move.Pows.z;
        if (Move.Pows.x != 0) Tx += "横:" + Move.Pows.x;
        if (Move.Pows.y != 0) Tx += "上下:" + Move.Pows.y;
        if (Move.YPower != 0) Tx += "Y方向:" + Move.YPower;
        return Tx;
    }
    string BufTx(BufAddsC BufAdd,int Co=0)
    {
        string Tx = "";
        Tx += "状態" + (Co>0? Co:"") + ":" + BufAdd.Buf + "[" + BufAdd.BufOP+"]";
        if (BufAdd.BufOP != BufOPE.解除)
        {
            if (BufAdd.BufPower > 0)
            {
                 Tx +=(BufAdd.BufOP == BufOPE.増加 ? "\n":",") + "段階:" + BufAdd.BufPower;
                if (BufAdd.BufOP == BufOPE.増加) Tx += "(Max:" + BufAdd.BufPowerMax+")";
            }
            Tx += (BufAdd.BufOP == BufOPE.増加 ? "\n" : ",") + "時間:" + (BufAdd.BufTime / 60f).ToString("F1") + "秒";
            if (BufAdd.BufOP == BufOPE.増加) Tx += "(Max:" + (BufAdd.BufTimeMax / 60f).ToString("F1") + "秒)";
        }
        return Tx;
    }
    #endregion
    #region メソッド
    /// <summary>UI閉じ</summary>
    public void UIClose(int ID)
    {
        CUIs[ID].ParaUI.SetActive(false);
        CUIs[ID].ACUI.SetActive(false);
        SelACSetB[ID] = false;
    }
    /// <summary>シーン変更</summary>
    public void SceneB(int SceneID)
    {
        SceneChangeUIs.SCUIDisp();
        SceneManager.LoadSceneAsync(SceneID);
    }
    /// <summary>バトルスタート</summary>
    public void BattleStartB()
    {
        int SceneID;
        if (StageDr.value <= 0)
        {
            while (true)
            {
                Suzuten_StageData StageD = DB.StageDatas[Random.Range(0, DB.StageDatas.Length)];
                if (!StageD.RandomNoSelects)
                {
                    SceneID = StageD.StageSceneID;
                    break;
                }
            }
        }
        else SceneID = DB.StageDatas[StageDr.value - 1].StageSceneID;

        SceneIDs = SceneID;
        PhotonNetwork.CreateRoom("SinglePlay");
    }
    /// <summary>バトル設定UI開閉</summary>
    public void OPUI_OC(bool val)
    {
        OPUI.SetActive(val);
    }
    /// <summary>バトル設定変更</summary>
    public void BOPSets(int ID)
    {
        if (ID < 100)
        {
            if (ID == 0)BOPC_Battle[ID] = BOPStartVST.isOn ? 1 : 0;
            else BOPC_Battle[ID] = int.Parse(BOPSetsI_Battle[ID - 1].text);
            if(BOPSetD.value==1) PlayerPrefs.SetInt("BOP_Battle_" + ID, BOPC_Battle[ID]);
        }
        else if (ID < 200)
        {
            BOPC_HMSP[ID-100] = int.Parse(BOPSetsI_HMSP[ID-100].text);
            if (BOPSetD.value == 1) PlayerPrefs.SetInt("BOP_HMSP_" + (ID-100), BOPC_HMSP[ID - 100]);
        }
        else if (ID <300)
        {
            BOPC_Moves[ID-200] = int.Parse(BOPSetsI_Moves[ID-200].text);
            if (BOPSetD.value == 1) PlayerPrefs.SetInt("BOP_Moves_" + (ID - 200), BOPC_Moves[ID - 200]);
        }
        else if (ID < 400)
        {
            BOPC_AtkStan[ID-300] = int.Parse(BOPSetsI_AtkStan[ID-300].text);
            if (BOPSetD.value == 1) PlayerPrefs.SetInt("BOP_AtkStan_" + (ID - 300), BOPC_AtkStan[ID - 300]);
        }
        BOP_Battle = BOPC_Battle.ToArray();
        BOP_HMSP = BOPC_HMSP.ToArray();
        BOP_Moves = BOPC_Moves.ToArray();
        BOP_AtkStan = BOPC_AtkStan.ToArray();
    }
    /// <summary>バトル設定プリセット</summary>
    public void BOPPriSet(int ID)
    {
        PlayerPrefs.SetInt("BOPSet", ID);
        BOPPriSets(ID);
    }
    #endregion
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.OfflineMode)
        {
            SceneChangeUIs.SCUIDisp();
            SceneManager.LoadSceneAsync(SceneIDs);
        }
    }

}
