using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HRM.Library.AppScopeClasses
{
    static class KeyHandler
    {
        public static void NumericOnly(KeyEventArgs e)
        {
            int key = KeyInterop.VirtualKeyFromKey(e.Key);
            if (key == 8 || key == 46 || key==13 || (key >= 37 && key<=40))
                return;
            if ((int)key < 48 || (int)key > 57)
                e.Handled = true;
        }
    }
}
