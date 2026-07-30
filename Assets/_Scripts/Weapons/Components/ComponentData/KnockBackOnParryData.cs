namespace DucAnh.Weapons.Components
{
    public class KnockBackOnParryData : ComponentData<AttackKnockBack>
    {
        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(KnockBackOnParry);
        }
    }
}
