using UnityEngine;

public class Net_Spawnes : MonoBehaviour
{
    static public Net_Spawnes NetSpawnes;
    public Transform[] Points;
    void Start()
    {
        NetSpawnes = this;
    }
}
