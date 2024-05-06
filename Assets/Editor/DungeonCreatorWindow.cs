using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DungeonCreatorWindow : EditorWindow
{
    DungeonCreatorPage currentPage;
    DungeonCreator creator;
    DungeonCreatorFixRooms fixRooms;
    [MenuItem("Tools/Dungeon Creator")]
    public static void OpenWindow()
    {
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
        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        currentPage.Draw();
    }
    public void ChangePage(DungeonCreatorPage page)
    {
        currentPage = page;
        currentPage.InitPage();
    }
}
