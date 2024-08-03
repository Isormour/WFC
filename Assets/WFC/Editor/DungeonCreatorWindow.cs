using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DungeonCreatorWindow : EditorWindow
{
    DungeonCreatorPage currentPage;
    OptionsManager optionsManager;
    DungeonCreator creator;
    DungeonCreatorFixRooms fixRooms;
    ConditionConfigPage config;
    TestingPage tests;
    internal static ConditionsConfig condConfig;
    [MenuItem("Tools/Dungeon Creator")]
    public static void OpenWindow()
    {
        condConfig = FindAsset<ConditionsConfig>("Assets/WFC/Conditions_Config.asset"); 
        DungeonCreatorWindow tempWindow = GetWindow<DungeonCreatorWindow>("Dungeon Creator");
        tempWindow.InitWindow();
        
    }
    void InitWindow()
    {
        creator = new DungeonCreator("Dungeon Creator");
        ChangePage(creator);
    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Condition config"))
        {
            config = new ConditionConfigPage("Condition Config");
            ChangePage(config);
        }
        if (GUILayout.Button("OptionsManager"))
        {
            optionsManager = new OptionsManager("Dungeon Creator");
            ChangePage(optionsManager);
        }
        if (GUILayout.Button("Generation"))
        {
            creator = new DungeonCreator("Dungeon Creator");
            ChangePage(creator);
        }
        if (GUILayout.Button("Fix Rooms"))
        {
            fixRooms = new DungeonCreatorFixRooms("Fix Rooms");
            fixRooms.SetGrid(creator.grid);
            ChangePage(fixRooms);
        }
        if (GUILayout.Button("tests"))
        {
            tests = new TestingPage("Fix Rooms");
            ChangePage(tests);
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        currentPage.Draw();
    }
    public void ChangePage(DungeonCreatorPage page)
    {
        currentPage = page;
        currentPage.InitPage();
    }

    public static T FindAsset<T>(string path) where T : UnityEngine.Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
    public static List<T> FindAssets<T>(string folderPath) where T : UnityEngine.Object
    {
        string[] paths = Directory.GetFiles(folderPath);
        List<string> pathsCleaned = new List<string>();
        foreach (var path in paths)
        {
            if (!path.Contains(".meta"))
            {
                pathsCleaned.Add(path);
            }
        }

        List<T> assetsFound = new List<T>();
        foreach (var path in pathsCleaned)
        {
            assetsFound.Add(FindAsset<T>(path));
        }
        return assetsFound;
    }
}
