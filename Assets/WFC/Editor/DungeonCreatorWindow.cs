using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using WFC;
public class DungeonCreatorWindow : EditorWindow
{
    DungeonCreatorPage currentPage;
    OptionsManager optionsManager;
    DungeonCreationPage creatorPage;
    DungeonCreatorFixRooms fixRooms;
    ConditionConfigPage config;
    TestingPage tests;
    GameObject dungeonManagerObject;
    private static ConditionsConfig _config;
    internal static ConditionsConfig condConfig {
        get {
            if (_config == null)
            {
                _config = FindAsset<ConditionsConfig>("Assets/WFC/Conditions_Config.asset");
            }
            return _config;
        } 
    }
    [MenuItem("Tools/Dungeon Creator")]
    public static void OpenWindow()
    {
        _config = FindAsset<ConditionsConfig>("Assets/WFC/Conditions_Config.asset"); 
        DungeonCreatorWindow tempWindow = GetWindow<DungeonCreatorWindow>("Dungeon Creator");
        tempWindow.InitWindow();
        
    }
    void InitWindow()
    {
        CollapseOption[] options = FindAssets<CollapseOption>("Assets/WFC/GeneratedOptions/").ToArray();
        dungeonManagerObject = Instantiate(FindAsset<GameObject>("Assets/WFC/Prefabs/DungeonManager.prefab"));
        dungeonManagerObject.GetComponent<DungeonManager>().Initialize();
        creatorPage = new DungeonCreationPage("Dungeon Creator", options);
        creatorPage.onGridCreated += OnGridCreated;
        creatorPage.GenerateAll();
        ChangePage(creatorPage);
    
    }
    private void OnDestroy()
    {
        if (dungeonManagerObject != null)
        {
            DestroyImmediate(dungeonManagerObject);
            dungeonManagerObject = null;
        }
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
            optionsManager = new OptionsManager("Options");
            ChangePage(optionsManager);
        }
        if (GUILayout.Button("Generation"))
        {
            CollapseOption[] options = FindAssets<CollapseOption>("Assets/WFC/GeneratedOptions/").ToArray();
            creatorPage = new DungeonCreationPage("Dungeon Creator", options);
            creatorPage.onGridCreated += OnGridCreated;
            ChangePage(creatorPage);
        }
        if (GUILayout.Button("Fix Rooms"))
        {
            fixRooms = new DungeonCreatorFixRooms("Fix Rooms", condConfig.conditions);
            fixRooms.SetGrid(creatorPage.creator.grid,creatorPage.creator.currentParent);
            ChangePage(fixRooms);
        }
        if (GUILayout.Button("tests"))
        {
            tests = new TestingPage("Fix Rooms");
            ChangePage(tests);
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            CollapseOption[] options = FindAssets<CollapseOption>("Assets/WFC/GeneratedOptions/").ToArray();
            creatorPage = new DungeonCreationPage("Dungeon Creator", options);
            fixRooms = new DungeonCreatorFixRooms("Fix Rooms", condConfig.conditions);
            fixRooms.SetGrid(creatorPage.creator.grid, creatorPage.creator.currentParent);
            ChangePage(fixRooms);
        }

        currentPage.Draw();
    }

    private void OnGridCreated()
    {
        creatorPage.onGridCreated -= OnGridCreated;
        fixRooms = new DungeonCreatorFixRooms("Fix Rooms", condConfig.conditions);
        fixRooms.SetGrid(creatorPage.creator.grid, creatorPage.creator.currentParent);
        ChangePage(fixRooms);
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
