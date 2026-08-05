using System;
using DucAnh.CoreSystem;
using DucAnh.Weapons;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DucAnh.UI
{
    public class EquippedWeaponUI : MonoBehaviour
    {
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TextMeshProUGUI chargesText;

        [SerializeField] private CombatInputs input;
        [SerializeField] private WeaponInventory weaponInventory;

        private WeaponDataSO weaponData;
        private DucAnh.Weapons.Components.Heal currentHealComponent;
        private string weaponObjectName;
        
        private void SetWeaponIcon()
        {
            weaponIcon.sprite = weaponData ? weaponData.Icon : null;
            weaponIcon.color = weaponData ? Color.white : Color.clear;
        }

        private void Update()
        {
            if (weaponData == null && weaponIcon.color.a > 0f)
            {
                // Ép nó tàng hình liên tục mỗi khung hình để chống lại Animator/Script khác
                weaponIcon.color = Color.clear;
            }
        }

        private void BindChargesEvent()
        {
            if (chargesText == null) return;
            
            if (currentHealComponent != null)
            {
                currentHealComponent.OnChargesChanged -= UpdateChargesText;
            }

            if (weaponObjectName == null)
            {
                weaponObjectName = input.ToString().Substring(0, 1).ToUpper() + input.ToString().Substring(1) + "Weapon";
            }

            var player = GameObject.Find("Player");
            if (player != null)
            {
                var weaponTransform = player.transform.Find("Core/" + weaponObjectName) ?? player.transform.Find(weaponObjectName);
                if (weaponTransform != null)
                {
                    currentHealComponent = weaponTransform.GetComponentInChildren<DucAnh.Weapons.Components.Heal>();
                    if (currentHealComponent != null)
                    {
                        currentHealComponent.OnChargesChanged += UpdateChargesText;
                        // Gọi 1 lần đầu tiên để update UI
                        if (currentHealComponent.data != null)
                        {
                            UpdateChargesText(currentHealComponent.CurrentCharges, currentHealComponent.data.MaxCharges);
                        }
                        chargesText.gameObject.SetActive(true);
                        return;
                    }
                }
            }
            
            chargesText.gameObject.SetActive(false);
        }

        private void UpdateChargesText(int current, int max)
        {
            if (chargesText != null)
            {
                chargesText.text = $"{current}";
            }
        }

        private void HandleWeaponDataChanged(int inputIndex, WeaponDataSO data)
        {
            if (inputIndex != (int)input)
                return;

            weaponData = data;
            SetWeaponIcon();
            
            StartCoroutine(BindChargesEventDelayed());
        }

        private System.Collections.IEnumerator BindChargesEventDelayed()
        {
            yield return new WaitForEndOfFrame();
            BindChargesEvent();
        }

        private void Start()
        {
            weaponObjectName = input.ToString().Substring(0, 1).ToUpper() + input.ToString().Substring(1) + "Weapon";
            weaponInventory.TryGetWeapon((int)input, out weaponData);
            SetWeaponIcon();
            StartCoroutine(BindChargesEventDelayed());
        }

        private void Awake()
        {
            // Tự động tìm đúng thằng con "WeaponIcon" của chính nó, bất chấp trong Inspector gán sai
            Transform iconTransform = transform.Find("WeaponIcon");
            if (iconTransform != null)
            {
                weaponIcon = iconTransform.GetComponent<Image>();
            }
        }

        private void OnEnable()
        {
            weaponInventory.OnWeaponDataChanged += HandleWeaponDataChanged;
        }

        private void OnDisable()
        {
            weaponInventory.OnWeaponDataChanged -= HandleWeaponDataChanged;
            if (currentHealComponent != null)
            {
                currentHealComponent.OnChargesChanged -= UpdateChargesText;
            }
        }
    }
}
