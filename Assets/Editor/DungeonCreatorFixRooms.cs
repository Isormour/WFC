using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreatorFixRooms : DungeonCreatorPage
{
    Cell[,] grid;
    List<Cell> CellsToCheck;
    public DungeonCreatorFixRooms(string name) : base(name)
    {
    }
    public void SetGrid(Cell[,] grid)
    {
        this.grid = grid;
        CellsToCheck = new List<Cell>();
        foreach (var item in grid)
        {
            if(item.condition.Top == ECondition.Pass ||
                item.condition.Bottom == ECondition.Pass|| 
                item.condition.Left == ECondition.Pass|| 
                item.condition.Right == ECondition.Pass)
                CellsToCheck.Add(item);
        }
    }
    public override void InitPage()
    {
        base.InitPage();
    }
    public override void Draw()
    {
        base.Draw();
        if (GUILayout.Button("Create graph"))
        {
            CreateGraphSingleStep();
        }
    }
    void CreateGraphSingleStep()
    {
        List<List<Cell>> Branches = new List<List<Cell>>();
        while (CellsToCheck.Count > 0)
        {
            Cell root = CellsToCheck[0];
            List<Cell> temp = new List<Cell>();
            GrowBranch(root, temp);

            Color col = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            foreach (var item in temp)
            {
                MeshRenderer[] rends = item.CellObject.GetComponentsInChildren<MeshRenderer>();
                foreach (var rend in rends)
                {
                    rend.material.color = col;
                }
                CellsToCheck.Remove(item);
            }
            Branches.Add(temp);
        }
    }
    void GrowBranch(Cell cell,List<Cell>branch)
    {
        List <Cell> tempNeibhours = cell.GetNeibhoursWithCondition(ECondition.Pass);

        foreach (var item in tempNeibhours)
        {
            if (!branch.Contains(item))
            {
                branch.Add(item);
                GrowBranch(item, branch);
            }
        }
    }

}
