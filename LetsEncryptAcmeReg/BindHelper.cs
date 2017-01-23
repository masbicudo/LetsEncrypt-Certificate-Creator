using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public static class BindHelper
    {
        public static Action Bind(this Bindable<bool> bindable, CheckBox checkBox)
        {
            Func<bool> getter = () => checkBox.Checked;
            Action<bool> setter = b =>
            {
                if (checkBox.Checked != b)
                    checkBox.Checked = b;
            };
            checkBox.CheckedChanged += (sender, args) => bindable.Update();
            return bindable.Bind(getter, setter);
        }

        public static Action Bind(this Bindable<string> bindable, Control control)
        {
            Func<string> getter = () => control.Text;
            Action<string> setter = b =>
            {
                if (control.Text != b)
                    control.Text = b;
            };
            control.TextChanged += (sender, args) => bindable.Update();
            return bindable.Bind(getter, setter);
        }

        public static Action Bind<T, T2>(this Bindable<T> bindable, ComboBox cmb, Func<T2, T> convert)
        {
            Func<T> getter = () => convert((T2)cmb.SelectedItem);
            Action<T> setter = b =>
            {
                if (!EqualityComparer<T>.Default.Equals(convert((T2)cmb.SelectedItem), b))
                    cmb.SelectedItem = b;
            };
            cmb.TextChanged += (sender, args) => bindable.Update();
            return bindable.Bind(getter, setter);
        }

        public static Action Bind<T, T2>(this Bindable<T> bindable, ListBox lst, Func<T2, T> convert)
        {
            Func<T> getter = () => convert((T2)lst.SelectedItem);
            Action<T> setter = b =>
            {
                if (!EqualityComparer<T>.Default.Equals(convert((T2)lst.SelectedItem), b))
                    lst.SelectedItem = b;
            };
            lst.SelectedIndexChanged += (sender, args) => bindable.Update();
            return bindable.Bind(getter, setter);
        }

        public static Action Bind(this Bindable<string> bindable, TextBox txt)
        {
            Func<string> getter = () => txt.Text;
            Action<string> setter = b =>
            {
                int oldSelStart = txt.SelectionStart;
                int oldSelLength = txt.SelectionLength;
                var areEqual = txt.Text == b;
                txt.Text = b;
                if (areEqual)
                {
                    txt.SelectionStart = oldSelStart;
                    txt.SelectionLength = oldSelLength;
                }
            };
            txt.TextChanged += (sender, args) => bindable.Update();
            return bindable.Bind(getter, setter);
        }

        public static Action Bind(this Bindable<string> bindable, ComboBox cmb)
        {
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
            cmb.TextChanged += (sender, args) => bindable.Update();
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
        public static Action BindExpression<T>(this Bindable<T> bindable, Expression<Func<T>> valueExpression, bool init = false)
        {
            // finding all bindables in the expression
            var finder = new BindableFinder<T>(bindable, valueExpression, init);
            finder.Visit(valueExpression);

            var initAction = finder.InitAction;
            if (initAction == null)
            {
                var expr = valueExpression.Compile();
                initAction = () => bindable.Value = expr();
                if (init)
                {
                    initAction();
                    initAction = null;
                }
            }

            return initAction;
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
        public static Action BindExpression(Expression<Action> boundExpression, bool init = false)
        {
            // finding all bindables in the expression
            var finder = new BindableFinder<string>(boundExpression, init);
            finder.Visit(boundExpression);
            return finder.InitAction;
        }
    }
}