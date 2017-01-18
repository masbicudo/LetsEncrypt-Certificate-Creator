using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public static class BindHelper
    {
        public static Action Bind(Bindable<bool> bindable, CheckBox checkBox)
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

        public static Action Bind(Bindable<string> bindable, Control control)
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

        public static Action Bind<T, T2>(Bindable<T> bindable, ComboBox cmb, Func<T2, T> convert)
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

        public static Action Bind<T, T2>(Bindable<T> bindable, ListBox lst, Func<T2, T> convert)
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

        public static Action Bind(Bindable<string> bindable, TextBox txt)
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

        public static Action Bind(Bindable<string> bindable, ComboBox cmb)
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

        public static Action Bind<T>(Bindable<T> bindable, Func<T> getter, Action<T> setter)
        {
            return bindable.Bind(getter, setter);
        }

        public static void Assign<T>(Bindable<T> bindable, Expression<Func<T>> exprValue)
        {
            // finding all bindables in the expression
            var finder = new BindableFinder<T>(bindable, exprValue);
            finder.Visit(exprValue);
        }
    }
}