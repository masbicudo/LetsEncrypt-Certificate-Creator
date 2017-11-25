using System.Threading.Tasks;

namespace LetsEncryptAcmeReg
{
    public delegate Task<(bool cancel, T newValue)> BindableChangingAsync<T>(Bindable<T> sender, T value, T prev, bool canceled);
}