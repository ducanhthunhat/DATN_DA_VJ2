namespace DucAnh.Weapons.Components
{
    public class HitStopOnHitBoxActionData : ComponentData<AttackHitStop>
    {
        protected override void SetComponentDependency()
        {
            ComponentDependency = typeof(HitStopOnHitBoxAction);
        }
    }
}
