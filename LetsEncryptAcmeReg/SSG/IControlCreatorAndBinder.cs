namespace LetsEncryptAcmeReg.SSG
{
    public interface IControlCreatorAndBinder
    {
        CreateAndBindResult ForBool(Bindable<bool> bindable, string label, string tooltip);
        CreateAndBindResult ForString(Bindable<string> bindable, string label, string tooltip);
    }
}