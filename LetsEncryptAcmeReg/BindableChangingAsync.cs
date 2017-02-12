using System.Threading.Tasks;

namespace LetsEncryptAcmeReg
{
    public delegate Task<bool> BindableChangingAsync<T>(Bindable<T> sender, T value, T prev, bool canceled);
}