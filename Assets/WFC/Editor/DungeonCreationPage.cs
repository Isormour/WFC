using System;
using UnityEditor;
using UnityEngine;
using WFC;

internal class DungeonCreationPage : DungeonCreatorPage
{
    internal Action onGridCreated;
    private string v;
    public DungeonCreator creator { private set; get; }
    bool drawOptions = false;
    public DungeonCreationPage(string v, CollapseOption[] options):base(v)
    {
        this.v = v;
        creator = new DungeonCreator(options);
    }
    public override void InitPage()
    {

    }
    public override void Draw()
    {
        base.Draw();

        creator.width = EditorGUILayout.IntField("width", creator.width);
        creator.height = EditorGUILayout.IntField("height", creator.height);
        if (GUILayout.Button("Draw Options " + creator.collapseOptions.Length))
        {
            drawOptions = !drawOptions;
        }
        if (drawOptions) DrawProperties();


        if (GUILayout.Button("CollapseBorder"))
        {
            creator.GenerateBorder();
        }

        if (GUILayout.Button("Create Single"))
        {
            creator.Generate(true);
        }
        if (GUILayout.Button("Create All"))
        {
            creator.GenerateAll(true);
            onGridCreated?.Invoke();
        }
    }
    public void GenerateAll()
    {
        creator.GenerateAll(true);
        onGridCreated?.Invoke();
    }
    void DrawProperties()
    {
        for (int i = 0; i < creator.collapseOptions.Length; i++)
        {
            creator.collapseOptions[i] = (CollapseOption)EditorGUILayout.ObjectField(creator.collapseOptions[i], typeof(CollapseOption), false);
        }
    }
}