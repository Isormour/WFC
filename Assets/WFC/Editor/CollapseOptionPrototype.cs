using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Collapse_Condition_Prototype", menuName = "WFC/Collapse_Condition_Prototype")]
internal class CollapseOptionPrototype : ScriptableObject
{
    public GameObject Prefab;
    public CollapseCondition Condition;
    public bool IsRotateable;
    public bool IsSymetric;
}