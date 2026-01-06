using EventProcess;
using GameProcessor;
using UIFramework;

namespace UI.GameSceneUI
{
    public class GameSceneUIManager : ASceneUIManager
    {
        protected override void AddSignal()
        {
            Signals.Get<ExitShop>().AddListener(ExitShop);

            GameManager.Instance.OnShopTime += ShowShop;
        }

        protected override void RemoveSignal()
        {
            Signals.Get<ExitShop>().RemoveListener(ExitShop);
            
            GameManager.Instance.OnShopTime -= ShowShop;
        }

        private void ExitShop()
        {
            UIFrame.HideUI("ShopWindow");
        }

        private void ShowShop()
        {
            UIFrame.ShowUI("ShopWindow");
        }
    }
}