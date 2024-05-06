using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreatorFixRooms : DungeonCreatorPage
{
    Cell[,] grid;
    public DungeonCreatorFixRooms(string name) : base(name)
    {
    }
    public void SetGrid(Cell[,] grid)
    {
        this.grid = grid;
    }
    public override void InitPage()
    {
        base.InitPage();
    }
    public override void Draw()
    {
        base.Draw();
    }
}
