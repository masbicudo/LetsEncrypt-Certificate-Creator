using System;
using System.Collections.Generic;

namespace LetsEncryptAcmeReg.SSG
{
    public interface ISsg :
        IDisposable
    {
        bool Initialize(ISsgController controller, ISsgMasterModel mainModel, IControlCreatorAndBinder createAndBind);

        IEnumerable<Exception> GetErrors();

        /// <summary>
        /// Patch the repository to allow the challenge files to be served.
        /// </summary>
        void Patch();
    }
}