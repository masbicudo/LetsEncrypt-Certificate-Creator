using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace LetsEncryptAcmeReg
{
    public abstract class Bindable
    {
        private static int _lastId;

        private readonly int id;
        private readonly string name;

        protected Bindable(string name)
        {
            this.id = Interlocked.Increment(ref _lastId);
            this.name = name ?? "";
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(this.name)
                ? $"{nameof(Bindable<int>)}({this.id})"
                : $"{nameof(Bindable<int>)}({this.id}): {this.name}";
        }
    }

    public class Bindable<T> : Bindable
    {

        const int FALSE = 0;
        const int TRUE = -1;

        private readonly BindableOptions flags;
        private sbyte isChanging;
        private sbyte isInit;
        private sbyte isUpdating;

        private readonly EqualityComparer<T> equalityComparer;

        private T value;
        private Func<T> getter;
        private BindableChanging<T> changingHandler;
        private Action<T> changedHandler;

        /// <summary>
        /// Gets the number of times this bindable object has been changed.
        /// </summary>
        public int Version { get; private set; }

        public Bindable(string name = "", EqualityComparer<T> equalityComparer = null, BindableOptions flags = 0)
            : base(name)
        {
            this.flags = flags;
            this.equalityComparer = equalityComparer;
        }

        /// <summary>
        /// Occurs when the value of this bindable object is changed.
        /// </summary>
        public event BindableChanging<T> Changing
        {
            add { this.changingHandler += value; }
            remove { this.changingHandler -= value; }
        }

        /// <summary>
        /// Occurs when the value of this bindable object is changed.
        /// </summary>
        public event Action<T> Changed
        {
            add { this.changedHandler += value; }
            remove { this.changedHandler -= value; }
        }

        /// <summary>
        /// Gets or sets the value of this bindable object,
        /// raising the event that indicates that the object is changing.
        /// </summary>
        public T Value
        {
            get { return value; }
            set
            {
                //if (isAsync != FALSE)
                //    throw new InvalidOperationException($"To set an async bindable object use the `{nameof(this.SetValueAsync)}` method.");

                if ((flags & BindableOptions.EqualMeansUnchanged) != 0)
                    if ((this.equalityComparer ?? EqualityComparer<T>.Default).Equals(this.value, value))
                        return;

                if (this.isChanging == FALSE)
                {
                    this.ForceSetValue(value);
                }
                else if ((flags & BindableOptions.AllowRecursiveSets) == 0)
                {
                    throw new InvalidOperationException("Recursively defining the value of a bindable object to different values is not allowed.");
                }
            }
        }

        /// <summary>
        /// Sets the value of this bindable object,
        /// raising the event that indicates that the object is changing.
        /// </summary>
        public void ForceSetValue(T value)
        {
            try
            {
                this.isChanging = TRUE;
                if (this.changingHandler != null)
                {
                    bool cancel = false;
                    this.changingHandler.Invoke(this, this.value, value, ref cancel);
                    if (cancel)
                        return;
                }

                this.value = value;
                this.Version++;
                this.isInit = TRUE;

                this.changedHandler?.Invoke(value);
                //if (this.changed != null)
                //    foreach (Action<T> @delegate in this.changed.GetInvocationList())
                //        @delegate(value);
            }
            finally
            {
                this.isChanging = FALSE;
            }
        }

        //public Task SetValueAsync(T value)
        //{
        //}

        /// <summary>
        /// Update the value of the bindable object with values from the registered external sources.
        /// </summary>
        /// <remarks>
        /// This should be called every time the external source is changed,
        /// unless you know exactly the source that is changing and knows how to update from it.
        /// Not every external source will be associated with a getter method.
        /// In that case, you will have to update the bindable value by setting it in your own code.
        /// </remarks>
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
        [ContractAnnotation("init:True => null; init:False => notnull")]
        public Action Bind([NotNull] Func<T> getter, [NotNull] Action<T> setter, bool init = false)
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
        public Action Bind([NotNull] Func<T> getter, bool init = false)
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
        [ContractAnnotation("init:True => null; init:False => notnull")]
        public Action Bind([NotNull] Action<T> setter, bool init = false)
        {
            if (setter == null) throw new ArgumentNullException(nameof(setter));
            return this.BindInternal(null, setter, init);
        }

        [ContractAnnotation("init:True => null; init:False => notnull")]
        private Action BindInternal([CanBeNull] Func<T> getter, [CanBeNull] Action<T> setter, bool init)
        {
            this.RegisterUpdater(getter);
            this.Changed += setter;

            bool used = false;
            Action initFn = () =>
            {
                if (!used) this.PostInitialize(getter, setter);
                used = true;
            };
            if (init) initFn();
            return init ? null : initFn;
        }

        private void PostInitialize([CanBeNull] Func<T> getter, [CanBeNull] Action<T> setter)
        {
            if (this.isInit != FALSE) setter?.Invoke(this.Value);
            else if (getter != null) this.Value = getter();
        }
    }
}