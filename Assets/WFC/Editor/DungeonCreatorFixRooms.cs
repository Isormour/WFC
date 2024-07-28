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
        ECondition[] connectedCond = new ECondition[] { ECondition.Pass, ECondition.Room };

        foreach (var item in grid)
        {
            foreach (var con in connectedCond)
            {
                if(CheckCellForCondition(item,con))
                {
                    if(!CellsToCheck.Contains(item))
                    CellsToCheck.Add(item);
                }
            }
        }
    }
    bool CheckCellForCondition(Cell cell,ECondition cond)
    {
        return (cell.condition.Top == cond ||
                cell.condition.Bottom == cond ||
                cell.condition.Left == cond ||
                cell.condition.Right == cond);
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
        int CellCount = CellsToCheck.Count;
        while (CellCount > 0)
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
                    rend.material.color = rend.material.color * col;
                }
                CellsToCheck.Remove(item);
            }
            Branches.Add(temp);
        }
    }
    void GrowBranch(Cell cell,List<Cell>branch)
    {
        List<ECondition> ConnectedConditions = new List<ECondition>();
        ConnectedConditions.Add(ECondition.Pass);
        ConnectedConditions.Add(ECondition.Room);

        List<Cell> tempNeibhours = cell.GetConnectedNeibhours(ConnectedConditions);

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
