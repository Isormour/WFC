using System;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

internal class ConditionConfigPage : DungeonCreatorPage
{
    ConditionsConfig config;
    string[] enumNames;
    public ConditionConfigPage(string name) : base(name)
    {
        enumNames = Enum.GetNames(typeof(ECondition));
        config = DungeonCreatorWindow.FindAsset<ConditionsConfig>("Assets/WFC/Conditions_Config.asset");
    }
    public override void Draw()
    {

        base.Draw();
        int enumCount = config.conditions.Length;
        for (int i = 0; i < enumCount; i++)
        {
            string name = config.conditions[i].condition.ToString();
            Enum mask = (ECondition)config.conditions[i].Mask;
            config.conditions[i].Mask = (int)(ECondition)EditorGUILayout.EnumFlagsField(name, mask);
        }
        if (GUILayout.Button("Save config"))
        {
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssetIfDirty(config);
        }
    }
}
