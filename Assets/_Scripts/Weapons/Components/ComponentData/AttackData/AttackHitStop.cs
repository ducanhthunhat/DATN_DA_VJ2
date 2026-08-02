using System;
using UnityEngine;

namespace DucAnh.Weapons.Components
{
    [Serializable]
    public class AttackHitStop : AttackData
    {
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public float TimeScale { get; private set; } = 0.05f;
    }
}
