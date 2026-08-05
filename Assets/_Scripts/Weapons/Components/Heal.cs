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

        private Coroutine healCoroutine;

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

            if (CurrentCharges > 0 && stats.Health.CurrentValue < stats.Health.MaxValue)
            {
                healCoroutine = StartCoroutine(HealGradually(data.Amount));
            }
            else if (CurrentCharges <= 0)
            {
                Debug.Log("No healing charges left!");
            }
        }

        protected override void HandleExit()
        {
            base.HandleExit();

            if (healCoroutine != null)
            {
                StopCoroutine(healCoroutine);
                healCoroutine = null;
            }
        }

        private System.Collections.IEnumerator HealGradually(float totalAmount)
        {
            // Đợi 0.6 giây để chạy xong clip Anticipation (chờ cái khiên bubble xanh hiện ra)
            yield return new WaitForSeconds(0.6f);

            CurrentCharges--;
            OnChargesChanged?.Invoke(CurrentCharges, data.MaxCharges);
            Debug.Log($"Started healing for {totalAmount}. Charges left: {CurrentCharges}/{data.MaxCharges}");

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

            // Hồi máu xong thì tự động ngắt (thu hồi phím X) để cất bình đi
            weapon.EventHandler.UseInputTrigger();
        }
    }
}
