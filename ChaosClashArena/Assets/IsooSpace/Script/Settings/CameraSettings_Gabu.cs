using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CameraSettings_Gabu : MonoBehaviour
{
    [SerializeField] SettingUIC[] SettingUIs;
    public Toggle MultiDisplayT;
    public Toggle MoblieACStickT;
    [System.Serializable]
    class SettingUIC
    {
        public Toggle[] Toggles;
        public Slider[] Sliders;
        public TextMeshProUGUI[] SliderVals;
        public TMP_Dropdown AutoDropDown;
    }
    /// <summary>
    /// <para>x=プレイヤー,y=下</para>
    /// <para>0=自動カメラ感度 1=手動カメラ感度</para>
    /// <para>2=カメラ距離 3=カメラ横位置 4=カメラ高度 </para>
    /// <para>5=コントローラー振動 </para>
    /// </summary>
    public static int[,] Settings_Int = new int[2,6];
    /// <summary>
    /// <para>0=無効</para>
    /// <para>1=常時ガード 2=常時ジャンプ 3=常時ジャンプ,落下</para>
    /// <para>4=常時接近 5=常時逃走</para>
    /// <para>6=てきとう自動戦闘LV1 7=てきと自動戦闘LV2 8=てき自動戦闘LV3</para>
    /// </summary>
    public static int[] Settings_Auto = new int[2];
    /// <summary>
    /// <para>x=プレイヤー,y=下</para>
    /// <para>0=矢印onoff,1=UI非表示</para>
    /// <para>2=HP数字表示</para>
    /// </summary>
    public static bool[,] Settings_Bool = new bool[2, 3];

    static public bool SettingOpen;

    public static bool MultiDisplayMode;

    public static bool MoblieACStick;

    public static bool Starts = false;



    private void Awake()
    {
        if (!Starts)
        {
            Starts = true;
            MultiDisplayMode = false;
            MoblieACStick = true;
            StartSets();
        }

    }
    void StartSets()
    {
        for (int i = 0; i < 2; i++)
        {
            Settings_Bool[i, 0] = PlayerPrefs.GetInt("Setting_B_" + i + "_0", 1) == 1 ? true : false;
            Settings_Bool[i, 1] = PlayerPrefs.GetInt("Setting_B_" + i + "_1", 0) == 1 ? true : false;
            Settings_Bool[i, 2] = PlayerPrefs.GetInt("Setting_B_" + i + "_2", 0) == 1 ? true : false;
            Settings_Int[i, 0] = PlayerPrefs.GetInt("Setting_I_" + i + "_0", 100);
            Settings_Int[i, 1] = PlayerPrefs.GetInt("Setting_I_" + i + "_1", 100);
            Settings_Int[i, 2] = PlayerPrefs.GetInt("Setting_I_" + i + "_2", 100);
            Settings_Int[i, 3] = PlayerPrefs.GetInt("Setting_I_" + i + "_3", i == 1 ? 100 : -100);
            Settings_Int[i, 4] = PlayerPrefs.GetInt("Setting_I_" + i + "_4", 100);
            Settings_Int[i, 5] = PlayerPrefs.GetInt("Setting_I_" + i + "_5", 100);
            Settings_Auto[i] = 0;
        }
    }
    public void Start()
    {
        LateUpdate();
    }
    private void LateUpdate()
    {
        MultiDisplayT.isOn = MultiDisplayMode;
        MoblieACStickT.isOn = MoblieACStick;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < Settings_Bool.GetLength(1); j++)
            {
                SettingUIs[i].Toggles[j].isOn = Settings_Bool[i, j];
            }
            for (int j = 0; j < Settings_Int.GetLength(1); j++)
            {
                SettingUIs[i].Sliders[j].value = Settings_Int[i,j];
                SettingUIs[i].SliderVals[j].text = Settings_Int[i, j] + "%";
            }
            SettingUIs[i].AutoDropDown.value = Settings_Auto[i];
        }

    }
    public void MultiDisplaySet()
    {
        MultiDisplayMode = MultiDisplayT.isOn;
    }
    public void MoblieACStickSet()
    {
        MoblieACStick = MoblieACStickT.isOn;
    }

    public void SliderSets_P1(int ID)
    {
        Sets(0,ID);
    }
    public void SliderSets_P2(int ID)
    {
        Sets(1, ID);
    }
    public void AutoDrSets(int Player)
    {
        Settings_Auto[Player] = SettingUIs[Player].AutoDropDown.value;
    }
    void Sets(int Player,int ID)
    {
        if (ID < 0)
        {
            Settings_Bool[Player, -ID - 1] = SettingUIs[Player].Toggles[-ID - 1].isOn;
            PlayerPrefs.SetInt("Setting_B_" + Player + "_" + (-ID - 1), Settings_Bool[Player, -ID - 1] ? 1 : 0);
        }
        else
        {
            Settings_Int[Player, ID] = Mathf.RoundToInt(SettingUIs[Player].Sliders[ID].value);
            PlayerPrefs.SetInt("Setting_I_" + Player + "_"+ID, Settings_Int[Player, ID]);
        }
    }
    public void ResetsB(int Player)
    {
        for (int i = 0; i < Settings_Bool.Length; i++) PlayerPrefs.DeleteKey("Setting_B_" + Player + "_" + i);
        for (int i = 0; i < Settings_Int.Length; i++) PlayerPrefs.DeleteKey("Setting_I_" + Player + "_" + i);
        StartSets();
    }
}
