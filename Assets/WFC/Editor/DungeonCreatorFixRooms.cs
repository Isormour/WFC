using System.Collections.Generic;
using UnityEngine;

public class DungeonCreatorFixRooms : DungeonCreatorPage
{
    List<Cell> CellsToCheck;
    List<Branch> dungeonBranches;
    Transform dungeonParent;
    Cell[,] grid;
    public DungeonCreatorFixRooms(string name) : base(name)
    {
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
                ColorCellObject(col, item);
                CellsToCheck.Remove(item);
            }
            branches.Add(new Branch(temp));
            cellCount = CellsToCheck.Count;
        }
        RemoveDuoBranches(branches);
        return branches;
    }

    private static void ColorCellObject(Color col, Cell item)
    {
        MeshRenderer[] rends = item.CellObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var rend in rends)
        {
            rend.material.color = rend.material.color * col;
        }
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
    class Branch
    {
        public List<Cell> cells { private set; get; }
        public Transform rootObject;
        public Cell StairsCell { private set; get; }
        public Branch(List<Cell> cells)
        {
            this.cells = cells;
        }
        public void FindEnterAndExit()
        {
            Cell stairsCell = cells[0];
            Cell exitCell = cells[1];
            //find entry branch

            List<Cell> singleRooms = FindAllCellsWithConditions(new List<ECondition> { ECondition.Pass, ECondition.Wall }, new List<int> { 1, 3 });
            List<Cell> otherRooms = FindAllCellsWithConditions(new List<ECondition> { ECondition.Pass }, new List<int> { 2 });

            if (singleRooms.Count > 0)
            {
                stairsCell = singleRooms[Random.Range(0, singleRooms.Count)];
                singleRooms.Remove(stairsCell);
            }
            else if (otherRooms.Count > 0)
            {
                stairsCell = otherRooms[Random.Range(0, otherRooms.Count)];
                otherRooms.Remove(stairsCell);
            }

            cells.Remove(stairsCell);
            cells.Insert(0, stairsCell);

            Color col = stairsCell.CellObject.GetComponentsInChildren<MeshRenderer>()[1].material.color;
            ColorCellObject(col + (col * 0.5f), stairsCell);

            if (singleRooms.Count > 0)
            {
                exitCell = singleRooms[Random.Range(0, singleRooms.Count)];
                singleRooms.Remove(exitCell);
            }
            else if (otherRooms.Count > 0)
            {
                exitCell = otherRooms[Random.Range(0, otherRooms.Count)];
                otherRooms.Remove(exitCell);
            }

            cells.Remove(exitCell);
            cells.Insert(0, exitCell);

            cells.Remove(stairsCell);
            cells.Insert(0, stairsCell);

            ColorCellObject(col - (col * 0.5f), exitCell);
            StairsCell = stairsCell;
        }

        List<Cell> FindAllCellsWithConditions(List<ECondition> conds, List<int> numberOfOccurances)
        {
            List<Cell> fits = new List<Cell>();
            for (int i = 0; i < cells.Count; i++)
            {
                fits.Add(cells[i]);
                for (int j = 0; j < conds.Count; j++)
                {
                    if (cells[i].condition.GetConditionAmount(conds[j]) == numberOfOccurances[j])
                    {
                        continue;
                    }
                    if (fits.Contains(cells[i]))
                    {
                        fits.Remove(cells[i]);
                        break;
                    }
                }
            }
            return fits;
        }
        internal void CreateContainer(Transform MainParent, string name)
        {
            GameObject branchParent = new GameObject(name);
            branchParent.transform.position = StairsCell.CellObject.transform.position;
            branchParent.transform.SetParent(MainParent);
            rootObject = branchParent.transform;
        }

        internal void ReparentCells()
        {
            foreach (var cell in cells)
            {
                cell.CellObject.transform.SetParent(rootObject);
            }
        }

        internal void Destroy()
        {
#if UNITY_EDITOR
            foreach (Cell cell in cells)
            {
                cell.Destroy();
            }
#endif
        }
    }
}
