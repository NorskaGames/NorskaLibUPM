namespace NorskaLib.UI.Markers
{
    [System.Flags]
    public enum MarkerModes : byte
    {
        Uninitialized = 0,

        /// <summary>
        /// Is displayed at world position of the entry's transform and only when it's present on the screen.
        /// </summary>
        World = 1 << 0,

        /// <summary>
        /// Is displayed at the edge of the screen and only when the object itself is out of sight.
        /// </summary>
        Compass = 1 << 1,
    }
}