using System;

namespace LetsEncryptAcmeReg
{
    public struct CreateAndBindResult
    {
        public readonly BindResult BindResult;
        public readonly Action RemoveTooltips;
        public readonly Action RemoveControls;

        public CreateAndBindResult(BindResult bindResult, Action removeTooltips, Action removeControls)
        {
            this.BindResult = bindResult;
            this.RemoveTooltips = removeTooltips;
            this.RemoveControls = removeControls;
        }
    }
}