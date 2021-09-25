using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public abstract class BaseAbility : MonoBehaviour
    {
        [SerializeField]
        protected float Duration = 5.0f;
        
        public readonly UnityEvent<float> OnCooldownStart = new UnityEvent<float>();
        
        private bool bCanExecute = true;
        private bool bInCooldown;

        public virtual bool Execute()
        {
            if (!CanExecute()) return false;
            ResourceManager.ConsumeStack();
            bInCooldown = true;
            OnCooldownStart.Invoke(Duration);
            Invoke(nameof(ResetCooldown), Duration);
            return true;
        }

        public virtual bool CanExecute()
        {
            bCanExecute = ResourceManager.GetCurrentStack() > 0;
            return !bInCooldown && bCanExecute;
        }

        private void ResetCooldown()
        {
            bInCooldown = false;
        }
    }
}