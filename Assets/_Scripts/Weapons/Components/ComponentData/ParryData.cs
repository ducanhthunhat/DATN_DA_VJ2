using UnityEngine;

namespace DucAnh.Weapons.Components
{
    public class ParryData : ComponentData<AttackParry>
    {
        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(Parry);
        }
    }
}
