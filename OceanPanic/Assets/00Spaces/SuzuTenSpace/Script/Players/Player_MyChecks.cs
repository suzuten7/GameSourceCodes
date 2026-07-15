using Photon.Pun;
using UnityEngine;

public class Player_MyChecks : MonoBehaviourPun
{
    [SerializeField] GameObject[] MyObjs;
    [SerializeField] GameObject[] NoObjs;
    void Awake()
    {
        Start();
    }
    void Start()
    {
        for (int i = 0; i < MyObjs.Length; i++) MyObjs[i].gameObject.SetActive(photonView.IsMine);
        for (int i = 0; i < NoObjs.Length; i++) NoObjs[i].gameObject.SetActive(!photonView.IsMine);
    }
}
