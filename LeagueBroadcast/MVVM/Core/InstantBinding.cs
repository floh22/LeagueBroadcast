using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace LeagueBroadcast.MVVM.Core
{
    class InstantBinding : Binding
    {
        public InstantBinding(string path) : base(path)
        {
            this.Mode = BindingMode.TwoWay;
            this.ValidatesOnNotifyDataErrors = true;
            this.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
    }

    class LostFocusBinding : Binding
    {
        public LostFocusBinding(string path) : base(path)
        {
            this.Mode = BindingMode.TwoWay;
            this.ValidatesOnNotifyDataErrors = true;
            this.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
        }
    }
}
