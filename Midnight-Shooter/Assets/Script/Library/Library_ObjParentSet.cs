using System.Collections.Generic;
using UnityEngine;

/* 内容
 * ・Prefabの親Objの自動生成
 * ・Prefabのセット
 */

public class Library_ObjParentSet : MonoBehaviour
{
    static Dictionary<string, GameObject> parents = new Dictionary<string, GameObject>();

    static public void ParentSet(GameObject obj,string key)
    {
        if (!parents.ContainsKey(key))
        {
            parents.Add(key,new GameObject(key));
        }
        if (parents[key] == null) parents[key] = new GameObject(key);
        obj.transform.SetParent(parents[key].transform);
    }
}
