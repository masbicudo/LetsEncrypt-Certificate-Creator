using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace LetsEncryptAcmeReg
{
    public abstract class Bindable
    {
        protected const int FALSE = 0;
        protected const int TRUE = -1;

        private static int _lastId;
        protected static readonly object _Locker = new object();

        private readonly int id;
        private readonly string name;

        protected readonly BindableOptions flags;
        protected sbyte isChanging;
        protected sbyte isInit;
        protected sbyte isUpdating;

        protected Bindable(string name, BindableOptions flags)
        {
            this.id = Interlocked.Increment(ref _lastId);
            this.name = name ?? "";
            this.flags = flags;
        }

        protected abstract object Object { get; }

        public override string ToString()
        {
            var obj = this.Object;

            var strName = "";
            if (!string.IsNullOrWhiteSpace(this.name))
                strName = $": {this.name}";

            var strState = "";
            strState += this.isChanging == FALSE ? " " : "C";
            strState += this.isChanging == FALSE ? " " : "U";

            strState = string.IsNullOrWhiteSpace(strState) ? "" : $" [{strState}]";

            var strVal = "";
            if (this.isInit != FALSE)
            {
                strVal += " = ";
                if (obj == null) strVal += "null";
                else if (obj is string)
                    strVal += '"' + $"{obj}"
                                  .Replace("\n", "\\n")
                                  .Replace("\t", "\\t")
                                  .Replace("\\", "\\\\")
                                  .Replace("\0", "\\0")
                                  .Replace("\f", "\\f")
                                  .Replace("\"", "\\\"")
                                  .Replace("\a", "\\a")
                                  .Replace("\b", "\\b")
                                  .Replace("\a", "\\a")
                                  .Replace("\r", "\\r")
                                  .Replace("\v", "\\v") + '"';
                else if (obj is char)
                    strVal += "'" + $"{obj}"
                                  .Replace("\n", "\\n")
                                  .Replace("\t", "\\t")
                                  .Replace("\\", "\\\\")
                                  .Replace("\0", "\\0")
                                  .Replace("\f", "\\f")
                                  .Replace("\'", "\\'")
                                  .Replace("\a", "\\a")
                                  .Replace("\b", "\\b")
                                  .Replace("\a", "\\a")
                                  .Replace("\r", "\\r")
                                  .Replace("\v", "\\v") + "'";
                else strVal += $"{obj}";
            }

            return $"{nameof(Bindable<int>)}({this.id}){strName}{strVal}{strState}";
        }
    }

    public class Bindable<T> : Bindable
    {
        private readonly EqualityComparer<T> equalityComparer;

        private T value;
        private Func<T> getter;
        private List<ChangingHandlers> changingHandlers;
        private List<ChangedHandlers> changedHandlers;

        /// <summary>
        /// Gets the number of times this bindable object has been changed.
        /// </summary>
        public int Version { get; private set; }

        public Bindable(string name = "", EqualityComparer<T> equalityComparer = null, BindableOptions flags = 0)
            : base(name, flags)
        {
            this.equalityComparer = equalityComparer;
        }

        /// <summary>
        /// Occurs when the value of this bindable object is changed.
        /// </summary>
        public event BindableChanging<T> Changing
        {
            add { lock (_Locker) if (value != null) ListAdder(ref this.changingHandlers, value); }
            remove { lock (_Locker) if (value != null) ListRemover(ref this.changingHandlers, value); }
        }

        /// <summary>
        /// Occurs when the value of this bindable object is changed.
        /// </summary>
        public event Action<T> Changed
        {
            add { lock (_Locker) if (value != null) ListAdder(ref this.changedHandlers, value); }
            remove { lock (_Locker) if (value != null) ListRemover(ref this.changedHandlers, value); }
        }

        /// <summary>
        /// Occurs when the value of this bindable object is changed.
        /// </summary>
        public event BindableChangingAsync<T> ChangingAsync
        {
            add { lock (_Locker) if (value != null) ListAdder(ref this.changingHandlers, value); }
            remove { lock (_Locker) if (value != null) ListRemover(ref this.changingHandlers, value); }
        }

        /// <summary>
        /// Occurs when the value of this bindable object is changed.
        /// </summary>
        public event Func<T, Task> ChangedAsync
        {
            add { lock (_Locker) if (value != null) ListAdder(ref this.changedHandlers, value); }
            remove { lock (_Locker) if (value != null) ListRemover(ref this.changedHandlers, value); }
        }

        private static void ListAdder<TH>(ref List<TH> list, TH value)
        {
            if (list == null)
                list = new List<TH>();
            list.Add(value);
        }

        private static void ListRemover<TH>(ref List<TH> list, TH value)
        {
            list.Remove(value);
            if (list.Count == 0)
                list = null;
        }

        /// <summary>
        /// Gets or sets the value of this bindable object,
        /// raising the event that indicates that the object is changing.
        /// </summary>
        public T Value
        {
            get { return this.value; }
            set
            {
                //if (isAsync != FALSE)
                //    throw new InvalidOperationException($"To set an async bindable object use the `{nameof(this.SetValueAsync)}` method.");

                if ((this.flags & BindableOptions.EqualMeansUnchanged) != 0)
                    if ((this.equalityComparer ?? EqualityComparer<T>.Default).Equals(this.value, value))
                        return;

                if (this.isChanging == FALSE)
                {
                    this.ForceSetValue(value);
                }
                else if ((this.flags & BindableOptions.AllowRecursiveSets) == 0)
                {
                    throw new InvalidOperationException("Recursively defining the value of a bindable object to different values is not allowed.");
                }
            }
        }

        /// <summary>
        /// Sets the value of this bindable object,
        /// raising the events that indicates that the object is changing.
        /// </summary>
        /// <remarks>
        /// ForceSetValue is an advanced method for setting the value of the bindable object.
        /// It should not be used unless you are creating extension methods for the bindable class.
        /// It's purpoe is to set the value of the bindable regardless of the option to compare
        /// with the previous value (see `BindableOptions.EqualMeansUnchanged`) and
        /// also disregarding the internal recursion control flag `isChanging`.
        /// So, if you use this method, the risk is of creating infinitely recursive behaviour.
        /// </remarks>
        [UsedImplicitly]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ForceSetValue(T value)
        {
            try
            {
                this.isChanging = TRUE;
                if (this.changingHandlers != null)
                {
                    bool cancel = false;
                    for (int it = 0; it < this.changingHandlers.Count; it++)
                        this.changingHandlers[it].Invoke(this, this.value, value, ref cancel);
                    if (cancel)
                        return;
                }

                this.value = value;
                this.Version++;
                this.isInit = TRUE;

                if (this.changedHandlers != null)
                    for (int it = 0; it < this.changedHandlers.Count; it++)
                        this.changedHandlers[it].Invoke(value);
            }
            finally
            {
                this.isChanging = FALSE;
            }
        }

        /// <summary>
        /// Sets the value of this bindable object,
        /// raising the event that indicates that the object is changing.
        /// </summary>
        /// <remarks>
        /// ForceSetValue is an advanced method for setting the value of the bindable object.
        /// It should not be used unless you are creating extension methods for the bindable class.
        /// It's purpoe is to set the value of the bindable regardless of the option to compare
        /// with the previous value (see `BindableOptions.EqualMeansUnchanged`) and
        /// also disregarding the internal recursion control flag `isChanging`.
        /// So, if you use this method, the risk is of creating infinitely recursive behaviour.
        /// </remarks>
        [UsedImplicitly]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public async Task ForceSetValueAsync(T value)
        {
            try
            {
                this.isChanging = TRUE;
                if (this.changingHandlers != null)
                {
                    bool cancel = false;
                    for (int it = 0; it < this.changingHandlers.Count; it++)
                        cancel = await this.changingHandlers[it].InvokeAsync(this, this.value, value, cancel);
                    if (cancel)
                        return;
                }

                this.value = value;
                this.Version++;
                this.isInit = TRUE;

                if (this.changedHandlers != null)
                    for (int it = 0; it < this.changedHandlers.Count; it++)
                        await this.changedHandlers[it].InvokeAsync(value);
            }
            finally
            {
                this.isChanging = FALSE;
            }
        }

        /// <summary>
        /// Sets the value of this bindable object asynchronously,
        /// raising the events that indicates that the object is changing.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [UsedImplicitly]
        public async Task SetValueAsync(T value)
        {
            if ((this.flags & BindableOptions.EqualMeansUnchanged) != 0)
                if ((this.equalityComparer ?? EqualityComparer<T>.Default).Equals(this.value, value))
                    return;

            if (this.isChanging == FALSE)
            {
                await this.ForceSetValueAsync(value);
            }
            else if ((this.flags & BindableOptions.AllowRecursiveSets) == 0)
            {
                throw new InvalidOperationException("Recursively defining the value of a bindable object to different values is not allowed.");
            }
        }

        /// <summary>
        /// Update the value of the bindable object with values from the registered external sources.
        /// </summary>
        /// <remarks>
        /// This should be called every time the external source is changed,
        /// unless you know exactly the source that is changing and knows how to update from it.
        /// Not every external source will be associated with a getter method.
        /// In that case, you will have to update the bindable value by setting it in your own code.
        /// </remarks>
        [UsedImplicitly]
        public void Update(bool force = false)
        {
            if (this.isUpdating != FALSE)
                return;

            this.isUpdating = TRUE;
            try
            {
                var value = this.Value;
                foreach (Func<T> getter in this.getter.GetInvocationList().OfType<Func<T>>())
                {
                    var newValue = getter();
                    if (!EqualityComparer<T>.Default.Equals(value, getter()))
                    {
                        value = newValue;
                        force = true;
                        break;
                    }
                }

                if (force)
                    this.ForceSetValue(value);
            }
            finally
            {
                this.isUpdating = FALSE;
            }
        }

        /// <summary>
        /// Regsiters an external source that can be used to later update the value of the bindable object.
        /// </summary>
        /// <param name="getter">The method that is used to get the external data.</param>
        /// <remarks>
        /// RegisterUpdater is an advanced method intended only for the
        /// implementation of extension methods for the bindable class.
        /// </remarks>
        [UsedImplicitly]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RegisterUpdater(Func<T> getter)
        {
            this.getter += getter;
        }

        /// <summary>
        /// Binds the bindable object with an external data source.
        /// Note that you should not use bindables inside the given delegates.
        /// </summary>
        /// <param name="getter">
        /// Delegate that is used to get data from the external data source.
        /// Note that you should not use bindables inside this delegate.
        /// </param>
        /// <param name="setter">
        /// Delegate that is used to set the external data source.
        /// Note that you should not use bindables inside this delegate.
        /// </param>
        /// <param name="init">
        /// If true, initializes either the bindable object with data from the external data source,
        /// or the external data source with data from the bindable object if already with data.
        /// If false, returns a delegate that does the same. In this case, if the delegate is not called
        /// then the bindable object and the external data source will not be syncrhonized until
        /// an event is raised from either objects.
        /// </param>
        /// <returns>
        /// If <see cref="init"/> is true, returns a delegate to do the first synchronization between the bindable object and the data source;
        /// otherwise it returns null.
        /// </returns>
        [UsedImplicitly]
        public BindResult Bind([NotNull] Func<T> getter, [NotNull] Action<T> setter, bool init = false)
        {
            if (getter == null) throw new ArgumentNullException(nameof(getter));
            if (setter == null) throw new ArgumentNullException(nameof(setter));
            return this.BindInternal(getter, setter, init);
        }

        /// <summary>
        /// Binds the bindable object with an external data source, as a single way binding.
        /// Note that you should not use bindables inside the given delegate.
        /// </summary>
        /// <param name="getter">
        /// Delegate that is used to get data from the external data source.
        /// Note that you should not use bindables inside this delegate.
        /// </param>
        /// <param name="init">
        /// If true, initializes the bindable object with data from the external data source, if not already with data.
        /// If false, returns a delegate that does the same. In this case, if the delegate is not called
        /// then the bindable object and the external data source will not be syncrhonized until
        /// an event is raised from either objects.
        /// </param>
        /// <returns>
        /// If <see cref="init"/> is true, returns a delegate to do the first synchronization between the bindable object and the data source;
        /// otherwise it returns null.
        /// </returns>
        [UsedImplicitly]
        public BindResult Bind([NotNull] Func<T> getter, bool init = false)
        {
            if (getter == null) throw new ArgumentNullException(nameof(getter));
            return this.BindInternal(getter, null, init);
        }

        /// <summary>
        /// Binds the bindable object as a source for external data.
        /// Note that you should not use bindables inside the given delegate.
        /// </summary>
        /// <param name="setter">
        /// Delegate that is used to set the external data source.
        /// Note that you should not use bindables inside this delegate.
        /// </param>
        /// <param name="init">
        /// If true, initializes the external data source with data from the bindable object if already with data.
        /// If false, returns a delegate that does the same. In this case, if the delegate is not called
        /// then the bindable object and the external data source will not be syncrhonized until
        /// an event is raised from either objects.
        /// </param>
        /// <returns>
        /// If <see cref="init"/> is true, returns a delegate to do the first synchronization between the bindable object and the data source;
        /// otherwise it returns null.
        /// </returns>
        [UsedImplicitly]
        public BindResult Bind([NotNull] Action<T> setter, bool init = false)
        {
            if (setter == null) throw new ArgumentNullException(nameof(setter));
            return this.BindInternal(null, setter, init);
        }

        private BindResult BindInternal([CanBeNull] Func<T> getter, [CanBeNull] Action<T> setter, bool init)
        {
            this.RegisterUpdater(getter);
            this.Changed += setter;

            bool used = false;
            Action initFn = () =>
            {
                if (!used) this.InitialSync(getter, setter);
                used = true;
            };
            if (init) initFn();

            Action unbinder = () =>
            {
                this.Changed -= setter;
                this.getter -= getter;
            };

            return new BindResult(init ? null : initFn, unbinder);
        }

        private void InitialSync([CanBeNull] Func<T> getter, [CanBeNull] Action<T> setter)
        {
            if (this.isInit != FALSE) setter?.Invoke(this.Value);
            else if (getter != null) this.Value = getter();
        }

        struct ChangingHandlers :
            IComparable<BindableChanging<T>>,
            IComparable<BindableChangingAsync<T>>
        {
            private BindableChanging<T> sync;
            private BindableChangingAsync<T> async;

            private ChangingHandlers([NotNull] BindableChanging<T> sync)
            {
                if (sync == null) throw new ArgumentNullException(nameof(sync));

                this.sync = sync;
                this.async = null;
            }

            private ChangingHandlers([NotNull] BindableChangingAsync<T> @async)
            {
                if (@async == null) throw new ArgumentNullException(nameof(@async));

                this.sync = null;
                this.async = async;
            }

            public void Invoke(Bindable<T> sender, T value, T prev, ref bool cancel)
            {
                if (this.sync != null)
                    this.sync(sender, value, prev, ref cancel);
                else if (this.@async != null)
                    cancel = this.@async(sender, value, prev, cancel).Result;
            }

            public async Task<bool> InvokeAsync(Bindable<T> sender, T value, T prev, bool cancel)
            {
                if (this.@async != null)
                    cancel = await this.@async(sender, value, prev, cancel);
                else if (this.sync != null)
                    this.sync(sender, value, prev, ref cancel);
                return cancel;
            }

            public static implicit operator ChangingHandlers(BindableChanging<T> b) => new ChangingHandlers(b);

            public static implicit operator ChangingHandlers(BindableChangingAsync<T> b) => new ChangingHandlers(b);

            public int CompareTo(BindableChanging<T> other)
            {
                return Comparer<BindableChanging<T>>.Default.Compare(this.sync, other);
            }

            public int CompareTo(BindableChangingAsync<T> other)
            {
                return Comparer<BindableChangingAsync<T>>.Default.Compare(this.@async, other);
            }
        }

        struct ChangedHandlers :
            IComparable<Action<T>>,
            IComparable<Func<T, Task>>
        {
            Action<T> sync;
            Func<T, Task> async;

            private ChangedHandlers([NotNull] Action<T> sync)
            {
                if (sync == null) throw new ArgumentNullException(nameof(sync));

                this.sync = sync;
                this.async = null;
            }

            private ChangedHandlers([NotNull] Func<T, Task> @async)
            {
                if (@async == null) throw new ArgumentNullException(nameof(@async));

                this.sync = null;
                this.@async = @async;
            }

            public void Invoke(T value)
            {
                if (this.sync != null)
                    this.sync(value);
                else if (this.@async != null)
                    this.@async(value).RunSynchronously();
            }

            public async Task InvokeAsync(T value)
            {
                if (this.@async != null)
                    await this.@async(value);
                else if (this.sync != null)
                    this.sync(value);
            }

            public static implicit operator ChangedHandlers(Action<T> b) => new ChangedHandlers(b);

            public static implicit operator ChangedHandlers(Func<T, Task> b) => new ChangedHandlers(b);

            public int CompareTo(Action<T> other)
            {
                return Comparer<Action<T>>.Default.Compare(this.sync, other);
            }

            public int CompareTo(Func<T, Task> other)
            {
                return Comparer<Func<T, Task>>.Default.Compare(this.@async, other);
            }
        }

        protected override object Object => this.Value;
    }
}