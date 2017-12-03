namespace LetsEncryptAcmeReg
{
    public static class BindableExtensions
    {
        public static void SetOnce<T>(this Bindable<T> bindable, T value)
        {
            var version = bindable.Version;
            BindableChanging<T> currentAuthStateOnChanging = (Bindable<T> sender, ref T val, T prev, ref bool cancel)
                =>
            {
                cancel |= sender.Version > version;
            };

            bindable.Changing += currentAuthStateOnChanging;
            try
            {
                bindable.Value = value;
            }
            finally
            {
                bindable.Changing -= currentAuthStateOnChanging;
            }
        }
    }
}