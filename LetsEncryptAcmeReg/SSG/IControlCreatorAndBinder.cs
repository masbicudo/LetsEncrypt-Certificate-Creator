namespace LetsEncryptAcmeReg.SSG
{
    public interface IControlCreatorAndBinder
    {
        BindResult ForBool(Bindable<bool> bindable, string label, string tooltip);
        BindResult ForString(Bindable<string> bindable, string label, string tooltip);
    }
}