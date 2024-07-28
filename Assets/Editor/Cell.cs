using System.Collections.Generic;
using UnityEngine;

public enum ECellDirection
{
    North,
    South,
    East,
    West
}
public class Cell
{
    public int x { private set; get; }
    public int y { private set; get; }
    Cell top;
    Cell bottom;
    Cell left;
    Cell right;
  
    public int EntropyValue { private set; get; }
    CollapseOption[] collaspeOptions;
    public CollapseCondition condition { private set; get; }
    public CollapseOption currentOption = null;
  
    public GameObject CellObject { private set; get; }

    public Cell(int x, int y, CollapseOption[] options)
    {
        this.x = x;
        this.y = y;
        this.collaspeOptions = options;
        EntropyValue = options.Length;
    }
    public void SetNeighbours(Cell top, Cell bottom, Cell left, Cell right)
    {
        this.top = top;
        this.bottom = bottom;
        this.right = right;
        this.left = left;
    }
    public List<Cell> GetNeibhoursWithCondition(ECondition cond)
    {
        List<Cell> neibhours = new List<Cell>();
        if (top != null && condition.Top == cond &&top.condition.Bottom == cond)
        {
            neibhours.Add(top);
        }
        if (bottom != null && condition.Bottom == cond && bottom.condition.Top == cond)
        {
            neibhours.Add(bottom);
        }
        if (left != null && condition.Left == cond && left.condition.Right == cond)
        {
            neibhours.Add(left);
        }
        if (right != null && condition.Right == cond && right.condition.Left == cond)
        {
            neibhours.Add(right);
        }
        return neibhours;
    }
    public void Collapse(Transform parent, CollapseOption[] options)
    {
        List<CollapseOption> properOptions = FindProperOptions(options, GetCollapseCondition());
        currentOption = properOptions[Random.Range(0, properOptions.Count)];

        GameObject cellInstance = GameObject.Instantiate(currentOption.Prefab);
        cellInstance.transform.localPosition = new Vector3(x, 0, y);
        cellInstance.transform.SetParent(parent);
        CellObject = cellInstance;
        condition = currentOption.Condition;
        UpdateNeighbourEntropy();
    }
    public void Collapse(Transform parent)
    {
        Collapse(parent,collaspeOptions);
    }

    private void UpdateNeighbourEntropy()
    {
        this.top?.UpdateEntropy();
        this.bottom?.UpdateEntropy();
        this.left?.UpdateEntropy();
        this.right?.UpdateEntropy();
    }

    private void UpdateEntropy()
    {
        EntropyValue = FindProperOptions(collaspeOptions, GetCollapseCondition()).Count;
    }
    private CollapseCondition GetCollapseCondition()
    {
        ECondition top = CheckNeighbhourCondition(this.top, ECellDirection.North);
        ECondition bottom = CheckNeighbhourCondition(this.bottom, ECellDirection.South);
        ECondition left = CheckNeighbhourCondition(this.left, ECellDirection.East);
        ECondition right = CheckNeighbhourCondition(this.right, ECellDirection.West);
        CollapseCondition cond = new CollapseCondition(top, bottom, left, right);
        return cond;
    }
    public List<CollapseOption> FindProperOptions(CollapseOption[] options, CollapseCondition cond)
    {
        List<CollapseOption> tempOptions = new List<CollapseOption>();
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i].Condition.CheckCondition(cond))
            {
                tempOptions.Add(options[i]);
            }
        }
        return tempOptions;
    }
    ECondition CheckNeighbhourCondition(Cell cell, ECellDirection dir)
    {
        ECondition cellCondition = ECondition.Any;
        if (cell == null || cell.condition == null)
        {
            return ECondition.Any;
        }
        switch (dir)
        {
            case ECellDirection.North:
                cellCondition = cell.condition.Bottom;
                break;
            case ECellDirection.South:
                cellCondition = cell.condition.Top;
                break;
            case ECellDirection.East:
                cellCondition = cell.condition.Right;
                break;
            case ECellDirection.West:
                cellCondition = cell.condition.Left;
                break;
            default:
                break;
        }
        return cellCondition;
    }
}