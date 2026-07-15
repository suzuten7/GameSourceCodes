namespace Player
{

    using State;
    using UnityEngine;
    using UIs;
    public partial class Player_State : State_StateBase
    {
        [Header("プレイヤーステータス")]
        public Class_State_PlayerValues PlayerValues;

        public int BotID;
        public int CharaID;
        public Player_Controle Cont;
        public Player_ModelSet ModelSet;
        public Camera PlCamera;

        public UI_ValueGraph_Main ValGraph;
        [SerializeField] Vector2 SpawneRange;
        public float RevTime;
        [SerializeField] float AutoSaveCT;

        public State_StateHit TargetHit;
        bool _starts = false;
        int _autoSaveT;



    }
}
