using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

public class DungeonCreator : DungeonCreatorPage
{
    bool drawOptions = false;
    int gridSize = 10;
    public Cell[,] grid;
    List<Cell> cells;
    CollapseOptionGroup collapseOptionsborder;
    CollapseOptionGroup collapseOptionsFill;
    CollapseOption[] collapseOptions;
    int tempX = 0, tempY = 0;
    Transform currentParent;

    public DungeonCreator(string name) : base(name)
    {
    }


    //[MenuItem("Tools/Dungeon Creator")]
    public static void OpenWindow()
    {
        //DungeonCreator tempWindow = GetWindow<DungeonCreator>("Dungeon Creator");
        //tempWindow.InitWindow();

    }

    public override void InitPage()
    {
        collapseOptions = FindOptions();
        collapseOptionsborder = FindAsset<CollapseOptionGroup>("Assets/WFC/GroupBorder.asset");
        collapseOptionsFill = FindAsset<CollapseOptionGroup>("Assets/WFC/GroupFill.asset");
        InitGrid();
    }
    CollapseOption[] FindOptions()
    {
        string path = "Assets/WFC/Options";
        string[] files = Directory.GetFiles(path);
        List<CollapseOption> foundAssets = new List<CollapseOption>();
        foreach (var file in files)
        {
            if (!file.Contains("meta"))
            {
                foundAssets.Add(FindAsset<CollapseOption>(file));
            }
        }
        return foundAssets.ToArray();
    }
    T FindAsset<T>(string path) where T : UnityEngine.Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
    void InitGrid()
    {
        //fill grid
        cells = new List<Cell>();
        grid = new Cell[gridSize, gridSize];
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j] = new Cell(i, j, this.collapseOptions);
                cells.Add(grid[i, j]);
            }
        }

        // set neighbours;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Cell top = GetCellFromGrid(i, j + 1);
                Cell bottom = GetCellFromGrid(i, j - 1);
                Cell left = GetCellFromGrid(i - 1, j);
                Cell right = GetCellFromGrid(i + 1, j);
                grid[i, j].SetNeighbours(top, bottom, left, right);
            }
        }
    }
    Cell GetCellFromGrid(int x, int y)
    {
        Cell cell = null;
        bool xInRange = x >= 0 && x < gridSize;
        bool yInRange = y >= 0 && y < gridSize;
        if (xInRange && yInRange)
        {
            cell = grid[x, y];
        }
        return cell;
    }
    public override void Draw()
    {
        base.Draw();
        collapseOptionsborder = (CollapseOptionGroup)EditorGUILayout.ObjectField("Border", collapseOptionsborder, typeof(CollapseOptionGroup), false);
        collapseOptionsFill = (CollapseOptionGroup)EditorGUILayout.ObjectField("Fill", collapseOptionsFill, typeof(CollapseOptionGroup), false);

        if (GUILayout.Button("Draw Options"))
        {
            drawOptions = !drawOptions;
        }
        if (drawOptions) DrawProperties();


        if (GUILayout.Button("CollapseBorder"))
        {
            GenerateBorder();
        }

        if (GUILayout.Button("Create Single"))
        {
            Generate();
        }
        if (GUILayout.Button("Create All"))
        {
            GenerateLoop();
        }
    }

    private void GenerateBorder()
    {
        if (currentParent == null)
        {
            currentParent = new GameObject("dungeonParent").transform;
        }

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                bool isBorder = i == 0 || j == 0 || j == gridSize - 1 || i == gridSize - 1;
                if (isBorder)
                {
                    grid[i, j].Collapse(currentParent, collapseOptionsborder.Group);
                    cells.Remove(grid[i, j]);
                    cells.OrderByDescending(c => c.EntropyValue);
                }
            }
        }
    }

    private void Generate()
    {
        SingleGenerationEntropy();
    }
    void SingleGenerationEntropy()
    {
        if (cells.Count == 0)
        {
            return;
        }
        if (currentParent == null)
        {
            currentParent = new GameObject("dungeonParent").transform;
        }
        if (collapseOptionsFill == null)
        {
            cells[0].Collapse(currentParent);
        }
        else
        {
            cells[0].Collapse(currentParent, collapseOptionsFill.Group);
        }
        cells.RemoveAt(0);
        cells.OrderByDescending(c => c.EntropyValue);
    }
    void SingleGenerationGrid()
    {
        if (tempY >= gridSize)
        {
            return;
        }
        if (currentParent == null)
        {
            currentParent = new GameObject("dungeonParent").transform;
        }
        grid[tempX, tempY].Collapse(currentParent);
        tempX++;
        if (tempX >= gridSize)
        {
            tempY++;
            tempX = 0;
        }

    }
    void GenerateLoop()
    {
        currentParent = new GameObject("dungeonParent").transform;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j].Collapse(currentParent.transform);
            }
        }
    }
    void DrawProperties()
    {
        for (int i = 0; i < collapseOptions.Length; i++)
        {
            collapseOptions[i] = (CollapseOption)EditorGUILayout.ObjectField(collapseOptions[i], typeof(CollapseOption), false);
        }
    }
}


