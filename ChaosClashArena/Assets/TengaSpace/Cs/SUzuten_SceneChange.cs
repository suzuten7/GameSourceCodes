using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class SUzuten_SceneChange : MonoBehaviourPunCallbacks
{
    bool Leaves = false;
    public void SceneChanges_int(int ID)
    {
        SceneChangeUIs.SCUIDisp();
        SceneManager.LoadSceneAsync(ID);
    }
    public void SceneChanges_string(string Name)
    {
        SceneChangeUIs.SCUIDisp();
        SceneManager.LoadSceneAsync(Name);
    }
    public void SelectBacks()
    {
        Leaves = true;
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        if (!Leaves) return;
        SceneChangeUIs.SCUIDisp();
        if (PhotonNetwork.OfflineMode) SceneManager.LoadSceneAsync(1);
        else SceneManager.LoadSceneAsync(2);
    }
}
