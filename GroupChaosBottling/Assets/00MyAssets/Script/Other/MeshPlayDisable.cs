using UnityEngine;

public class MeshPlayDisable : MonoBehaviour
{
    [SerializeField] MeshRenderer[] Meshs;
    void Start()
    {
        for(int i = 0; i < Meshs.Length; i++)
        {
            if (Meshs[i] != null) Meshs[i].enabled = false;
        }
    }
}
