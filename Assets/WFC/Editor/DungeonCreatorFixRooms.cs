using System.Collections.Generic;
using UnityEngine;
using WFC;

public partial class DungeonCreatorFixRooms : DungeonCreatorPage
{
    List<Cell> CellsToCheck;
    List<Branch> dungeonBranches;
    Transform dungeonParent;
    Cell[,] grid;
    ConditionsConfig.Condition[] conditions;
    public DungeonCreatorFixRooms(string name, ConditionsConfig.Condition[] conditions) : base(name)
    {
        this.conditions = conditions;
    }
    public void SetGrid(Cell[,] grid, Transform mainParent)
    {
        this.grid = grid;
        CellsToCheck = new List<Cell>();
        ECondition[] connectedCond = new ECondition[] { ECondition.Pass, ECondition.RoomL, ECondition.RoomR };

        dungeonParent = mainParent;
        //check connected cells
        foreach (var item in grid)
        {
            foreach (var con in connectedCond)
            {
                if (CheckCellForCondition(item, con))
                {
                    if (!CellsToCheck.Contains(item))
                        CellsToCheck.Add(item);
                }
            }
        }

        dungeonBranches = CreateGraph();
        ReparentBranches(dungeonBranches);
        RepositionBranches(dungeonBranches);
        CleanUp();
    }
    bool CheckCellForCondition(Cell cell, ECondition cond)
    {
        return (cell.condition.Top == cond ||
                cell.condition.Bottom == cond ||
                cell.condition.Left == cond ||
                cell.condition.Right == cond);
    }
    void CleanUp()
    {
        foreach (Cell item in grid)
        {
            bool toRemove = true;
            for (int i = 0; i < dungeonBranches.Count; i++)
            {
                if (dungeonBranches[i].cells.Contains(item))
                {
                    toRemove = false;
                    break;
                }
            }
            if (toRemove)
                item.Destroy();
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
            dungeonBranches = CreateGraph();
        }
        if (GUILayout.Button("reparent branches"))
        {
            ReparentBranches(dungeonBranches);
        }
        if (GUILayout.Button("reposition branches"))
        {
            RepositionBranches(dungeonBranches);
        }
    }

    private void RemoveDuoBranches(List<Branch> branches)
    {
        for (int i = 0; i < branches.Count; i++)
        {
            if (branches[i].cells.Count == 2)
            {
                branches[i].Destroy();
                branches.Remove(branches[i]);
                i--;
            }
        }
    }

    void ReparentBranches(List<Branch> graph)
    {
        for (int i = 0; i < graph.Count; i++)
        {
            Branch branch = graph[i];
            branch.FindEnterAndExit();
            branch.CreateContainer(dungeonParent, "Branch " + i);
            branch.ReparentCells();
        }
    }
    void RepositionBranches(List<Branch> graph)
    {
        graph.Sort((a, b) => b.cells.Count.CompareTo(a.cells.Count));

        for (int i = 1; i < graph.Count; i++)
        {
            Vector3 targetPosition = graph[i - 1].cells[1].CellObject.transform.position + new Vector3(0, 1, 0);
            graph[i].rootObject.transform.position = targetPosition;
            GameObject stairsObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stairsObject.name = "stairs";
            stairsObject.transform.localScale = MultiplyVector(stairsObject.transform.localScale, new Vector3(0.75f, 1.0f, 0.75f));
            stairsObject.transform.position = targetPosition - new Vector3(0, 0.5f, 0);
            stairsObject.transform.SetParent(graph[i].rootObject.transform);
        }
    }
    List<Branch> CreateGraph()
    {
        List<Branch> branches = new List<Branch>();
        int cellCount = CellsToCheck.Count;
        while (cellCount > 0)
        {
            Cell root = CellsToCheck[0];
            List<Cell> temp = new List<Cell>();
            GrowBranch(root, temp);

            Color col = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            foreach (var item in temp)
            {
                DungeonManager.ColorCellObject(col, item);
                CellsToCheck.Remove(item);
            }
            branches.Add(new Branch(temp));
            cellCount = CellsToCheck.Count;
        }
        RemoveDuoBranches(branches);
        return branches;
    }

    public static Vector3 MultiplyVector(Vector3 a, Vector3 b)
    {
        Vector3 result = a;
        result.x *= b.x;
        result.y *= b.y;
        result.z *= b.z;
        return result;
    }
    void GrowBranch(Cell cell, List<Cell> branch)
    {
        List<ECondition> ConnectedConditions = new List<ECondition>
        {
            ECondition.Pass,
            ECondition.RoomL,
            ECondition.RoomR,
            ECondition.Room
        };

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
