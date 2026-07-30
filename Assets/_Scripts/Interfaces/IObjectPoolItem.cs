using System;
using DucAnh.ObjectPoolSystem;
using UnityEngine;

namespace DucAnh.Interfaces
{
    public interface IObjectPoolItem
    {
        void SetObjectPool<T>(ObjectPool pool, T comp) where T : Component;

        void Release();
    }
}
