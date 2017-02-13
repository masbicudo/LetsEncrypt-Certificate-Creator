using System;
using JetBrains.Annotations;

namespace LetsEncryptAcmeReg
{
    [UsedImplicitly]
    struct AppenderResult
    {
        [UsedImplicitly]
        public readonly Action RemoveGroup;

        public AppenderResult(Action removeGroup)
        {
            this.RemoveGroup = removeGroup;
        }
    }
}