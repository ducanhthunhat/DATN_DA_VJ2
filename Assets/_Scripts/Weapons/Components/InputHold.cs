using UnityEngine;

namespace DucAnh.Weapons.Components
{
    public class InputHold : WeaponComponent
    {
        private Animator anim;

        private bool input;

        private bool minHoldPassed;

        private int holdHash;
        private int cancelHash;
        private bool hasHoldParam;
        private bool hasCancelParam;

        private RuntimeAnimatorController lastController;

        public override void Init()
        {
            base.Init();
        }

        protected override void HandleEnter()
        {
            base.HandleEnter();

            UpdateParameterCache();

            minHoldPassed = false;
            
            // Đảm bảo reset biến cancel mỗi khi bắt đầu đòn đánh
            if (hasCancelParam) anim.SetBool(cancelHash, false);
            
            // Đồng bộ lại input hiện tại ngay khi bắt đầu đòn đánh mới
            input = weapon.CurrentInput;
            SetAnimatorParameter();
        }

        protected override void HandleExit()
        {
            base.HandleExit();
            
            // Áp buộc reset biến hold và cancel về false khi vũ khí bị ngắt (ví dụ: người chơi bị choáng)
            if (hasHoldParam) anim.SetBool(holdHash, false);
            if (hasCancelParam) anim.SetBool(cancelHash, false);
        }

        private void HandleCurrentInputChange(bool newInput)
        {
            input = newInput;
            SetAnimatorParameter();
        }

        private void UpdateParameterCache()
        {
            // Chỉ chạy vòng lặp kiểm tra lại nếu Animator Controller bị đổi vũ khí (để tránh giật lag do rác bộ nhớ)
            if (anim.runtimeAnimatorController == lastController) return;
            
            lastController = anim.runtimeAnimatorController;
            
            hasHoldParam = false;
            hasCancelParam = false;
            
            foreach (var param in anim.parameters)
            {
                if (param.nameHash == holdHash) hasHoldParam = true;
                if (param.nameHash == cancelHash) hasCancelParam = true;
            }
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
                if (hasHoldParam) anim.SetBool(holdHash, input);
                return;
            }

            if (minHoldPassed)
            {
                if (hasHoldParam) anim.SetBool(holdHash, false);
            }
            else
            {
                if (hasCancelParam) anim.SetBool(cancelHash, true);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            anim = GetComponentInChildren<Animator>();

            holdHash = Animator.StringToHash("hold");
            cancelHash = Animator.StringToHash("cancel");

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
