using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static CameraSettings_Gabu;


public class SettingsOnOffSystem_Gabu : MonoBehaviour
{
    #region 変数

    [SerializeField]PlayerInput[] playerInput;

    [SerializeField, Header("action名")]
    private string action = "Settings";

    [SerializeField, Header("SettingsCanvasを入れて")]
    private GameObject settingsCanvas;
    [SerializeField] GameObject settingsCanvasP2;
    [SerializeField] GameObject ExplanaUI;
    float stay = 0;

    static public SettingsOnOffSystem_Gabu Settings;
    #endregion
    private void Start()
    {
        settingsCanvas.SetActive(false);
        settingsCanvasP2.SetActive(false);
        ExplanaUI.SetActive(false);
        SettingOpen = false;
    }
    void Update()
    {
        Settings = this;
        stay -= 0.02f;
        bool ButtonIn = false;
        if (playerInput != null)
        {
            for (int i = 0; i < playerInput.Length; i++)
            {
                if (playerInput[i] !=null&& playerInput[i].actions[action].triggered) ButtonIn = true;
            }
        }
        if (Suzuten_PlayerSets.PSs != null)
        {
            for (int i = 0; i < Suzuten_PlayerSets.PSs.Length; i++)
            {
                if (Suzuten_PlayerSets.PSs[i] !=null&& Suzuten_PlayerSets.PSs[i].PI.actions[action].triggered) ButtonIn = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)) ButtonIn = true;

        if (stay<=0&&ButtonIn)
        {
            stay = 0.2f;
            OnOffSettings();
        }

        settingsCanvasP2.SetActive(settingsCanvas.activeSelf);
        SettingOpen = settingsCanvas.activeSelf;
    }

    public void OnOffSettings()
    {
        settingsCanvas.SetActive(!settingsCanvas.activeSelf);
    }
    public void ExplanaOC()
    {
        ExplanaUI.SetActive(!ExplanaUI.activeSelf);
    }
}
