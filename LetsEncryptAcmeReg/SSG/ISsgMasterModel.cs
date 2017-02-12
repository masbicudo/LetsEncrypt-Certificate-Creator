namespace LetsEncryptAcmeReg.SSG
{
    public interface ISsgMasterModel
    {
        Bindable<string> Domain { get; }
        Bindable<string> Key { get; }
        Bindable<string> FileRelativePath { get; }
        Bindable<string> SiteRoot { get; }
        Bindable<string> FilePath { get; }
        Bindable<string[]> Files { get; }
    }
}