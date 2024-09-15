namespace VIRTUE
{
    [System.Flags]
    public enum Layers
    {
        Nothing = 0,
        Everything = ~0,
        Default = 1 << 0,
        TransparentFX = 1 << 1,
        IgnoreRaycast = 1 << 2,
        Water = 1 << 4,
        UI = 1 << 5,
        Resource = 1 << 6,
        Furnace = 1 << 7,
        Ally = 1 << 8,
        Enemy = 1 << 9
    }
}