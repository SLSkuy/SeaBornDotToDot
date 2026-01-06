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
            UIFrame.HideUI("ShopPanel");
            UIFrame.ShowUI("ScorePanel");
            UIFrame.HideUI("ShopTitlePanel");
        }

        private void ShowShop()
        {
            Invoke(nameof(ShowShopDelay), 1.5f);
            
            UIFrame.HideUI("ScorePanel");
            UIFrame.ShowUI("ShopTitlePanel");
        }
        
        private void ShowShopDelay() => UIFrame.ShowUI("ShopPanel");
    }
}