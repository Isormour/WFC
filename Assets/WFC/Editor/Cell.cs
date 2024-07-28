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
    Cell nTop;
    Cell nBottom;
    Cell nLeft;
    Cell nRight;
  
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
        this.nTop = top;
        this.nBottom = bottom;
        this.nRight = right;
        this.nLeft = left;
    }
    public List<Cell> GetConnectedNeibhours(List<ECondition> conds)
    {
        List<Cell> neibhours = new List<Cell>();
        if (nTop != null && conds.Contains(condition.Top) && condition.Top == nTop.condition.Bottom)
        {
            neibhours.Add(nTop);
        }
        if (nBottom != null && conds.Contains(condition.Bottom) && condition.Bottom == nBottom.condition.Top)
        {
            neibhours.Add(nBottom);
        }
        if (nLeft != null && conds.Contains(condition.Left)&& condition.Left == nLeft.condition.Right)
        {
            neibhours.Add(nLeft);
        }
        if (nRight != null && conds.Contains(condition.Right) && condition.Right == nRight.condition.Left)
        {
            neibhours.Add(nRight);
        }
        return neibhours;
    }
    public List<Cell> GetNeibhoursWithCondition(ECondition cond)
    {
        List<Cell> neibhours = new List<Cell>();
        if (nTop != null && condition.Top == cond &&nTop.condition.Bottom == cond)
        {
            neibhours.Add(nTop);
        }
        if (nBottom != null && condition.Bottom == cond && nBottom.condition.Top == cond)
        {
            neibhours.Add(nBottom);
        }
        if (nLeft != null && condition.Left == cond && nLeft.condition.Right == cond)
        {
            neibhours.Add(nLeft);
        }
        if (nRight != null && condition.Right == cond && nRight.condition.Left == cond)
        {
            neibhours.Add(nRight);
        }
        return neibhours;
    }
    public void Collapse(Transform parent, CollapseOption[] options)
    {
        List<CollapseOption> properOptions = FindProperOptions(options, GetCollapseCondition());
        currentOption = properOptions[Random.Range(0, properOptions.Count)];

        GameObject cellInstance = GameObject.Instantiate(currentOption.Prefab);
        cellInstance.transform.localPosition = new Vector3(x, 0, y);
        cellInstance.transform.localRotation = Quaternion.Euler(0, currentOption.RotatedAngle, 0);
        cellInstance.transform.SetParent(parent);
        cellInstance.name = currentOption.name;
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
        this.nTop?.UpdateEntropy();
        this.nBottom?.UpdateEntropy();
        this.nLeft?.UpdateEntropy();
        this.nRight?.UpdateEntropy();

        for (int i = 0; i < condition.Optionals.Length; i++)
        {
            switch (condition.Optionals[i].dir)
            {
                case ECellDirection.North:
                    nTop?.RemoveOptions(condition.Optionals[i].condition);
                    break;
                case ECellDirection.South:
                    nBottom?.RemoveOptions(condition.Optionals[i].condition);
                    break;
                case ECellDirection.East:
                    nLeft?.RemoveOptions(condition.Optionals[i].condition);
                    break;
                case ECellDirection.West:
                    nRight?.RemoveOptions(condition.Optionals[i].condition);
                    break;
                default:
                    break;
            }
        }
    }
    public void RemoveOptions(ECondition condition)
    {
        List<CollapseOption> tempOptions = new List<CollapseOption>();
        foreach (var item in collaspeOptions)
        {
            if (!item.Condition.CheckAnySideCondition(condition))
            {
                tempOptions.Add(item);
            }
        }
        this.collaspeOptions = tempOptions.ToArray();
    }
    private void UpdateEntropy()
    {
        collaspeOptions = FindProperOptions(collaspeOptions, GetCollapseCondition()).ToArray();
        EntropyValue = collaspeOptions.Length;
    }
    private CollapseCondition GetCollapseCondition()
    {
        ECondition top = CheckNeighbhourCondition(this.nTop, ECellDirection.North);
        ECondition bottom = CheckNeighbhourCondition(this.nBottom, ECellDirection.South);
        ECondition left = CheckNeighbhourCondition(this.nLeft, ECellDirection.East);
        ECondition right = CheckNeighbhourCondition(this.nRight, ECellDirection.West);
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