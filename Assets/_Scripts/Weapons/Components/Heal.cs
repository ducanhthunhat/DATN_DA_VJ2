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
                stats.Health.Increase(data.Amount);
                CurrentCharges--;
                
                OnChargesChanged?.Invoke(CurrentCharges, data.MaxCharges);
                
                Debug.Log($"Healed for {data.Amount}. Charges left: {CurrentCharges}/{data.MaxCharges}");
            }
            else if (CurrentCharges <= 0)
            {
                Debug.Log("No healing charges left!");
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
