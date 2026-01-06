using System;
using UnityEngine;

namespace CellCard
{
    /// <summary>
    /// 道具卡基类，只提供默认信息
    /// </summary>
    public abstract class ToolCard : MonoBehaviour, IToolCard
    {
        public ToolCardType toolType;
        public ToolCardTiming timing;

        public event Action OnSkillFinished;

        public virtual void StartSkill()
        {
            OnSkillFinished?.Invoke();
        }
    }
}
