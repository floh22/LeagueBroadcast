using LeagueBroadcast.OperatingSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Common.Controllers
{
    class GameInputController
    {
        public GameInputController()
        {
            AppStateController.GameLoad += InitUI;
        }

        private async void InitUI(object sender, EventArgs e)
        {
            if(ConfigController.Component.Replay.UseAutoInitUI)
            {
                Process p = InputUtils.GetActiveProcess();
                Point cursorP = InputUtils.GetCursorPosition();
                InputUtils.SetForegroundWindow(IngameController.LeagueProcess.MainWindowHandle);
                await Task.Delay(20);
                InputUtils.SendInput((uint)InputUtils.InputStart.Length, InputUtils.InputStart, Marshal.SizeOf(typeof(InputUtils.Input)));
                await Task.Delay(20);
                InputUtils.SetForegroundWindow(p.MainWindowHandle);
                InputUtils.SetCursorPosition(cursorP);
            }
        }

        private static async void SendToLeague(InputUtils.Input[] Input)
        {
            if (IngameController.LeagueProcess == null)
                return;
            Process p = InputUtils.GetActiveProcess();
            Point cursorP = InputUtils.GetCursorPosition();
            InputUtils.SetForegroundWindow(IngameController.LeagueProcess.MainWindowHandle);
            await Task.Delay(50);
            InputUtils.SendInput((uint)Input.Length, Input, Marshal.SizeOf(typeof(InputUtils.Input)));
            await Task.Delay(50);
            InputUtils.SetForegroundWindow(p.MainWindowHandle);
            InputUtils.SetCursorPosition(cursorP);
        }
    }
}
