namespace GmSystem
{
    using Fusion;
    using Obj;
    using Player;
    using State;
    using System.Collections.Generic;
    using UnityEngine;

    public class GS_GlobalValues
    {
        static public List<GameObject> ParentList = new ();
        static public List<State_StateBase> StateList = new ();
        static public List<State_StateHit> StHitList = new ();
        static public List<State_StateBase> BossList = new ();
        static public List<Player_State> PStaList = new ();
        static public List<Player_State> MyPList = new ();

        static public List<State_Shot_Base> ShotList = new ();

        static public List<Obj_ActionObject> ActionObjs = new ();
        static public List<Obj_AreaView> CurrentAreas = new ();
        static public Obj_Shop CurrentShop = null;
        static public string SayName;
        static public string SayMessage;
        static public Texture SayIcon;
        static public Player_State MyPlayer = null;

        static public List<Vector2Int> CraftSets = new ();

        static float _ListRefleshTime = 0;
        static public void ListRefleshs()
        {
            _ListRefleshTime -= Time.fixedDeltaTime;
            if (_ListRefleshTime <= 0)
            {
                _ListRefleshTime = 5f;
                ParentList.RemoveAll(x => x == null);
                StateList.RemoveAll(x => x == null);
                StHitList.RemoveAll(x => x == null);
                BossList.RemoveAll(x => x == null);
                PStaList.RemoveAll(x => x == null);
                MyPList.RemoveAll(x => x == null);
                ShotList.RemoveAll(x => x == null);
                Debug.Log("リスト更新");
            }
        }
        static public void ParentStrage(GameObject obj, string parentName)
        {
            ParentList.RemoveAll(x => x == null);
            var parent = ParentList.Find(x => x.name == parentName);
            if (parent == null)
            {
                parent = new GameObject(parentName);
                ParentList.Add(parent);
            }
            obj.transform.parent = parent.transform;
        }
        static public Obj_AreaView AreaGet
        {
            get
            {
                Obj_AreaView ao = null;
                for (int i = 0; i < CurrentAreas.Count; i++)
                {
                    if (CurrentAreas[i] == null) continue;
                    if (ao == null || CurrentAreas[i].Order >= ao.Order)
                    {
                        ao = CurrentAreas[i];
                    }
                }
                return ao;
            }
        }
    }
}

