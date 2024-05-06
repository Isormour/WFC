public enum ECondition
{
    None,
    Any,
    Top,
    Bottom,
    Left,
    Right,
}

[System.Serializable]
public class CollapseCondition
{
    public ECondition Top;
    public ECondition Bottom;
    public ECondition Left;
    public ECondition Right;
    public CollapseCondition(ECondition top, ECondition bottom, ECondition left, ECondition right)
    {
        Top = top;
        Bottom = bottom;
        Left = left;
        Right = right;
    }
    public bool CheckCondition(CollapseCondition other)
    {
        bool fits =
        (this.Top == other.Top || (this.Top == ECondition.Any||other.Top == ECondition.Any)) &&
        (this.Bottom == other.Bottom || (this.Bottom == ECondition.Any || other.Bottom == ECondition.Any)) &&
        (this.Left == other.Left || (this.Left == ECondition.Any || other.Left == ECondition.Any)) &&
        (this.Right == other.Right|| (this.Right == ECondition.Any || other.Right == ECondition.Any));
        return fits;
    }
}