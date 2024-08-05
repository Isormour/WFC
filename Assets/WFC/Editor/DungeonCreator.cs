using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using WFC;

public class DungeonCreator : DungeonCreatorPage
{
    bool drawOptions = false;
    int width = 10;
    int height = 10;
    public Cell[,] grid;
    List<Cell> cells;
    CollapseOption collapseOptionsborder;
    CollapseOption[] collapseOptions;
    int tempX = 0, tempY = 0;
    public event Action onGridCreated;

    public Transform currentParent { private set; get; }
    
    public DungeonCreator(string name) : base(name)
    {
    }


    //[MenuItem("Tools/Dungeon Creator")]
    public static void OpenWindow()
    {


    }

    public override void InitPage()
    {
        collapseOptions = DungeonCreatorWindow.FindAssets<CollapseOption>("Assets/WFC/GeneratedOptions/").ToArray();
        collapseOptionsborder = collapseOptions[0];
        InitGrid();
    }
    

    void InitGrid()
    {
        //fill grid
        cells = new List<Cell>();
        grid = new Cell[width, height];
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = new Cell(i, j, this.collapseOptions);
                cells.Add(grid[i, j]);
            }
        }

        // set neighbours;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
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
        bool xInRange = x >= 0 && x < grid.GetLength(0);
        bool yInRange = y >= 0 && y < grid.GetLength(1);
        if (xInRange && yInRange)
        {
            cell = grid[x, y];
        }
        return cell;
    }
    public override void Draw()
    {
        base.Draw();
     
        collapseOptionsborder = (CollapseOption)EditorGUILayout.ObjectField("Border", collapseOptionsborder, typeof(CollapseOption), false);
        width = EditorGUILayout.IntField("width", width);
        height = EditorGUILayout.IntField("height", height);
        if (GUILayout.Button("Draw Options "+ collapseOptions.Length))
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
            GenerateAll();
            onGridCreated?.Invoke();
        }
    }

    private void GenerateBorder()
    {
        if (currentParent == null)
        {
            currentParent = new GameObject("dungeonParent").transform;
        }
        InitGrid();
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                bool isBorder = i == 0 || j == 0 || j == grid.GetLength(1) - 1 || i == grid.GetLength(0) - 1;
                if (isBorder)
                {
                    grid[i, j].Collapse(currentParent,new CollapseOption[1] { collapseOptionsborder });
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
      
        cells[0].Collapse(currentParent);
        
        cells.RemoveAt(0);
        cells = cells.OrderBy(c => c.EntropyValue).ToList();
    }
   
   public void GenerateAll()
    {
        currentParent = new GameObject("dungeonParent").transform;
        InitGrid();
        GenerateBorder();
        while (cells.Count > 0)
        {
            SingleGenerationEntropy();
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


