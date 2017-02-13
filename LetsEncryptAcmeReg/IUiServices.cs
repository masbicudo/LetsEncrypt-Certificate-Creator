using LetsEncryptAcmeReg.SSG;

namespace LetsEncryptAcmeReg
{
    public interface IUIServices
    {
        IControlCreatorAndBinder CreatePanelForSsg();
        void ClearPanelForSsg();
    }
}