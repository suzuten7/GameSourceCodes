using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "DataCre/PhotonPrefabAdd")]
public class Net_PhotonPrefabAdd : ScriptableObject
{
    [Header("同期プレファブ")] public GameObject[] PrefabAdds;
}
