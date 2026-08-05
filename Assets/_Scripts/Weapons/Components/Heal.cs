using UnityEngine;
using DucAnh.CoreSystem;
using System;

namespace DucAnh.Weapons.Components
{
    public class Heal : WeaponComponent
    {
        private HealData data;
        private Stats stats;
        
        public int CurrentCharges { get; private set; }

        public event Action<int, int> OnChargesChanged;

        public override void Init()
        {
            base.Init();
            data = weapon.Data.GetData<HealData>();
        }

        protected override void Start()
        {
            base.Start();
            
            stats = Core.GetCoreComponent<Stats>();
            
            if (data != null)
            {
                CurrentCharges = data.MaxCharges;
            }
        }

        protected override void HandleEnter()
        {
            base.HandleEnter();
        }

        private void HandleAttackAction()
        {
            if (CurrentCharges > 0 && stats.Health.CurrentValue < stats.Health.MaxValue)
            {
                CurrentCharges--;
                OnChargesChanged?.Invoke(CurrentCharges, data.MaxCharges);
                
                // Thay vì cộng 1 cục, gọi Coroutine để bơm máu từ từ
                StartCoroutine(HealGradually(data.Amount));
                
                Debug.Log($"Started healing for {data.Amount}. Charges left: {CurrentCharges}/{data.MaxCharges}");
            }
            else if (CurrentCharges <= 0)
            {
                Debug.Log("No healing charges left!");
            }
        }

        private System.Collections.IEnumerator HealGradually(float totalAmount)
        {
            float healedSoFar = 0f;
            int totalTicks = 30; // Chia làm 30 cục nhỏ
            float healPerTick = totalAmount / totalTicks;
            WaitForSeconds wait = new WaitForSeconds(0.05f); // Mỗi 0.05 giây nhảy 1 cục (tổng cộng mất 1.5 giây)

            for (int i = 0; i < totalTicks; i++)
            {
                if (stats.Health.CurrentValue >= stats.Health.MaxValue)
                {
                    break;
                }

                stats.Health.Increase(healPerTick);
                healedSoFar += healPerTick;
                yield return wait;
            }
            
            // Bơm bù nốt phần lẻ nếu có sai số
            if (stats.Health.CurrentValue < stats.Health.MaxValue && healedSoFar < totalAmount)
            {
                stats.Health.Increase(totalAmount - healedSoFar);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            AnimationEventHandler.OnAttackAction += HandleAttackAction;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            AnimationEventHandler.OnAttackAction -= HandleAttackAction;
        }
    }
}
