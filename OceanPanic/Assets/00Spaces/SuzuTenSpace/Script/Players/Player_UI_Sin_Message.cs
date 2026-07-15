using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player_UI_Sin_Message : MonoBehaviour
{
    public Image BackImages;
    public TextMeshProUGUI Texts;
    int RemTimes = 600;
    int AlphaTimes = 400;
    float AlphaBase = 0.6f;
    int Times = 0;
    private void FixedUpdate()
    {
        Times++;
        float TimeA = 1f;
        if (Times >= AlphaTimes)TimeA = 1f - ((float)(Times - AlphaTimes) / (RemTimes - AlphaTimes));

        var BackCol = BackImages.color;
        BackCol.a = AlphaBase * TimeA * 0.5f;
        BackImages.color = BackCol;
        var TextColor = Texts.color;
        TextColor.a = AlphaBase * TimeA;
        Texts.color = TextColor;

        if (Times >= RemTimes) Destroy(gameObject);
    }
}
