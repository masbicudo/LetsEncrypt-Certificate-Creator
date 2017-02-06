namespace Replaceables
{
    public interface ISystemIoFile
    {
        bool Exists(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string contents);
    }
}