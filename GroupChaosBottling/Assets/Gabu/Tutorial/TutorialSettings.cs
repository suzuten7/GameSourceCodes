using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "Tutorial", menuName = "Gabu/Tutorial/Tutorial")]
public class TutorialSettings : ScriptableObject
{
    public string tutorialName;
    public Texture icon;
    [TextArea(3, 10)]
    public string tutorialDescription;
    public Texture[] images;
}
