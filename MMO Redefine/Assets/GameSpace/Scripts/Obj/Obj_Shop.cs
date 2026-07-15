
namespace Obj
{
    using Datas;
    using Player;
    using UIs;
    using static GmSystem.GS_GlobalValues;
    public class Obj_Shop : Obj_ActionObject
    {
        public Data_Shop ShopD;
        public override void PlayAction(Player_State PSta)
        {
            CurrentShop = this;
            UIs.UI_System.ShopOpen();
        }
    }
}
