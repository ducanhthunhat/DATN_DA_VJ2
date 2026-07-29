using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoreComponent : MonoBehaviour, ILogicUpdate
{
    protected Core core;

    protected virtual void Awake()
    {
        core = transform.parent.GetComponent<Core>();

        if(core == null)
        Debug.LogError("no Core in parent");
        
        core.AddComponent(this);
    }

    public virtual void LogicUpdate()
    {
        
    }
}
