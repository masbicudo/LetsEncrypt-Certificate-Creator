using System.Linq;
using System.Windows.Forms;

namespace LetsEncryptAcmeReg
{
    public static class ControlExtensions
    {
        public static void SetItems<T>(this ComboBox cb, T[] array)
        {
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
    }
}