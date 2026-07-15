using TMPro;
using UnityEngine;

public class UI_TeamSet : MonoBehaviour
{
    static public int teamID;
    [SerializeField] TMP_Dropdown teamDr;
    void Update()
    {
        teamDr.value = teamID;
    }
    public void TeamSet()
    {
        teamID = teamDr.value;
    }
}
