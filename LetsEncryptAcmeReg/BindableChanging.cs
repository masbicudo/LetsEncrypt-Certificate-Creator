namespace LetsEncryptAcmeReg
{
    public delegate void BindableChanging<T>(Bindable<T> sender, ref T value, T prev, ref bool cancel);
}