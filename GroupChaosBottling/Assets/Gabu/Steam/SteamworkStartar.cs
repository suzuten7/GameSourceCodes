using Steamworks;
using UnityEngine;
using TMPro;

public class SteamworkStartar : MonoBehaviour
{

    public TextMeshProUGUI tmpUGUI;
    void Start()
    {
        if (!SteamAPI.Init())
        {
            Debug.Log("Steam init failed");
        }
        Debug.Log(SteamUtils.GetAppID());
        tmpUGUI.text = SteamUtils.GetAppID().ToString();

    }
    void Update()
    {
        SteamAPI.RunCallbacks();
    }

    void OnApplicationQuit()
    {
        SteamAPI.Shutdown();
    }
}
