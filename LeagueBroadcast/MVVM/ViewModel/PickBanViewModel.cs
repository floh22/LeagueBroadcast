using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.Core;

namespace LeagueBroadcast.MVVM.ViewModel
{
    public class PickBanViewModel : ObservableObject
    {
        public PickBanViewModel()
        {
            TeamConfigViewModel.BlueTeam.Init(ConfigController.PickBan.frontend.blueTeam);
            TeamConfigViewModel.RedTeam.Init(ConfigController.PickBan.frontend.redTeam);
        }
    }
}