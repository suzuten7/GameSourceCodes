using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using static GlovalValues;
using UnityEngine.Audio;
using Photon.Pun;
using UnityEngine.InputSystem.UI;
using static GameInfos;
using static Player_States;
public class MenuUIs : MonoBehaviour
{
    #region エディタ変数
    PlayerInput PI;
    [SerializeField] AudioMixer AudioMix;

    [SerializeField] GameObject UIPanel;
    [SerializeField] GameObject VMouseUI;
    [SerializeField] VirtualMouseInput VMouseIn;

    [SerializeField] Image[] ButtonsBack;
    [SerializeField] GameObject[] UIs;

    [SerializeField] DispsC Disps;
    [SerializeField] SoundsC Sounds;
    [SerializeField] GameObject[] KeyCons;

    [SerializeField] Image[] HelpButtonsBack;
    [SerializeField] GameObject[] HelpUIs;

    [SerializeField] GameObject GamePlayUI;
    [SerializeField] GameObject GameInterruptionB;

    [SerializeField] bool MouseLocks;
    #endregion
    #region クラス
    [System.Serializable]
    class DispsC
    {
        public Slider CamSpeedSlider;
        public TextMeshProUGUI CamSpeedValTx;
    }
    [System.Serializable]
    class SoundsC
    {
        public Slider BaseVolumeSlider;
        public TextMeshProUGUI BaseVolumeValTx;

        public Slider BGMVolumeSlider;
        public TextMeshProUGUI BGMVolumeValTx;

        public Slider SEVolumeSlider;
        public TextMeshProUGUI SEVolumeValTx;
    }
    #endregion
    #region 内部変数
    int SelUIID = 0;
    int HelpUIID = 0;
    #endregion
    private void Start()
    {
        PI = FindFirstObjectByType<PlayerInput>();
        UIPanel.SetActive(false);
        OptionLoad();
    }
    private void LateUpdate()
    {
        if (GamePlayUI.activeSelf)
        {
            GameInterruptionB.SetActive(PhotonNetwork.IsMasterClient);
        }
        if (MouseLocks)
        {
            bool VmouseIf = false;
            if (UIPanel != null && UIPanel.activeSelf)VmouseIf = true;
            if (GInfo != null && GInfo.End) VmouseIf = true;
            if (MyPSta != null && MyPSta.HP <= 0 && !MyPSta.GostModes) VmouseIf = true;

            if (VMouseUI != null) VMouseUI.SetActive(VmouseIf);
            if (VMouseIn != null) VMouseIn.enabled = VmouseIf;
            Cursor.lockState = VmouseIf ? CursorLockMode.None : CursorLockMode.Locked;
        }
        else Cursor.lockState = CursorLockMode.None;
        if (PI.actions["Menu"].triggered) MenuOC();
        if (UIPanel.activeSelf)
        {
            for (int i = 0; i < UIs.Length; i++)
            {
                ButtonsBack[i].color = i == SelUIID ? Color.yellow : Color.gray;
                UIs[i].SetActive(i == SelUIID);
            }
            for (int i = 0; i < HelpUIs.Length; i++)
            {
                HelpButtonsBack[i].color= i == HelpUIID ? Color.yellow : Color.gray;
                HelpUIs[i].SetActive(i == HelpUIID);
            }
            switch (SelUIID)
            {
                case 0: DispsUIS(); break;
                case 1: SoundUIS(); break;
                case 2: KeyConfigUIS(); break;
            }
        }
    }
    #region 内部メソッド
    void DispsUIS()
    {
        Disps.CamSpeedSlider.value = DispOptions[0];
        Disps.CamSpeedValTx.text = DispOptions[0] + "%";
    }
    void SoundUIS()
    {
        Sounds.BaseVolumeSlider.value = SoundOptions[0];
        Sounds.BaseVolumeValTx.text = SoundOptions[0] + "%";

        Sounds.BGMVolumeSlider.value = SoundOptions[1];
        Sounds.BGMVolumeValTx.text = SoundOptions[1] + "%";

        Sounds.SEVolumeSlider.value = SoundOptions[2];
        Sounds.SEVolumeValTx.text = SoundOptions[2] + "%";
    }
    void KeyConfigUIS()
    {
        int KCUIID = -1;
        switch (PI.currentControlScheme)
        {
            case "KeyboardMouse": KCUIID = 0; break;
            case "Gamepad": KCUIID = 1; break;
            case "VR": KCUIID = -1; break;
        }
        for (int i = 0; i < KeyCons.Length; i++)
        {
            KeyCons[i].SetActive(i == KCUIID);
        }
    }
    #endregion
    #region 呼び出しメソッド
    public void MenuOC()
    {
        UIPanel.SetActive(!UIPanel.activeSelf);

    }
    public void UIChanges(int ID)
    {
        SelUIID = ID;
    }
    public void DispSets(int ID)
    {
        float Value;
        switch (ID)
        {
            default: return;
            case 0: Value = Disps.CamSpeedSlider.value; break;
        }
        DispOptions[ID] = (int)Value;
        OptionSave();
    }
    public void DispResets(int ID)
    {
        DispOptions[ID] = DispBase[ID];
        OptionSave();
    }
    public void SoundSets(int ID)
    {
        float Value;
        switch (ID)
        {
            default: return;
            case 0: Value = Sounds.BaseVolumeSlider.value; break;
            case 1: Value = Sounds.BGMVolumeSlider.value; break;
            case 2: Value = Sounds.SEVolumeSlider.value; break;
        }
        SoundOptions[ID] = (int)Value;
        OptionSave();
    }
    public void SoundResets(int ID)
    {
        SoundOptions[ID] = SoundBase[ID];
        OptionSave();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public void GameInterruption()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        var CRoom = PhotonNetwork.CurrentRoom;
        var RoomHashs = CRoom.CustomProperties;
        RoomHashs["GameStarts"] = false;
        CRoom.SetCustomProperties(RoomHashs);
        PhotonNetwork.DestroyAll();
        PhotonNetwork.LoadLevel(0);
    }

    public void HelpUIChange(int ID)
    {
        HelpUIID = ID;
    }
    #endregion
}
