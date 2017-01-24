using System;

namespace LetsEncryptAcmeReg
{
    [Flags]
    public enum BindableOptions : byte
    {
        /// <summary>
        /// Indicates that the change event is only raised if the previous and next values are different.
        /// Otherwise, the change event is called even if the value does not change.
        /// </summary>
        EqualMeansUnchanged = 1,

        /// <summary>
        /// Indicates that the change event handler is allowed to recursively set the value of the bindable.
        /// If this flags is not present, then an exception is thrown.
        /// </summary>
        AllowRecursiveSets = 2,
    }
}