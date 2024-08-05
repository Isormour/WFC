using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using WFC;

public class DungeonCreator
{
    public int width = 10;
    public int height = 10;
    public Cell[,] grid;
    public List<Cell> cells;
    public CollapseOption[] collapseOptions;
    int tempX = 0, tempY = 0;
    public event Action onGridCreated;

    public Transform currentParent { private set; get; }
    
    public DungeonCreator(CollapseOption[] collapseOptions)
    {
        this.collapseOptions = collapseOptions;
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
   

    public void GenerateBorder()
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
                    grid[i, j].Collapse(currentParent,new CollapseOption[1] { collapseOptions[0] },false);
                    cells.Remove(grid[i, j]);
                    cells.OrderByDescending(c => c.EntropyValue);
                }
            }
        }
    }
   public  void Generate(bool createWorldObject)
    {
        if (cells.Count == 0)
        {
            return;
        }
        if (currentParent == null)
        {
            currentParent = new GameObject("dungeonParent").transform;
        }
      
        cells[0].Collapse(currentParent, createWorldObject);
        
        cells.RemoveAt(0);
        cells = cells.OrderBy(c => c.EntropyValue).ToList();
    }
   
   public void GenerateAll(bool createWorldObject)
    {
        currentParent = new GameObject("dungeonParent").transform;
        InitGrid();
        GenerateBorder();
        while (cells.Count > 0)
        {
            Generate(createWorldObject);
        }
    }
}


