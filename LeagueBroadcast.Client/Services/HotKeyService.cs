using LeagueBroadcast.Client.Utils;
using LeagueBroadcast.Common.Config;
using LeagueBroadcast.Utils.Log;

namespace LeagueBroadcast.Client.Services
{
    public static class HotKeyService
    {
        public static void RegisterHotKeys()
        {
            ClientConfig cfg = ConfigController.Get<ClientConfig>();

            cfg.HotKeys.PropertyChanged += (s, e) => {

                //HotKey changedKey = (HotKey)((typeof(HotKeySettings).GetField(e.PropertyName?? "")?? throw new FieldAccessException()).GetValue(cfg.HotKeys)?? throw new FieldAccessException());
                //GlobalHotKey.UpdateHotKeyById(changedKey., )
            };

            var success = GlobalHotKey.RegisterHotKey(cfg.HotKeys.SwapTeamSidesHotKey, () =>
            {
                $"Swapping Team sides DEBUG".Debug();
            });

            /*
            success = success && GlobalHotKey.RegisterHotKey(cfg.HotKeys.SwapTeamSidesHotKey, () =>
            {
                $"Swapping Team sides DEBUG".Debug();
            });

            */
            if (success)
                $"{GlobalHotKey.RegisteredHotKeys.Count} HotKeys registered".Info();
            else
                "Error registering Hotkeys".Info();
        }
    }
}
