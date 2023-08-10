using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Controllers;
using LeagueBroadcast.MVVM.Core;
using System.Windows.Input;

namespace LeagueBroadcast.MVVM.ViewModel
{
    class IngameViewModel : ObservableObject
    {
        public ObjectivesTabViewModel Objectives = new();
        public PlayersTabViewModel Players = new();
        public TeamsTabViewModel Teams = new();

        public IngameTeamsViewModel IngameTeamsVM { get; set; }

        public bool IsActive
        {
            get => ConfigController.Component.Ingame.IsActive;
            //TODO Update App state on change
            set { ConfigController.Component.Ingame.IsActive = value; OnPropertyChanged(); }
        }

        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            set { _isOpen = value; OnPropertyChanged(); }
        }

        private DelegateCommand _openCommand;

        public DelegateCommand OpenCommand => _openCommand;

        private DelegateCommand _closeCommand;

        public DelegateCommand CloseCommand
        {
            get => _closeCommand;
            set => _closeCommand = value;
        }

        private bool _teamsIsOpen;

        public bool TeamsIsOpen
        {
            get => _teamsIsOpen;
            set { _teamsIsOpen = value; OnPropertyChanged(); }
        }

        private DelegateCommand _teamsButtonCommand;

        public DelegateCommand TeamsButtonCommand
        {
            get => _teamsButtonCommand;
            set => _teamsButtonCommand = value;
        }


        public IngameViewModel()
        {
            _openCommand = new(o =>
            {
                IsOpen = true;
                BroadcastController.Instance.Main.SetIngameSelected();
                MainViewModel.HomeVM.InfoButtonIsVisible = false;
                MainViewModel.HomeVM.InfoIsOpen = false;
            });
            _openCommand.MouseGesture = MouseAction.LeftClick;

            _closeCommand = new(o => { IsOpen = false; });
            _closeCommand.GestureKey = Key.Escape;

            _teamsButtonCommand = new(o => { TeamsIsOpen ^= true; Log.Verbose("Switch Team View visibility"); });
            _teamsButtonCommand.MouseGesture = MouseAction.LeftClick;

            IngameTeamsVM = new();
        }
    }

    public class ObjectivesTabViewModel : ObservableObject
    {
        public ControlButtonViewModel BaronPowerPlay = new("Baron PowerPlay", "Show Timer and Gold difference when Baron is taken", ConfigController.Component.Ingame.UseLiveEvents);
        public ControlButtonViewModel ElderPowerPlay = new("Elder PowerPlay", "Show Timer and Gold difference when Elder Drake is taken", ConfigController.Component.Ingame.UseLiveEvents);
        public ControlButtonViewModel InhibTimer = new("Inhibitor Timers", "Show Inhibitor Timers", true);
        public ControlButtonViewModel ObjectiveSpawn = new("Spawn", "Show Spawn PopUp when an objective spawns", ConfigController.Component.Ingame.UseLiveEvents);
        public ControlButtonViewModel ObjectiveKill = new("Kill", "Show Spawn PopUp when an objective is killed", ConfigController.Component.Ingame.UseLiveEvents);
        public ControlButtonViewModel DragonTimer = new("Dragon", "Show Dragon Timers", ConfigController.Component.Ingame.UseLiveEvents);
        public ControlButtonViewModel BaronTimer = new("Baron", "Show Baron Timers", ConfigController.Component.Ingame.UseLiveEvents);

        public bool BaronIsActive { get => ConfigController.Component.Ingame.Objectives.DoBaronKill; set { ConfigController.Component.Ingame.Objectives.DoBaronKill = value; OnPropertyChanged(); } }
        public bool ElderIsActive { get => ConfigController.Component.Ingame.Objectives.DoDragonKill; set { ConfigController.Component.Ingame.Objectives.DoDragonKill = value; OnPropertyChanged(); } }
        public bool InhibIsActive { get => ConfigController.Component.Ingame.Objectives.DoInhibitors; set { ConfigController.Component.Ingame.Objectives.DoInhibitors = value; OnPropertyChanged(); } }
        public bool ObjectiveSpawnPopUpIsActive { get => ConfigController.Component.Ingame.Objectives.DoObjectiveSpawnPopUp; set { ConfigController.Component.Ingame.Objectives.DoObjectiveSpawnPopUp = value; OnPropertyChanged(); } }
        public bool ObjectiveKillPopUpIsActive { get => ConfigController.Component.Ingame.Objectives.DoObjectiveKillPopUp; set { ConfigController.Component.Ingame.Objectives.DoObjectiveKillPopUp = value; OnPropertyChanged(); } }
        public bool DragonTimerIsActive { get => ConfigController.Component.Ingame.Objectives.UseCustomDragonTimer; set { ConfigController.Component.Ingame.Objectives.UseCustomDragonTimer = value; OnPropertyChanged(); } }
        public bool BaronTimerIsActive { get => ConfigController.Component.Ingame.Objectives.UseCustomBaronTimer; set { ConfigController.Component.Ingame.Objectives.UseCustomBaronTimer = value; OnPropertyChanged(); } }

        private DelegateCommand _baronClickCommand;

        public DelegateCommand BaronClickCommand
        {
            get => _baronClickCommand;
            set => _baronClickCommand = value;
        }

        private DelegateCommand _dragonClickCommand;

        public DelegateCommand DragonClickCommand
        {
            get => _dragonClickCommand;
            set => _dragonClickCommand = value;
        }

        private DelegateCommand _inhibClickCommand;

        public DelegateCommand InhibClickCommand
        {
            get => _inhibClickCommand;
            set => _inhibClickCommand = value;
        }

        private DelegateCommand _objectiveSpawnClickCommand;

        public DelegateCommand ObjectiveSpawnClickCommand
        {
            get => _objectiveSpawnClickCommand;
            set => _objectiveSpawnClickCommand = value;
        }

        private DelegateCommand _objectiveKillClickCommand;

        public DelegateCommand ObjectiveKillClickCommand
        {
            get => _objectiveKillClickCommand;
            set => _objectiveKillClickCommand = value;
        }

        private DelegateCommand _dragonTimerClickCommand;
        public DelegateCommand DragonTimerClickCommand
        {
            get => _dragonTimerClickCommand;
            set => _dragonTimerClickCommand = value;
        }

        private DelegateCommand _baronTimerClickCommand;
        public DelegateCommand BaronTimerClickCommand
        {
            get => _baronTimerClickCommand;
            set => _baronTimerClickCommand = value;
        }


        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            set => _isOpen = value;
        }


        public ObjectivesTabViewModel()
        {
            _baronClickCommand = new(o =>
            {
                BaronIsActive ^= true;
            });
            _baronClickCommand.MouseGesture = MouseAction.LeftClick;

            _dragonClickCommand = new(o =>
            {
                ElderIsActive ^= true;
            });
            _dragonClickCommand.MouseGesture = MouseAction.LeftClick;

            _inhibClickCommand = new(o =>
            {
                InhibIsActive ^= true;
            });
            _inhibClickCommand.MouseGesture = MouseAction.LeftClick;

            _objectiveKillClickCommand = new(o =>
            {
                ObjectiveKillPopUpIsActive ^= true;
            });
            _objectiveKillClickCommand.MouseGesture = MouseAction.LeftClick;

            _objectiveSpawnClickCommand = new(o =>
            {
                ObjectiveSpawnPopUpIsActive ^= true;
            });
            _objectiveSpawnClickCommand.MouseGesture = MouseAction.LeftClick;

            _dragonTimerClickCommand = new(o =>
            {
                DragonTimerIsActive ^= true;
            });
            _dragonTimerClickCommand.MouseGesture = MouseAction.LeftClick;

            _baronTimerClickCommand = new(o =>
            {
                BaronTimerIsActive ^= true;
            });
            _baronTimerClickCommand.MouseGesture = MouseAction.LeftClick;
        }
    }

    public class PlayersTabViewModel : ObservableObject
    {
        public ControlButtonViewModel Items = new("Item Notifications", "Show Item pop up over player icons when item is completed", ConfigController.Component.Ingame.DoItemCompleted);
        public ControlButtonViewModel LevelUp = new("Level Up Notifications", "Show Lvl Up pop up @ lvls 6/11/16", ConfigController.Component.Ingame.DoLevelUp);
        public ControlButtonViewModel EXP = new("EXP Tab", "EXP and Level per Player scoreboard", true);
        public ControlButtonViewModel PlayerGold = new("Gold Tab", "Gold per Player scoreboard", true);
        public ControlButtonViewModel PlayerCSperMin = new("CS per Min.", "CS per Minute per Player scoreboard", true);

        public bool ItemsIsActive { get => ConfigController.Component.Ingame.DoItemCompleted; set { ConfigController.Component.Ingame.DoItemCompleted = value; OnPropertyChanged(); } }
        public bool LevelUpIsActive { get => ConfigController.Component.Ingame.DoLevelUp; set { ConfigController.Component.Ingame.DoLevelUp = value; OnPropertyChanged(); } }
        public bool EXPIsActive { get => IngameController.CurrentSettings.EXP; set { IngameController.CurrentSettings.EXP = value; OnPropertyChanged(); } }
        public bool PlayerGoldIsActive { get => IngameController.CurrentSettings.PlayerGold; set { IngameController.CurrentSettings.PlayerGold = value; OnPropertyChanged(); } }
        public bool PlayerCSPerMinIsActive { get => IngameController.CurrentSettings.CSPerMin; set { IngameController.CurrentSettings.CSPerMin = value; OnPropertyChanged(); } }

        private DelegateCommand _itemsButtonCommand;

        public DelegateCommand ItemsButtonCommand
        {
            get => _itemsButtonCommand;
            set => _itemsButtonCommand = value;
        }

        private DelegateCommand _levelUpButtonCommand;

        public DelegateCommand LevelUpButtonCommand
        {
            get => _levelUpButtonCommand;
            set => _levelUpButtonCommand = value;
        }

        private DelegateCommand _expButtonCommand;

        public DelegateCommand ExpButtonCommand
        {
            get => _expButtonCommand;
            set => _expButtonCommand = value;
        }

        private DelegateCommand _playerGoldButtonCommand;

        public DelegateCommand PlayerGoldButtonCommand
        {
            get => _playerGoldButtonCommand;
            set => _playerGoldButtonCommand = value;
        }

        private DelegateCommand _csPerMinButtonCommand;

        public DelegateCommand CsPerMinButtonCommand
        {
            get => _csPerMinButtonCommand;
            set => _csPerMinButtonCommand = value;
        }


        private bool _isOpen;

        public bool IsOpen
        {
            get => _isOpen;
            set => _isOpen = value;
        }

        public PlayersTabViewModel()
        {
            _itemsButtonCommand = new(o =>
            {
                ItemsIsActive ^= true;
            });
            _itemsButtonCommand.MouseGesture = MouseAction.LeftClick;

            _levelUpButtonCommand = new(o =>
            {
                LevelUpIsActive ^= true;
            });
            _levelUpButtonCommand.MouseGesture = MouseAction.LeftClick;

            _expButtonCommand = new(o =>
            {
                EXPIsActive ^= true;
            });
            _expButtonCommand.MouseGesture = MouseAction.LeftClick;

            _playerGoldButtonCommand = new(o =>
            {
                PlayerGoldIsActive ^= true;
            });
            _playerGoldButtonCommand.MouseGesture = MouseAction.LeftClick;

            _csPerMinButtonCommand = new(o =>
            {
                PlayerCSPerMinIsActive ^= true;
            });
            _csPerMinButtonCommand.MouseGesture = MouseAction.LeftClick;
        }
    }

    public class TeamsTabViewModel : ObservableObject
    {
        public ControlButtonViewModel Name = new("Team Names", "Show Item pop up over player icons when item is completed", ConfigController.Component.Ingame.Teams.DoTeamNames);
        public ControlButtonViewModel Score = new("Team Scores", "Show Item pop up over player icons when item is completed", ConfigController.Component.Ingame.Teams.DoTeamScores);
        public ControlButtonViewModel Icon = new("Team Icons", "Show Item pop up over player icons when item is completed", ConfigController.Component.Ingame.Teams.DoTeamIcons);
        public ControlButtonViewModel Gold = new("Gold Graph", "Scoreboard team gold difference graph since the start of the game", IngameController.CurrentSettings.GoldGraph);
        public ControlButtonViewModel Scoreboard = new("Scoreboard", "Use custom scoreboard", ConfigController.Component.Ingame.UseCustomScoreboard);

        public bool NamesIsActive { get => ConfigController.Component.Ingame.Teams.DoTeamNames; set { ConfigController.Component.Ingame.Teams.DoTeamNames = value; OnPropertyChanged(); } }
        public bool ScoresIsActive { get => ConfigController.Component.Ingame.Teams.DoTeamScores; set { ConfigController.Component.Ingame.Teams.DoTeamScores = value; OnPropertyChanged(); } }
        public bool IconsIsActive { get => ConfigController.Component.Ingame.Teams.DoTeamIcons; set { ConfigController.Component.Ingame.Teams.DoTeamIcons = value; OnPropertyChanged(); } }
        public bool ScoreboardIsActive { get => ConfigController.Component.Ingame.UseCustomScoreboard; set { ConfigController.Component.Ingame.UseCustomScoreboard = value; OnPropertyChanged(); } }

        public bool GoldGraphIsActive { get => IngameController.CurrentSettings.GoldGraph; set { IngameController.CurrentSettings.GoldGraph = value; OnPropertyChanged(); } }

        private DelegateCommand _namesButtonCommand;

        public DelegateCommand NamesButtonCommand
        {
            get => _namesButtonCommand;
            set => _namesButtonCommand = value;
        }

        private DelegateCommand _scoresButtonCommand;

        public DelegateCommand ScoresButtonCommand
        {
            get => _scoresButtonCommand;
            set => _scoresButtonCommand = value;
        }

        private DelegateCommand _scoreboardButtonCommand;

        public DelegateCommand ScoreboardButtonCommand
        {
            get => _scoreboardButtonCommand;
            set => _scoreboardButtonCommand = value;
        }

        private DelegateCommand _iconsButtonCommand;

        public DelegateCommand IconsButtonCommand
        {
            get => _iconsButtonCommand;
            set => _iconsButtonCommand = value;
        }

        private DelegateCommand _goldGraphButtonCommand;

        public DelegateCommand GoldGraphButtonCommand
        {
            get => _goldGraphButtonCommand;
            set => _goldGraphButtonCommand = value;
        }


        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }


        public TeamsTabViewModel()
        {
            _namesButtonCommand = new(o =>
            {
                NamesIsActive ^= true;
            });
            _namesButtonCommand.MouseGesture = MouseAction.LeftClick;

            _scoresButtonCommand = new(o =>
            {
                ScoresIsActive ^= true;
            });
            _scoresButtonCommand.MouseGesture = MouseAction.LeftClick;

            _iconsButtonCommand = new(o =>
            {
                IconsIsActive ^= true;
            });
            _iconsButtonCommand.MouseGesture = MouseAction.LeftClick;

            _goldGraphButtonCommand = new(o =>
            {
                GoldGraphIsActive ^= true;
            });
            _goldGraphButtonCommand.MouseGesture = MouseAction.LeftClick;

            _scoreboardButtonCommand = new(o =>
            {
                ScoreboardIsActive ^= true;
            });
            _scoreboardButtonCommand.MouseGesture = MouseAction.LeftClick;
        }

    }

    public class ControlButtonViewModel : ObservableObject
    {

        private string _title;

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _description;

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        public ControlButtonViewModel(string title, string desc, bool isEnabled)
        {
            this._isEnabled = isEnabled;
            this._title = title;
            this._description = desc;
        }

    }
}
