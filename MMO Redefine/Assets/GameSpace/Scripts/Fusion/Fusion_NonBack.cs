namespace FNet
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    public class Fusion_NonBack : MonoBehaviour
    {
        [SerializeField] int SceneID;
        void Update()
        {
            if (Fusion_Manager.FMananger== null)SceneManager.LoadScene(SceneID);
        }
    }
}

