using System;

namespace LetsEncryptAcmeReg.SSG
{
    public interface ISsg :
        IDisposable
    {
        bool Initialize(ISsgController controller, ISsgMasterModel mainModel, IControlCreatorAndBinder createAndBind);

        bool IsValid();

        /// <summary>
        /// Patch the repository to allow the challenge files to be served.
        /// </summary>
        void Patch();
    }
}