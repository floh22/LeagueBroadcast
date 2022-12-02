using LeagueBroadcast.Client.MVVM.Core;
using LeagueBroadcast.Utils.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Interop;

namespace LeagueBroadcast.Client.Utils
{
    public class GlobalHotKey : IDisposable
    {
        private static int _currentHotKeyId;
        private static WindowInteropHelper _windowInteropHelper;

        private static HwndSource _source;
        private const int WM_HOTKEY = 0x0312;

        public static Dictionary<int, HotKeyWithAction> RegisteredHotKeys;

        static GlobalHotKey()
        {
            if(ClientController.Instance.StartupWin is null)
            {
                $"Attempted to register HotKey before load complete!".Error();
            }
            _windowInteropHelper = new WindowInteropHelper(ClientController.Instance.StartupWin);
            _source = HwndSource.FromHwnd(_windowInteropHelper.Handle);
            _source.AddHook(HwndHook);

            RegisteredHotKeys = new();
        }
        public static bool RegisterHotKey(HotKey hotKey, Action? a, bool allowPassthrough = false)
        {
            try
            {
                return RegisterHotKey(nameof(hotKey), hotKey, a, allowPassthrough);
            } catch(Exception e)
            {
                $"[HotKey] Attempted to load existing HotHey. Unregistering now, but this hothey will not work".Warn();
                $"{e.Source}: {e.Message}\nStacktrace: {e.StackTrace}".Debug();
                ForceUnregisterHotKey(KeyInterop.VirtualKeyFromKey(hotKey.Key));

                return false;
            }
            
        }
        public static bool RegisterHotKey(string name, HotKey hotKey, Action? a, bool allowPassthrough)
        {
            try
            {
                int aVirtualKeyCode = KeyInterop.VirtualKeyFromKey(hotKey.Key);
                bool res = Win32.RegisterHotKey(_windowInteropHelper.Handle, ++_currentHotKeyId, (uint)hotKey.Modifiers, (uint)aVirtualKeyCode);
                if (!res)
                    return false;

                RegisteredHotKeys.Add(_currentHotKeyId, new HotKeyWithAction(name, _currentHotKeyId, hotKey, a));
                return true;
            } catch(Exception e)
            {
                $"[HotKey] Attempted to load existing HotHey. Unregistering now, but this hothey will not work".Warn();
                $"{e.Source}: {e.Message}\nStacktrace: {e.StackTrace}".Debug();
                ForceUnregisterHotKey(KeyInterop.VirtualKeyFromKey(hotKey.Key));

                return false;
            }
        }

        public static bool UnregisterHotKey(int id)
        {
            bool res = RegisteredHotKeys.Remove(id);
            if (!res)
                return false;
            return Win32.UnregisterHotKey(_windowInteropHelper.Handle, id);
        }

        public static bool UnregisterHotKey(string name)
        {
            try
            {
                int id = (RegisteredHotKeys.Values.SingleOrDefault(hotkey => hotkey.Name == name)?? throw new ArgumentException($"Attempted to unregister unknow HotKey `{name}`")).Id;
                bool res = RegisteredHotKeys.Remove(id);
                if (!res)
                    return false;

                return Win32.UnregisterHotKey(_windowInteropHelper.Handle, id);
            }
            catch (ArgumentException e)
            {
                e.Message.Debug();
                return false;
            }
        }

        private static bool ForceUnregisterHotKey(int id)
        {
            try
            {
                return Win32.UnregisterHotKey(_windowInteropHelper.Handle, id);
            } catch
            {
                $"[HotKey] Could not unregister HotKey {id}".Warn();
                return false;
            }
        }

        public static bool UnregisterAllHotKeys()
        {
            bool res = RegisteredHotKeys.Keys.All(id => { return Win32.UnregisterHotKey(_windowInteropHelper.Handle, id); });
            RegisteredHotKeys.Clear();
            return res;
        }

        private static IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                HotKeyWithAction? found = RegisteredHotKeys.GetValueOrDefault(wParam.ToInt32());
                if (found is not null)
                {
                    found.Action?.Invoke();
                    if(!found.HotKey.AllowPassthrough)
                        handled = true;
                }
            }
            return IntPtr.Zero;
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _source.RemoveHook(HwndHook);
            UnregisterAllHotKeys();
        }
    }

    public class HotKeyWithAction : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private readonly int _id;

        public int Id { get { return _id; } }

        private HotKey _hotKey;

        public HotKey HotKey
        {
            get { return _hotKey; }
            set { _hotKey = value; }
        }

        private Action? _action;

        public Action? Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public HotKeyWithAction(string name, int id, HotKey hotkey, Action? action = null)
        {
            _name = name;
            _action = action;
            _hotKey = hotkey;   
            _id = id;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
