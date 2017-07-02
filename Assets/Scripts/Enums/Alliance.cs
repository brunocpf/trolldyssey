public enum Alliance
{
    None = 0,
    Ally = 1 << 0,
    Enemy = 1 << 1,
    All = Ally | Enemy
}

public static class AllianceExtensions
{
    public static bool IsSameAlliance(this Alliance a, Alliance b)
    {
        if ((a == Alliance.None && b != Alliance.None) || (a != Alliance.None && b == Alliance.None))
            return false;
        return (a & b) == a;
    }
}