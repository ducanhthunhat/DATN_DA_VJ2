using UnityEngine;
using DucAnh.Weapons.Components;

namespace DucAnh.Weapons.Components
{
    [System.Serializable]
    public class HealData : ComponentData
    {
        [field: SerializeField] public float Amount { get; private set; } = 50f;
        [field: SerializeField] public int MaxCharges { get; private set; } = 3;

        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(Heal);
        }
    }
}
