using UnityEngine;
public class Debug_ObjBase : MonoBehaviour
{
    static Debug_ObjBase DObjBase = null;
    [SerializeField] int PhotonDebugID;
    [SerializeField] GameObject PhotonDebugs;
    void Start()
    {
        if (DObjBase == null)
        {
            DObjBase = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        PhotonDebugs.SetActive(UI_DebugDispSet.DebugDisps[PhotonDebugID]);
    }
}
