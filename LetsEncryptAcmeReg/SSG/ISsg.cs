namespace LetsEncryptAcmeReg.SSG
{
    public interface ISsg
    {
        bool Init(string dir);
        bool IsValid();
        void CreateModel();
        void Patch();
    }
}