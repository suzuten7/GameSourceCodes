using TMPro;
using UnityEngine;

public class UI_Version : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI VerTx;
    void Start()
    {
        VerTx.text = "Ver" + Application.version;
    }
}
