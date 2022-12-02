using System;
using System.Text;
using System.Windows.Input;

namespace LeagueBroadcast.Client.MVVM.Core
{
    public readonly partial record struct HotKey(Key Key, ModifierKeys Modifiers = ModifierKeys.None, bool AllowPassthrough = false)
    {
        public override string ToString()
        {
            if (Key == Key.None && Modifiers == ModifierKeys.None)
                return "< None >";

            var buffer = new StringBuilder();

            if (Modifiers.HasFlag(ModifierKeys.Control))
                buffer.Append("Ctrl + ");
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                buffer.Append("Shift + ");
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                buffer.Append("Alt + ");
            if (Modifiers.HasFlag(ModifierKeys.Windows))
                buffer.Append("Win + ");

            buffer.Append(Key);

            return buffer.ToString();
        }
    }

    public partial record struct HotKey
    {
        public static HotKey None { get; } = new();
    }

    public class HotKeyWithAction
    {
        public int Id { get; set; }

        public HotKey HotKey { get; set; }
        public Action Action { get; set; }

        public bool AllowPassthrough { get; set; }

        public HotKeyWithAction(HotKey hotkey, Action action, int id, bool allowPassthrough = false)
        {
            this.HotKey = hotkey;
            this.Action = action;
            this.Id = id;
            this.AllowPassthrough = allowPassthrough;
        }

        public HotKeyWithAction(Key key, ModifierKeys modifier, Action action, int id, bool allowPassthrough = false)
        {
            this.HotKey = new(key, modifier);
            this.Action = action;
            this.Id = id;
            this.AllowPassthrough = allowPassthrough;
        }
    }
}
