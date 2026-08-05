using UnityEngine;

namespace DucAnh.Weapons.Components
{
    public class InputHoldData : ComponentData
    {
        [field: SerializeField] public bool AutoRelease { get; private set; }

        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(InputHold);
        }
    }
}
