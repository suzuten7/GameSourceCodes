using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sin_Message : MonoBehaviour
{
    public Image BackUI;
    public TextMeshProUGUI SendTx;
    public TextMeshProUGUI MessageTx;
    public float DelTime;
    float times = 0;
    private void Update()
    {
        if (DelTime <= 0) return;
        times += Time.fixedDeltaTime;
        float Alpha = 1f - (times / DelTime);
        Color BackCol = BackUI.color;
        BackCol.a = Alpha * 0.5f;
        BackUI.color = BackCol;
        Color TxCol = MessageTx.color;
        TxCol.a = Alpha;
        SendTx.color = TxCol;
        MessageTx.color = TxCol;
        if (times >= DelTime)Destroy(gameObject);
    }
}
