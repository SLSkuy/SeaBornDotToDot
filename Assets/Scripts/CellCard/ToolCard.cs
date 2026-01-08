using System;
using UnityEngine;
using UnityEngine.UI;

namespace CellCard
{
    /// <summary>
    /// 道具卡基类，只提供默认信息
    /// </summary>
    public abstract class ToolCard : MonoBehaviour, IToolCard
    {
        public ToolCardType toolType;
        public ToolCardTiming timing;
        public Sprite icon;
        public string cardName;
        public string description;

        [SerializeField]private Image image;

        public event Action OnSkillFinished;

        public virtual void StartSkill()
        {
            OnSkillFinished?.Invoke();
        }

        private void Start()
        {
            if (icon != null)
            {
                image.sprite = icon;
            }
        }
    }
}
