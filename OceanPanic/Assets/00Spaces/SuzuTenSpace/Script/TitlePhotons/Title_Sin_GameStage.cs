using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Title_Sin_GameStage : MonoBehaviour
{
    [SerializeField] TitleUIs Titles;
    public RawImage StageImage;
    public TextMeshProUGUI StageNameTx;
    public int ID;

    public void StageSet()
    {
        Titles.RoomSetsUIStageIDSet(ID);
    }
}
