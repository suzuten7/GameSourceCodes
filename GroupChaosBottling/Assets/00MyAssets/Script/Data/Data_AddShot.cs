using UnityEngine;
using static Manifesto;
[CreateAssetMenu(menuName ="DataCre/AddShot")]
public class Data_AddShot : ScriptableObject
{
    public Class_Atk_Shot_Base[] Shots;
    public Class_Atk_SEPlay[] SEPlays;
}
