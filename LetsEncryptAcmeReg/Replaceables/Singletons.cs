namespace Replaceables
{
    public static class Singletons
    {
        public static ISystemIoFile File { get; set; } = new DefaultSystemIoFile();
        public static ISystemIoDirectory Directory { get; set; } = new DefaultSystemIoDirectory();
    }
}