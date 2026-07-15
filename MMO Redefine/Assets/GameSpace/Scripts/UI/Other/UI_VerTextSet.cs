namespace UIs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_VerTextSet : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI verTx;
        void Start()
        {
            verTx.text = "Ver." + Application.version;
        }

    }
}
