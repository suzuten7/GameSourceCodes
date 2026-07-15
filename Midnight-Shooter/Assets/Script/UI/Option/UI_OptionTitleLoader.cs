using UnityEngine;

public class UI_OptionTitleLoader : MonoBehaviour
{
    [SerializeField] UI_OptionManager OptionManager;
    void Start()
    {
        OptionManager.SetUp();
    }
}
