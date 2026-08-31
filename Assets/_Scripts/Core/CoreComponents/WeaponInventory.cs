using System;
using DucAnh.Weapons;
using UnityEngine;

namespace DucAnh.CoreSystem
{
    public class WeaponInventory : CoreComponent
    {
        public event Action<int, WeaponDataSO> OnWeaponDataChanged;

        [SerializeField] private WeaponDataSO[] _weaponData;
        public WeaponDataSO[] weaponData => _weaponData;

        // Lưu vũ khí ban đầu để reset khi player die
        private WeaponDataSO[] _initialWeaponData;

        protected override void Awake()
        {
            base.Awake();

            // Sao chép mảng vũ khí ban đầu (từ Inspector) để có thể khôi phục sau
            _initialWeaponData = new WeaponDataSO[_weaponData.Length];
            System.Array.Copy(_weaponData, _initialWeaponData, _weaponData.Length);
        }

        /// <summary>
        /// Reset vũ khí về trạng thái ban đầu (khi player die/restart).
        /// </summary>
        public void ResetToDefault()
        {
            System.Array.Copy(_initialWeaponData, _weaponData, _initialWeaponData.Length);

            // Thông báo cho tất cả WeaponGenerator cập nhật lại vũ khí
            for (int i = 0; i < _weaponData.Length; i++)
            {
                OnWeaponDataChanged?.Invoke(i, _weaponData[i]);
            }
        }

        public bool TrySetWeapon(WeaponDataSO newData, int index, out WeaponDataSO oldData)
        {
            if (index >= weaponData.Length)
            {
                oldData = null;
                return false;
            }

            oldData = weaponData[index];
            weaponData[index] = newData;

            OnWeaponDataChanged?.Invoke(index, newData);

            return true;
        }

        public bool TryGetWeapon(int index, out WeaponDataSO data)
        {
            if (index >= weaponData.Length)
            {
                data = null;
                return false;
            }

            data = weaponData[index];
            return true;
        }

        public bool TryGetEmptyIndex(out int index)
        {
            for (var i = 0; i < weaponData.Length; i++)
            {
                if (weaponData[i] is not null)
                    continue;

                index = i;
                return true;
            }

            index = -1;
            return false;
        }

        public WeaponSwapChoice[] GetWeaponSwapChoices()
        {
            var choices = new WeaponSwapChoice[weaponData.Length];

            for (var i = 0; i < weaponData.Length; i++)
            {
                var data = weaponData[i];

                choices[i] = new WeaponSwapChoice(data, i);
            }

            return choices;
        }
    }
}
