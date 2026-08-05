using System;
using System.Linq;
using DucAnh.Weapons.Components;
using UnityEngine;
using UnityEditor;

public class HealDataChecker
{
    [MenuItem("Tools/Check Heal Data")]
    public static void Check()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
        var filteredTypes = types.Where(type => type.IsSubclassOf(typeof(ComponentData)) && !type.ContainsGenericParameters && type.IsClass);
        Debug.Log("Total ComponentData types: " + filteredTypes.Count());
        foreach(var t in filteredTypes)
        {
            if (t.Name.Contains("Heal")) Debug.Log("Found: " + t.Name);
        }
    }
}
