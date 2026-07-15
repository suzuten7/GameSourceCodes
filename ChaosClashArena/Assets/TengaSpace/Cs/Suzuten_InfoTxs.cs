
using UnityEngine;
using TMPro;
public class Suzuten_InfoTxs : MonoBehaviour
{
    public TextMeshProUGUI Tx;
    public RectTransform RT;
    public float AlphaSpeed = 1;
    public float MoveSpeed = 1;
    void FixedUpdate()
    {
        RT.localPosition += new Vector3(0,MoveSpeed*0.1f, 0);
        Color col = Tx.color;
        col.a -= AlphaSpeed*0.001f;
        Tx.color = col;
        if (col.a <= 0) Destroy(gameObject);
    }
}
