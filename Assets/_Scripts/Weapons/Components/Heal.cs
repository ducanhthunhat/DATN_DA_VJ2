using UnityEngine;
using DucAnh.CoreSystem;
using System;

namespace DucAnh.Weapons.Components
{
    public class Heal : WeaponComponent
    {
        public HealData data { get; private set; }
        private Stats stats;
        
        public int CurrentCharges { get; private set; }

        public event Action<int, int> OnChargesChanged;

        private Coroutine healCoroutine;
        private bool minHoldPassed;
        private bool hasStartedHealing;

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
            
            minHoldPassed = false;
            hasStartedHealing = false;

            if (CurrentCharges > 0 && stats.Health.CurrentValue < stats.Health.MaxValue)
            {
                healCoroutine = StartCoroutine(HealGradually(data.Amount));
            }
            else 
            {
                if (CurrentCharges <= 0)
                {
                    Debug.Log("No healing charges left!");
                }
                else
                {
                    Debug.Log("Health is already full!");
                }
                
                // Máu đầy hoặc hết bình thì lập tức thu hồi phím để nhảy thẳng sang Cancel
                weapon.EventHandler.UseInputTrigger();
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
            
            minHoldPassed = false;
        }

        private void HandleCurrentInputChange(bool input)
        {
            // Nếu người chơi nhả phím X -> Ngừng bơm máu ngay lập tức
            if (!input && healCoroutine != null)
            {
                StopCoroutine(healCoroutine);
                healCoroutine = null;
            }
        }

        private void HandleMinHoldPassed()
        {
            minHoldPassed = true;
        }

        private System.Collections.IEnumerator HealGradually(float totalAmount)
        {
            // Đợi đến khi quá trình gồng Anticipation kết thúc thành công (không bị cancel sớm)
            yield return new WaitUntil(() => minHoldPassed);

            hasStartedHealing = true;
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

            // Hồi máu xong thì tự động ngắt (thu hồi phím X) để cất bình đi nếu vẫn đang cầm
            weapon.EventHandler.UseInputTrigger();
        }

        protected override void Awake()
        {
            base.Awake();
            AnimationEventHandler.OnMinHoldPassed += HandleMinHoldPassed;
            weapon.OnCurrentInputChange += HandleCurrentInputChange;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            AnimationEventHandler.OnMinHoldPassed -= HandleMinHoldPassed;
            weapon.OnCurrentInputChange -= HandleCurrentInputChange;
        }
    }
}
