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
    public CollapseConditionOptional[] Optionals;
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
    public bool CheckAnySideCondition(ECondition condition)
    {
        return Top == condition || Bottom == condition || Left == condition || Right == condition;
    }
    public void RotateOptionals()
    {
        if (Optionals == null) return;
        for (int i = 0; i < Optionals.Length; i++)
        {
            switch (Optionals[i].dir)
            {
                case ECellDirection.North:
                    Optionals[i].dir = ECellDirection.East;
                    break;
                case ECellDirection.South:
                    Optionals[i].dir = ECellDirection.West;
                    break;
                case ECellDirection.East:
                    Optionals[i].dir = ECellDirection.South;
                    break;
                case ECellDirection.West:
                    Optionals[i].dir = ECellDirection.North;
                    break;
                default:
                    break;
            }
        }
    }
    public CollapseConditionOptional[] GetOptionals()
    {
        CollapseConditionOptional[] temp = new CollapseConditionOptional[Optionals.Length];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = new CollapseConditionOptional();
            temp[i].condition = Optionals[i].condition;
            temp[i].dir = Optionals[i].dir;
        }
        return temp;
    }
}