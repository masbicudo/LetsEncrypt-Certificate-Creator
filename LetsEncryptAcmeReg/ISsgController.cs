using System;

namespace LetsEncryptAcmeReg
{
    public interface ISsgController
    {
        /// <summary>
        /// Runs the given delegate inside a try/catch block and logs the exception if any.
        /// </summary>
        T CatchError<T>(Func<T> func);

        /// <summary>
        /// Runs the given delegate inside a try/catch block and logs the exception if any.
        /// </summary>
        void CatchError(Action action);
    }
}