using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Title_Sin_GameOption : MonoBehaviour
{
    [SerializeField] TitleUIs Titles;
    public TextMeshProUGUI OptionNameTx;
    public TextMeshProUGUI OptionOnOffTx;
    public Image OnOffButtonImage;
    public int ID;

    public void OnOffCh()
    {
        Titles.RoomSetsUIOptionSet(ID);
    }
}
