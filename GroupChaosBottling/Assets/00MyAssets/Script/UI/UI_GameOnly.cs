using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_GameOnly : MonoBehaviour
{
    [SerializeField] GameObject[] OnlyUIs;
    void Update()
    {
        var SceneID = SceneManager.GetActiveScene().buildIndex;
        for (int i = 0; i < OnlyUIs.Length; i++) OnlyUIs[i].SetActive(SceneID > 0);
    }
}
