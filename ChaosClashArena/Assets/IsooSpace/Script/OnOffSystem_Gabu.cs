using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffSystem_Gabu : MonoBehaviour
{
    public void OnOffObject(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
    }
}
