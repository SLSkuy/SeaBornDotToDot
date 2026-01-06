using System;
using UnityEngine;

namespace CellCard
{
    public class ToolCard : MonoBehaviour, IToolCard
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
