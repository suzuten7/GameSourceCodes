using UnityEngine;
[CreateAssetMenu(fileName = "ReticlesData", menuName = "ScriptableObjects/Reticles")]
public class Data_Reticles : ScriptableObject
{
    public string name;
    [TextArea]public string info;
    public Texture icon;
    public Color iconCol;
    public GameObject reticleObj;
}
