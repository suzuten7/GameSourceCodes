using UnityEngine;
using UnityEngine.SceneManagement;

public class Net_NoBacks : MonoBehaviour
{
    [SerializeField] int titleID;
    [SerializeField] float checkTime = 10;
    float t = 0;
    void Update()
    {
        if (Net_Connect.InsRunner == null || (!Net_Connect.InsRunner.IsServer && !Net_Connect.InsRunner.IsConnectedToServer))
        {
            t += Time.deltaTime;
            if (t < checkTime && Net_Connect.NetCon != null) return;
            if (Net_Connect.InsRunner != null) Net_Connect.InsRunner.Shutdown();
        }
        else t = 0;
        if (Net_Connect.NetCon == null) SceneManager.LoadScene(titleID);
    }
}
