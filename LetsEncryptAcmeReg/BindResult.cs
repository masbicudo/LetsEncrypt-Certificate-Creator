using System;
using JetBrains.Annotations;

namespace LetsEncryptAcmeReg
{
    public struct BindResult
    {
        /// <summary>
        /// Empty bind result.
        /// </summary>
        public static readonly BindResult Null = new BindResult();

        /// <summary>
        /// Action that can be used to synchronize the bound elements for the first time.
        /// </summary>
        /// <remarks>
        /// Running this multiple times has no effect.
        /// Only the first run will do something.
        /// </remarks>
        [CanBeNull]
        [UsedImplicitly]
        public readonly Action InitAction;

        /// <summary>
        /// Action that can be used to unbind the bound elements.
        /// </summary>
        /// <remarks>
        /// Unbinding does not undo the initial synchronization
        /// nor any synchronization that happen due to the binding.
        /// </remarks>
        [CanBeNull]
        [UsedImplicitly]
        public readonly Action UnbindAction;

        public BindResult([CanBeNull] Action initAction, [CanBeNull] Action unbindAction)
        {
            this.InitAction = initAction;
            this.UnbindAction = unbindAction;
        }

        public BindResult([CanBeNull] Action initAction)
        {
            this.InitAction = initAction;
            this.UnbindAction = null;
        }

        public static BindResult operator +(BindResult a, BindResult b)
        {
            return new BindResult(
                a.InitAction + b.InitAction,
                b.UnbindAction + a.UnbindAction);
        }
    }
}