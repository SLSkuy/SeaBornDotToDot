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

            GameManager.Instance.OnGameOver += ShowGameOver;
            GameManager.Instance.OnRoundChange += ShowRoundView;
            GameManager.Instance.OnShopTime += ShowShop;
        }

        protected override void RemoveSignal()
        {
            Signals.Get<ExitShop>().RemoveListener(ExitShop);

            GameManager.Instance.OnGameOver -= ShowGameOver;
            GameManager.Instance.OnRoundChange -= ShowRoundView;
            GameManager.Instance.OnShopTime -= ShowShop;
        }

        private void ShowGameOver()
        {
            UIFrame.ShowUI("GameOverPanel");
        }

        private void ShowRoundView(string text)
        {
            UIFrame.ShowUI("RoundViewPanel", new RoundViewPanelProp(text));
            
            Invoke(nameof(HideRoundView), 1f);
        }

        private void HideRoundView()
        {
            UIFrame.HideUI("RoundViewPanel");
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