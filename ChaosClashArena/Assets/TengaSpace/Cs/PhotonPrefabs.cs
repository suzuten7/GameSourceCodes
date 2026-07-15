using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
[CreateAssetMenu(menuName = "SuzutenDCre/PhotonPrefabs")]
public class PhotonPrefabs : ScriptableObject, IPunPrefabPool
{
    [SerializeField] GameObject[] PrefabObjs;
    [SerializeField] PhotonPrefabAdd[] PrefabAdd;
    static Dictionary<string, GameObject> ObjDic = new Dictionary<string, GameObject>();
    public void PrefabPoolSet()
    {
        PhotonNetwork.PrefabPool = this;
        StrSets();
    }

    GameObject IPunPrefabPool.Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        var go = Instantiate(ObjDic[prefabId], position, rotation);
        go.SetActive(false);
        return go;

    }

    void IPunPrefabPool.Destroy(GameObject gameObject)
    {
        Destroy(gameObject);
    }

    void StrSets()
    {
        ObjDic.Clear();
        for(int i = 0; i < PrefabObjs.Length; i++)
        {
            GameObject Obj = PrefabObjs[i];
            if (Obj != null) ObjDic.TryAdd(Obj.name, Obj);
        }
        for (int i = 0; i < PrefabAdd.Length; i++)
        {
            var Adds = PrefabAdd[i];
            if (Adds == null) continue;
            for (int j = 0; j < Adds.PrefabAdds.Length; j++)
            {
                GameObject Obj = Adds.PrefabAdds[j];
                if (Obj != null) ObjDic.TryAdd(Obj.name, Obj);
            }
        }
    }
}

