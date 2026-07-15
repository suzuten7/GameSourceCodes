using UnityEngine;

public class Obj_SpawneArea : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    public int teamID;
    private void Start()
    {
        Obj_LocalObjects.Spawnes.Add(this);
    }
    private void Update()
    {
        var col = Data_Base.TeamColorGet(teamID);
        col.a = sr.color.a;
        sr.color = col;
    }
}
