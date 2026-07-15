using Photon.Pun;
using UnityEngine;

public class TestObjSetter : MonoBehaviour
{
    [SerializeField] GameObject[] Enemys;
    [SerializeField] int StartEnemyCount;
    [SerializeField] int AddEnemyCount;
    [SerializeField] int AddCTs;
    [SerializeField] Vector3 SetRanges;
    int CT = 0;
    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        CT = AddCTs;
        for(int i = 0; i < StartEnemyCount; i++)Insta();
        
    }
    private void FixedUpdate()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        CT--;
        if (CT <= 0)
        {
            CT = AddCTs;
            for (int i = 0; i < AddEnemyCount; i++) Insta();
        }
    }
    void Insta()
    {
        var Pos = transform.position;
        var RandV = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized;
        Pos += new Vector3(RandV.x * Random.Range(0, SetRanges.x), RandV.y * Random.Range(0, SetRanges.y), RandV.z * Random.Range(0, SetRanges.z));
        PhotonNetwork.Instantiate(Enemys[Random.Range(0, Enemys.Length)].name, Pos, Quaternion.Euler(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f)));
    }
}
