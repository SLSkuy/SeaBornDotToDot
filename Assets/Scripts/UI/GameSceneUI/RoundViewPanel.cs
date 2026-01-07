using EventProcess;
using UIFramework.Panel;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameSceneUI
{
    public class RoundViewPanelShow : ASignal<string> { }
    public class RoundViewPanelHide : ASignal { }

    public class RoundViewPanelProp : PanelProperties
    {
        public string Content;
        
        public RoundViewPanelProp(string content, PanelPriority priority = PanelPriority.Blocker) : base(priority)
        {
            Content = content;
        }
    }
    
    public class RoundViewPanel : PanelController
    {
        [Header("显示文本")] 
        public Text text;

        protected override void SetProperties(PanelProperties props)
        {
            if (props is RoundViewPanelProp)
            {
                var prop = (RoundViewPanelProp)props;
                text.text = prop.Content;
            }
            
            base.SetProperties(props);
        }
    }
}
