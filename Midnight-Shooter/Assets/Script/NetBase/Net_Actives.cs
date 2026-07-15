using UnityEngine;
using Fusion;
public class Net_Actives : NetworkBehaviour
{
    [SerializeField] GameObject[] OwnerObjs;
    [SerializeField] GameObject[] OtherObjs;
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
        var owner = Net_Connect.CanControl(Object);
        for (int i = 0; i < OwnerObjs.Length; i++)
        {
            if (OwnerObjs[i] == null) continue;
            if (OwnerObjs[i].activeSelf != owner)OwnerObjs[i].SetActive(owner);
        }
        for (int i = 0; i < OtherObjs.Length; i++)
        {
            if (OtherObjs[i] == null) continue;
            if (OtherObjs[i].activeSelf == owner) OtherObjs[i].SetActive(!owner);
        }
    }
}
