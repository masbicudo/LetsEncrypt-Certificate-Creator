namespace LetsEncryptAcmeReg
{
    public delegate void BindableChanging<T>(Bindable<T> sender, T value, T prev, ref bool cancel);
}