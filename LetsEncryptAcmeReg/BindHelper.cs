using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace LetsEncryptAcmeReg
{
    public static class BindHelper
    {
        public static BindResult BindControl([NotNull] this Bindable<string> bindable, [NotNull] Control control)
        {
            if (bindable == null) throw new ArgumentNullException(nameof(bindable));
            if (control == null) throw new ArgumentNullException(nameof(control));

            bool isSetting = false;
            Func<string> getter = () => control.Text;
            Action<string> setter = b =>
            {
                isSetting = true;
                try
                {
                    if (control.Text != b)
                        control.Text = b;
                }
                finally
                {
                    isSetting = false;
                }
            };
            control.TextChanged += (sender, args) =>
            {
                if (!isSetting)
                    bindable.Value = getter();
            };
            return bindable.Bind(getter, setter);
        }

        public static BindResult BindControl([NotNull] this Bindable<bool> bindable, [NotNull] CheckBox checkBox)
        {
            if (bindable == null) throw new ArgumentNullException(nameof(bindable));
            if (checkBox == null) throw new ArgumentNullException(nameof(checkBox));

            bool isSetting = false;
            Func<bool> getter = () => checkBox.Checked;
            Action<bool> setter = b =>
            {
                isSetting = true;
                try
                {
                    if (checkBox.Checked != b)
                        checkBox.Checked = b;
                }
                finally
                {
                    isSetting = false;
                }
            };
            checkBox.CheckedChanged += (sender, args) =>
            {
                if (!isSetting)
                    bindable.Value = getter();
            };
            return bindable.Bind(getter, setter);
        }

        public static BindResult BindControl<T>([NotNull] this Bindable<T> bindable, [NotNull] ComboBox cmb)
        {
            if (bindable == null) throw new ArgumentNullException(nameof(bindable));
            if (cmb == null) throw new ArgumentNullException(nameof(cmb));

            bool isSetting = false;
            Func<T> getter = () => (T)cmb.SelectedItem;
            Action<T> setter = b =>
            {
                isSetting = true;
                try
                {
                    if (!EqualityComparer<T>.Default.Equals((T)cmb.SelectedItem, b))
                        cmb.SelectedItem = b;
                }
                finally
                {
                    isSetting = false;
                }
            };
            cmb.TextChanged += (sender, args) =>
            {
                if (cmb.DropDownStyle != ComboBoxStyle.DropDownList && !isSetting)
                    bindable.Value = getter();
            };
            cmb.SelectedIndexChanged += (sender, args) =>
            {
                if (cmb.DropDownStyle == ComboBoxStyle.DropDownList && !isSetting)
                    bindable.Value = getter();
            };
            return bindable.Bind(getter, setter);
        }

        public static BindResult BindControl<TValue, TListItem>(
            [NotNull] this Bindable<TValue> bindable,
            [NotNull] ComboBox cmb,
            [NotNull] Func<TListItem, TValue> convertIn,
            [CanBeNull] Func<TValue, TListItem> convertOut = null
            )
        {
            if (bindable == null) throw new ArgumentNullException(nameof(bindable));
            if (cmb == null) throw new ArgumentNullException(nameof(cmb));
            if (convertIn == null) throw new ArgumentNullException(nameof(convertIn));

            bool isSetting = false;
            Func<TValue> getter = () => convertIn((TListItem)cmb.SelectedItem);
            Action<TValue> setter = b =>
            {
                isSetting = true;
                try
                {
                    if (!EqualityComparer<TValue>.Default.Equals(convertIn((TListItem)cmb.SelectedItem), b))
                        cmb.SelectedItem = convertOut != null ? (object)convertOut(b) : b;
                }
                finally
                {
                    isSetting = false;
                }
            };
            cmb.TextChanged += (sender, args) =>
            {
                if (cmb.DropDownStyle != ComboBoxStyle.DropDownList && !isSetting)
                    bindable.Value = getter();
            };
            cmb.SelectedIndexChanged += (sender, args) =>
            {
                if (cmb.DropDownStyle == ComboBoxStyle.DropDownList && !isSetting)
                    bindable.Value = getter();
            };
            return bindable.Bind(getter, setter);
        }

        public static BindResult BindControl<T>([NotNull] this Bindable<T> bindable, [NotNull] ListBox lst)
        {
            if (bindable == null) throw new ArgumentNullException(nameof(bindable));
            if (lst == null) throw new ArgumentNullException(nameof(lst));

            bool isSetting = false;
            Func<T> getter = () => (T)lst.SelectedItem;
            Action<T> setter = b =>
            {
                isSetting = true;
                try
                {
                    if (!EqualityComparer<T>.Default.Equals((T)lst.SelectedItem, b))
                        lst.SelectedItem = b;
                }
                finally
                {
                    isSetting = false;
                }
            };
            lst.SelectedIndexChanged += (sender, args) =>
            {
                if (!isSetting)
                    bindable.Value = getter();
            };
            return bindable.Bind(getter, setter);
        }

        public static BindResult Bind<TValue, TListItem>(
            [NotNull] this Bindable<TValue> bindable,
            [NotNull] ListBox lst,
            [NotNull] Func<TListItem, TValue> convertIn,
            [CanBeNull] Func<TValue, TListItem> convertOut = null
            )
        {
            if (bindable == null) throw new ArgumentNullException(nameof(bindable));
            if (lst == null) throw new ArgumentNullException(nameof(lst));
            if (convertIn == null) throw new ArgumentNullException(nameof(convertIn));

            bool isSetting = false;
            Func<TValue> getter = () => convertIn((TListItem)lst.SelectedItem);
            Action<TValue> setter = b =>
            {
                isSetting = true;
                try
                {
                    if (!EqualityComparer<TValue>.Default.Equals(convertIn((TListItem)lst.SelectedItem), b))
                        lst.SelectedItem = convertOut != null ? (object)convertOut(b) : b;
                }
                finally
                {
                    isSetting = false;
                }
            };
            lst.SelectedIndexChanged += (sender, args) =>
            {
                if (!isSetting)
                    bindable.Value = getter();
            };
            return bindable.Bind(getter, setter);
        }

        public static BindResult BindControl([NotNull] this Bindable<string> bindable, [NotNull] TextBox txt)
        {
            if (bindable == null) throw new ArgumentNullException(nameof(bindable));
            if (txt == null) throw new ArgumentNullException(nameof(txt));

            bool isSetting = false;
            Func<string> getter = () => txt.Text;
            Action<string> setter = b =>
            {
                isSetting = true;
                try
                {
                    int oldSelStart = txt.SelectionStart;
                    int oldSelLength = txt.SelectionLength;
                    var areEqual = txt.Text == b;

                    if (txt.Text != b)
                        txt.Text = b;

                    if (areEqual)
                    {
                        txt.SelectionStart = oldSelStart;
                        txt.SelectionLength = oldSelLength;
                    }
                }
                finally
                {
                    isSetting = false;
                }
            };
            txt.TextChanged += (sender, args) =>
            {
                if (!isSetting)
                    bindable.Value = getter();
            };
            return bindable.Bind(getter, setter);
        }

        public static BindResult BindControl([NotNull] this Bindable<string> bindable, [NotNull] ComboBox cmb)
        {
            if (bindable == null) throw new ArgumentNullException(nameof(bindable));
            if (cmb == null) throw new ArgumentNullException(nameof(cmb));

            Func<string> getter = () => cmb.Text;
            Action<string> setter = b =>
            {
                int oldSelStart = cmb.SelectionStart;
                int oldSelLength = cmb.SelectionLength;
                var areEqual = cmb.Text == b;
                cmb.Text = b;
                if (areEqual && cmb.DropDownStyle != ComboBoxStyle.DropDownList)
                {
                    cmb.SelectionStart = oldSelStart;
                    cmb.SelectionLength = oldSelLength;
                }
            };
            cmb.TextChanged += (sender, args) =>
            {
                if (cmb.DropDownStyle != ComboBoxStyle.DropDownList)
                    bindable.Value = getter();
            };
            cmb.SelectedIndexChanged += (sender, args) =>
            {
                if (cmb.DropDownStyle == ComboBoxStyle.DropDownList)
                    bindable.Value = getter();
            };
            return bindable.Bind(getter, setter);
        }

        public static BindResult BindControl(
            [NotNull] this Bindable<string> bindable,
            [NotNull] ComboBox cmb,
            [NotNull] Func<string, string> convertIn,
            [CanBeNull] Func<string, string> convertOut = null
            )
        {
            if (bindable == null) throw new ArgumentNullException(nameof(bindable));
            if (cmb == null) throw new ArgumentNullException(nameof(cmb));

            bool isSetting = false;
            Func<string> getter = () => convertIn(cmb.Text);
            Action<string> setter = b =>
            {
                int oldSelStart = cmb.SelectionStart;
                int oldSelLength = cmb.SelectionLength;
                var areEqual = convertIn(cmb.Text) == b;

                isSetting = true;
                try
                {
                    cmb.Text = convertOut != null ? convertOut(b) : b;
                }
                finally
                {
                    isSetting = false;
                }

                if (areEqual && cmb.DropDownStyle != ComboBoxStyle.DropDownList)
                {
                    cmb.SelectionStart = oldSelStart;
                    cmb.SelectionLength = oldSelLength;
                }
            };
            cmb.TextChanged += (sender, args) =>
            {
                if (!isSetting)
                    if (cmb.DropDownStyle != ComboBoxStyle.DropDownList)
                        bindable.Value = getter();
            };
            cmb.SelectedIndexChanged += (sender, args) =>
            {
                if (!isSetting)
                    if (cmb.DropDownStyle == ComboBoxStyle.DropDownList)
                        bindable.Value = getter();
            };
            return bindable.Bind(getter, setter);
        }

        /// <summary>
        /// Binds the value of the bindable object with an expression that may contain other bindable references.
        /// Whenever one of the bindable references changes, the value of the expression will be assigned to the bindable object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindable">To bindable object that will be assigned to the expression.</param>
        /// <param name="valueExpression">Expression containing bindable references that is used as the value for the bindable object.</param>
        /// <param name="init">
        /// If true, assigns the value of the expression to the bindable object using data from the bindable references if they contain data already.
        /// If false, returns a delegate that does the same. In this case, if the delegate is not called
        /// then the bindable object and the external data source will not be syncrhonized until
        /// an event is raised from either objects.
        /// </param>
        /// <returns>
        /// If <see cref="init"/> is true, returns a delegate to assign the value of the expression the the bindable for the first time;
        /// otherwise it returns null.
        /// </returns>
        public static BindResult BindExpression<T>([NotNull] this Bindable<T> bindable,
            [NotNull] Expression<Func<T>> valueExpression, bool init = false)
        {
            if (bindable == null) throw new ArgumentNullException(nameof(bindable));
            if (valueExpression == null) throw new ArgumentNullException(nameof(valueExpression));

            // finding all bindables in the expression
            var finder = new BindableFinder<T>(bindable, valueExpression, init);
            finder.Visit(valueExpression);

            if (finder.BindablesFound.Count != 0)
                return finder.BindResult;

            var expr = valueExpression.Compile();
            Action initAction = () => bindable.Value = expr();
            if (init)
            {
                initAction();
                initAction = null;
            }

            return new BindResult(initAction, null);
        }

        /// <summary>
        /// Binds an action with an expression containing references to bindable objects.
        /// Whenever one of the bindable references changes, the action is called passing the value of the whole expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueExpression">Expression containing bindable references that is used as the value for the action.</param>
        /// <param name="action">Action that will be called every time the expression changes.</param>
        /// <param name="init">
        /// If true, calls the action passing the value of the expression, if it contains data already.
        /// If false, returns a delegate that does the same. In this case, if the delegate is not called
        /// then the bindable references and the action will not be synchronized.
        /// </param>
        /// <returns>
        /// If <see cref="init"/> is true, returns a delegate to call the action for the first time;
        /// otherwise it returns null.
        /// </returns>
        public static BindResult BindExpression<T>([NotNull] Expression<Func<T>> valueExpression, [NotNull] Action<T> action, bool init = false)
        {
            if (valueExpression == null) throw new ArgumentNullException(nameof(valueExpression));
            if (action == null) throw new ArgumentNullException(nameof(action));

            // finding all bindables in the expression
            var finder = new BindableFinder<T>(action, valueExpression, init);
            finder.Visit(valueExpression);

            if (finder.BindablesFound.Count == 0)
                throw new ArgumentException("The expression must contain a bindable reference to bind to.", nameof(valueExpression));

            return finder.BindResult;
        }

        /// <summary>
        /// Binds an expression containing bindable references.
        /// Whenever one of the bindable references changes, the whole expression is executed.
        /// </summary>
        /// <param name="boundExpression">
        /// The expression to be bound to it's containing bindables.
        /// </param>
        /// <param name="init">
        /// If true, executes the bound expression with data from the bindable objects if they contain data already.
        /// If false, returns a delegate that does the same. In this case, if the delegate is not called
        /// then the bindable object and the external data source will not be syncrhonized until
        /// an event is raised from either objects.
        /// </param>
        /// <returns>
        /// If <see cref="init"/> is true, returns a delegate to do the first execution of the bound expression as a way of synchronization;
        /// otherwise it returns null.
        /// </returns>
        public static BindResult BindExpression([NotNull] Expression<Action> boundExpression, bool init = false)
        {
            if (boundExpression == null) throw new ArgumentNullException(nameof(boundExpression));

            // finding all bindables in the expression
            var finder = new BindableFinder<string>(boundExpression, init);
            finder.Visit(boundExpression);
            return finder.BindResult;
        }
    }
}