using System;

namespace CellCard
{
    public interface IToolCard
    {
        public event Action OnSkillFinished;
        
        public void StartSkill();
    }
}