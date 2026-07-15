using Photon.Pun;
using UnityEngine;

public class Net_MyObj : MonoBehaviourPun
{
    [Tooltip("自己用オブジェクト"), SerializeField] GameObject[] MyObjs;
    [Tooltip("他人用オブジェクト"), SerializeField] GameObject[] OtherObjs;
    private void Start()
    {
        Set();
    }
    void Update()
    {
        Set();
    }
    void Set()
    {
        for (int i = 0; i < MyObjs.Length; i++) MyObjs[i].SetActive(photonView.IsMine);
        for (int i = 0; i < OtherObjs.Length; i++) OtherObjs[i].SetActive(!photonView.IsMine);
    }
}
