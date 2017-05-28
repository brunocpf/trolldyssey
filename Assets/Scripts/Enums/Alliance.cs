public enum Alliance
{
    None = 0,
    Ally = 1 << 0,
    Enemy = 1 << 1,
    All = Ally | Enemy
}