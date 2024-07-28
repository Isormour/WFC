public enum ECondition
{
    None,
    Pass,
    Any,
    Room
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
        bool tempTop = (this.Top == other.Top || this.Top == ECondition.Any || other.Top == ECondition.Any);
        bool tempBottom = (this.Bottom == other.Bottom || this.Bottom == ECondition.Any || other.Bottom == ECondition.Any);
        bool tempLeft = (this.Left == other.Left || this.Left == ECondition.Any || other.Left == ECondition.Any);
        bool tempRight = (this.Right == other.Right || this.Right == ECondition.Any || other.Right == ECondition.Any);

        return tempTop && tempBottom && tempLeft && tempRight;
    }
}