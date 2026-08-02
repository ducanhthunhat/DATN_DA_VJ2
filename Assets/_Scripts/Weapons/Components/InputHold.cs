using UnityEngine;

namespace DucAnh.Weapons.Components
{
    public class InputHold : WeaponComponent
    {
        private Animator anim;

        private bool input;

        private bool minHoldPassed;

        protected override void HandleEnter()
        {
            base.HandleEnter();

            minHoldPassed = false;
            
            // Đồng bộ lại input hiện tại ngay khi bắt đầu đòn đánh mới
            input = weapon.CurrentInput;
            SetAnimatorParameter();
        }

        protected override void HandleExit()
        {
            base.HandleExit();
            
            // Ép buộc reset biến hold về false khi vũ khí bị ngắt (ví dụ: người chơi bị choáng)
            anim.SetBool("hold", false);
        }

        private void HandleCurrentInputChange(bool newInput)
        {
            input = newInput;

            SetAnimatorParameter();
        }

        private void HandleMinHoldPassed()
        {
            minHoldPassed = true;

            SetAnimatorParameter();
        }

        private void SetAnimatorParameter()
        {
            if (input)
            {
                anim.SetBool("hold", input);
                return;
            }

            if (minHoldPassed)
            {
                anim.SetBool("hold", false);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            anim = GetComponentInChildren<Animator>();

            weapon.OnCurrentInputChange += HandleCurrentInputChange;
            AnimationEventHandler.OnMinHoldPassed += HandleMinHoldPassed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            weapon.OnCurrentInputChange -= HandleCurrentInputChange;
            AnimationEventHandler.OnMinHoldPassed -= HandleMinHoldPassed;
        }
    }
}
