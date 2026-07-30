using System;
using DucAnh.Combat.Damage;
using DucAnh.ProjectileSystem.Components;
using UnityEngine;

namespace DucAnh.ProjectileSystem
{
    /*
     * This MonoBehaviour is simply used to print the damage amount received in the ProjectileTestScene
     */
    public class TestDamageable : MonoBehaviour, IDamageable
    {
        public void Damage(DamageData data)
        {
            print($"{gameObject.name} Damaged: {data.Amount}");
        }
    }
}
