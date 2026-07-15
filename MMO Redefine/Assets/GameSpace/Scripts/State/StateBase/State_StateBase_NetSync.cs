namespace State
{
    using Fusion;
    using System.Linq;
    using UnityEngine;
    using static FNet.Fusion_Manager;
    public partial class State_StateBase
    {
        protected float _syncTic = 0;

        Class_State_CommonValues _bnt_CommonValues;
        #region Net用
        [Networked] NetworkString<_64> _net_Name { get; set; }
        [Networked] int _net_Team { get; set; }
        [Networked] int _net_LV { get; set; }
        #endregion
        Class_State_BaseValues _bnt_BaseValues;
        #region Net用
        [Networked] float _net_MHP { get; set; }
        [Networked] float _net_MMP { get; set; }
        [Networked] float _net_MST { get; set; }
        [Networked] float _net_PAtk { get; set; }
        [Networked] float _net_MAtk { get; set; }
        [Networked] float _net_PDef { get; set; }
        [Networked] float _net_MDef { get; set; }
        #endregion
        Class_State_ChangeValues _bnt_ChangeValues;
        #region Net用
        [Networked] float _net_HP { get; set; }
        [Networked] float _net_MP { get; set; }
        [Networked] float _net_ST { get; set; }
        [Networked] float _net_EX { get; set; }
        [Networked, Capacity(32)] NetworkLinkedList<Struct_Bufs> _net_Bufs => default;
        [Networked] int _net_DeathTic { get; set; }
        #endregion
        Class_State_AnimValues _bnt_AnimValues;
        #region Net用
        [Networked] int _net_Anim_MoveID { get; set; }
        [Networked] float _net_Anim_MoveSpeed { get; set; }
        [Networked] int _net_Anim_LAtkID { get; set; }
        [Networked] int _net_Anim_LAtkCo { get; set; }
        [Networked] float _net_Anim_LAtkSpeed { get; set; }
        [Networked] int _net_Anim_RAtkID { get; set; }
        [Networked] int _net_Anim_RAtkCo { get; set; }
        [Networked] float _net_Anim_RAtkSpeed { get; set; }
        [Networked] int _net_Anim_SAtkID { get; set; }
        [Networked] int _net_Anim_SAtkCo { get; set; }
        [Networked] float _net_Anim_SAtkSpeed { get; set; }
        [Networked] int _net_Anim_OtherID { get; set; }
        [Networked] float _net_Anim_OtherSpeed { get; set; }
        #endregion
        virtual protected bool NetServs()
        {
            if (InsRunner.GameMode == GameMode.Single)
            {
                //Debug.Log("オフライン");
                return false;
            }
            _syncTic -= Time.unscaledDeltaTime;
            if (_syncTic > 0) return false;
            _syncTic = Mathf.Max(0.1f, SettingValues.SyncTime);
            //Debug.Log("同期処理"+_syncTic + "秒間隔");
            if (_bnt_CommonValues == null) _bnt_CommonValues = new Class_State_CommonValues();
            if (_bnt_CommonValues.Name != CommonValues.Name ||
                _bnt_CommonValues.Team != CommonValues.Team ||
                _bnt_CommonValues.LV != CommonValues.LV)
            {
                _bnt_CommonValues.Name = CommonValues.Name;
                _bnt_CommonValues.Team = CommonValues.Team;
                _bnt_CommonValues.LV = CommonValues.LV;
                RPC_Serv_CommonVal(
                CommonValues.Name,
                (int)CommonValues.Team,
                CommonValues.LV
                );
            }
            if (_bnt_BaseValues == null) _bnt_BaseValues = new Class_State_BaseValues();
            if (_bnt_BaseValues.MHP != BaseValues.MHP ||
                _bnt_BaseValues.MMP != BaseValues.MMP ||
                _bnt_BaseValues.MST != BaseValues.MST ||
                _bnt_BaseValues.PAtk != BaseValues.PAtk ||
                _bnt_BaseValues.MAtk != BaseValues.MAtk ||
                _bnt_BaseValues.PDef != BaseValues.PDef ||
                _bnt_BaseValues.MDef != BaseValues.MDef)
            {
                _bnt_BaseValues.MHP = BaseValues.MHP;
                _bnt_BaseValues.MMP = BaseValues.MMP;
                _bnt_BaseValues.MST = BaseValues.MST;
                _bnt_BaseValues.PAtk = BaseValues.PAtk;
                _bnt_BaseValues.MAtk = BaseValues.MAtk;
                _bnt_BaseValues.PDef = BaseValues.PDef;
                _bnt_BaseValues.MDef = BaseValues.MDef;
                RPC_Serv_BaseVal(
             BaseValues.MHP,
             BaseValues.MMP,
             BaseValues.MST,
             BaseValues.PAtk,
             BaseValues.MAtk,
             BaseValues.PDef,
             BaseValues.MDef
             );
            }
            if (_bnt_ChangeValues == null) _bnt_ChangeValues = new Class_State_ChangeValues();
            var c_hpper = HP / Mathf.Max(1f, F_MHP) * 100f;
            var b_hpper = _bnt_ChangeValues.HP / Mathf.Max(1f, F_MHP) * 100f;
            bool hpifs = false;
            if (_bnt_ChangeValues.HP > 0 && HP <= 0) hpifs = true;
            else if (_bnt_ChangeValues.HP <= 0 && HP > 0) hpifs = true;
            else if (Mathf.Abs(c_hpper - b_hpper) >= 0.5f) hpifs = true;
            if (hpifs)
            {
                _bnt_ChangeValues.HP = HP;
                RPC_Serv_HPVal(HP);
            }
            if (_bnt_ChangeValues.MP != MP || _bnt_ChangeValues.ST != ST || _bnt_ChangeValues.EX != EX || _bnt_ChangeValues.DeathTic != ChangeValues.DeathTic)
            {
                _bnt_ChangeValues.MP = MP;
                _bnt_ChangeValues.ST = ST;
                _bnt_ChangeValues.EX = EX;
                _bnt_ChangeValues.DeathTic = ChangeValues.DeathTic;
                RPC_Serv_ChangeVal(
                MP,
                ST,
                EX,
                ChangeValues.DeathTic
                );
            }
            bool bufifs = false;
            if (_bnt_ChangeValues.Bufs.Count != ChangeValues.Bufs.Count) bufifs = true;
            else
            {
                for (int i = 0; i < _bnt_ChangeValues.Bufs.Count; i++)
                {
                    var bbuf = _bnt_ChangeValues.Bufs[i];
                    var cbuf = ChangeValues.Bufs[i];
                    if (bbuf.ID != cbuf.ID)
                    {
                        bufifs = true;
                        break;
                    }
                    if (bbuf.Index != cbuf.Index)
                    {
                        bufifs = true;
                        break;
                    }
                    if (bbuf.Op != cbuf.Op)
                    {
                        bufifs = true;
                        break;
                    }
                    if (bbuf.MTime != cbuf.MTime)
                    {
                        bufifs = true;
                        break;
                    }
                    if (bbuf.CPow != cbuf.CPow)
                    {
                        bufifs = true;
                        break;
                    }
                    if (bbuf.MPow != cbuf.MPow)
                    {
                        bufifs = true;
                        break;
                    }
                    if (Mathf.Abs(bbuf.CTime - cbuf.CTime) >= 2.5f)
                    {
                        bufifs = true;
                        break;
                    }
                }
            }
            if (bufifs)
            {
                _bnt_ChangeValues.Bufs = ChangeValues.Bufs.ToList();
                RPC_Serv_BufVal(ChangeValues.Bufs.ToArray());
            }
            if (_bnt_AnimValues == null) _bnt_AnimValues = new Class_State_AnimValues();
            if (_bnt_AnimValues.MoveID != AnimValues.MoveID ||
                _bnt_AnimValues.MoveSpeed != AnimValues.MoveSpeed)
            {
                _bnt_AnimValues.MoveID = AnimValues.MoveID;
                _bnt_AnimValues.MoveSpeed = AnimValues.MoveSpeed;
                RPC_Serv_AnimVal(0, AnimValues.MoveID, AnimValues.MoveSpeed, 0);
            }
            if (_bnt_AnimValues.LAtkID != AnimValues.LAtkID ||
                 _bnt_AnimValues.LAtkSpeed != AnimValues.LAtkSpeed ||
                 _bnt_AnimValues.LAtkCo != AnimValues.LAtkCo)
            {
                _bnt_AnimValues.LAtkID = AnimValues.LAtkID;
                _bnt_AnimValues.LAtkSpeed = AnimValues.LAtkSpeed;
                _bnt_AnimValues.LAtkCo = AnimValues.LAtkCo;
                RPC_Serv_AnimVal(1, AnimValues.LAtkID, AnimValues.LAtkSpeed, AnimValues.LAtkCo);
            }
            if (_bnt_AnimValues.RAtkID != AnimValues.RAtkID ||
                _bnt_AnimValues.RAtkSpeed != AnimValues.RAtkSpeed ||
                _bnt_AnimValues.RAtkCo != AnimValues.RAtkCo)
            {
                _bnt_AnimValues.RAtkID = AnimValues.RAtkID;
                _bnt_AnimValues.RAtkSpeed = AnimValues.RAtkSpeed;
                _bnt_AnimValues.RAtkCo = AnimValues.RAtkCo;
                RPC_Serv_AnimVal(2, AnimValues.RAtkID, AnimValues.RAtkSpeed, AnimValues.RAtkCo);
            }
            if (_bnt_AnimValues.SAtkID != AnimValues.SAtkID ||
                 _bnt_AnimValues.SAtkSpeed != AnimValues.SAtkSpeed ||
                 _bnt_AnimValues.SAtkCo != AnimValues.SAtkCo)
            {
                _bnt_AnimValues.SAtkID = AnimValues.SAtkID;
                _bnt_AnimValues.SAtkSpeed = AnimValues.SAtkSpeed;
                _bnt_AnimValues.SAtkCo = AnimValues.SAtkCo;
                RPC_Serv_AnimVal(3, AnimValues.SAtkID, AnimValues.SAtkSpeed, AnimValues.SAtkCo);
            }
            if (_bnt_AnimValues.OtherID != AnimValues.OtherID ||
                _bnt_AnimValues.OtherSpeed != AnimValues.OtherSpeed)
            {
                _bnt_AnimValues.OtherID = AnimValues.OtherID;
                _bnt_AnimValues.OtherSpeed = AnimValues.OtherSpeed;
                RPC_Serv_AnimVal(4, AnimValues.OtherID, AnimValues.OtherSpeed, 0);
            }
            return true;
        }
        protected virtual void NetsLocalSet()
        {
            CommonValues.Name = _net_Name.Value;
            CommonValues.Team = (Enum_Team)_net_Team;
            CommonValues.LV = _net_LV;

            BaseValues.MHP = _net_MHP;
            BaseValues.MMP = _net_MMP;
            BaseValues.MST = _net_MST;
            BaseValues.PAtk = _net_PAtk;
            BaseValues.MAtk = _net_MAtk;
            BaseValues.PDef = _net_PDef;
            BaseValues.MDef = _net_MDef;

            HP = _net_HP;
            MP = _net_MP;
            ST = _net_ST;
            ChangeValues.DeathTic = _net_DeathTic;
            ChangeValues.Bufs = _net_Bufs.ToList();

            AnimValues.MoveID = _net_Anim_MoveID;
            AnimValues.MoveSpeed = _net_Anim_MoveSpeed;
            AnimValues.LAtkID = _net_Anim_LAtkID;
            AnimValues.LAtkSpeed = _net_Anim_LAtkSpeed;
            AnimValues.LAtkCo = _net_Anim_LAtkCo;
            AnimValues.RAtkID = _net_Anim_RAtkID;
            AnimValues.RAtkSpeed = _net_Anim_RAtkSpeed;
            AnimValues.RAtkCo = _net_Anim_RAtkCo;
            AnimValues.SAtkID = _net_Anim_SAtkID;
            AnimValues.SAtkSpeed = _net_Anim_SAtkSpeed;
            AnimValues.SAtkCo = _net_Anim_SAtkCo;
            AnimValues.OtherID = _net_Anim_OtherID;
            AnimValues.OtherSpeed = _net_Anim_OtherSpeed;
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Serv_CommonVal(NetworkString<_64> syName, int syTeam, int syLV)
        {
            _net_Name = syName;
            _net_Team = syTeam;
            _net_LV = syLV;
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Serv_BaseVal(float syMHP, float syMMP, float syMST, float syPAtk, float syMAtk, float syPDef, float syMDef)
        {
            _net_MHP = syMHP;
            _net_MMP = syMMP;
            _net_MST = syMST;
            _net_PAtk = syPAtk;
            _net_MAtk = syMAtk;
            _net_PDef = syPDef;
            _net_MDef = syMDef;
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Serv_HPVal(float syHP)
        {
            _net_HP = syHP;
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Serv_ChangeVal(float syMP, float syST, float syEX, int syDeathTic)
        {
            _net_MP = syMP;
            _net_ST = syST;
            _net_EX = syEX;
            _net_DeathTic = syDeathTic;
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Serv_BufVal(Struct_Bufs[] syBufs)
        {
            _net_Bufs.Clear();
            foreach (var sbuf in syBufs) _net_Bufs.Add(sbuf);
        }
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_Serv_AnimVal(byte Type, int syID, float sySpeed, int syCo)
        {
            switch (Type)
            {
                case 0:
                    _net_Anim_MoveID = syID;
                    _net_Anim_MoveSpeed = sySpeed;
                    break;
                case 1:
                    _net_Anim_LAtkID = syID;
                    _net_Anim_LAtkSpeed = sySpeed;
                    _net_Anim_LAtkCo = syCo;
                    break;
                case 2:
                    _net_Anim_RAtkID = syID;
                    _net_Anim_RAtkSpeed = sySpeed;
                    _net_Anim_RAtkCo = syCo;
                    break;
                case 3:
                    _net_Anim_SAtkID = syID;
                    _net_Anim_SAtkSpeed = sySpeed;
                    _net_Anim_SAtkCo = syCo;
                    break;
                case 4:
                    _net_Anim_OtherID = syID;
                    _net_Anim_OtherSpeed = sySpeed;
                    break;
            }
        }

    }
}

