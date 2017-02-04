using System;
using System.Linq;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public static class ControlExtensions
    {
        public static void SetItems<T>(this ComboBox cb, T[] array)
        {

            if (array == null)
                return;

            cb.BeginUpdate();
            try
            {
                var oldSelected = cb.SelectedItem;
                cb.Items.Clear();
                cb.Items.AddRange(array.Cast<object>().ToArray());
                cb.SelectedItem = oldSelected;
            }
            finally
            {
                cb.EndUpdate();
            }
        }

        public static void SetItems<T>(this ListBox lb, T[] array)
        {
            lb.BeginUpdate();
            try
            {
                var oldSelected = lb.SelectedItem;
                lb.Items.Clear();
                lb.Items.AddRange(array.Cast<object>().ToArray());
                lb.SelectedItem = oldSelected;
            }
            finally
            {
                lb.EndUpdate();
            }
        }

        public static void SetItems<T>(this CheckedListBox lb, T[] items, T[] checkedItems)
        {
            lb.BeginUpdate();
            checkedItems = checkedItems ?? new T[0];
            try
            {
                lb.Items.Clear();
                lb.Items.AddRange(items.Cast<object>().ToArray());
            }
            finally
            {
                lb.EndUpdate();
            }
            foreach (var index in checkedItems.Select(i => Array.IndexOf(items, i)))
                lb.SetItemChecked(index, true);
        }
    }
}