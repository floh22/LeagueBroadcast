using LeagueBroadcast.Common;
using LeagueBroadcast.Common.Data.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LeagueBroadcast.Ingame.Data.Config
{
    public class IngameConfig : JSONConfig
    {
        public override string Name => "Ingame";

        public override string FileVersion { get => _fileVersion; set => _fileVersion = value; }

        [JsonIgnore]
        public static new string CurrentVersion => "2.0";

        public InhibitorDisplayConfig Inhib;
        public ScoreDisplayConfig Score;
        public ObjectiveKillConfig ObjectiveKill;
        public ItemCompletedDisplayConfig ItemComplete;
        public LevelUpDisplayConfig LevelUp;
        public InfoPageDisplayConfig InfoPage;
        public ObjectiveTimerDisplayConfig BaronTimer;
        public ObjectiveTimerDisplayConfig ElderTimer;

        public override string GETCurrentVersion()
        {
            return CurrentVersion;
        }

        public override string GETJson()
        {
            return SerializeIndented(this);
        }

        public override void UpdateValues(string readValues)
        {
            var Cfg = JsonConvert.DeserializeObject<IngameConfig>(readValues);
            Inhib = Cfg.Inhib;
            Score = Cfg.Score;
            InfoPage = Cfg.InfoPage;
            ItemComplete = Cfg.ItemComplete;
            LevelUp = Cfg.LevelUp;
            ObjectiveKill = Cfg.ObjectiveKill;
            BaronTimer = Cfg.BaronTimer;
            ElderTimer = Cfg.ElderTimer;
            FileVersion = Cfg.FileVersion;
        }

        public override string GETDefaultString()
        {
            return JsonConvert.SerializeObject(CreateDefault(), Formatting.Indented);
        }

        public override void RevertToDefault()
        {
            var def = CreateDefault();
            this.Inhib = def.Inhib;
            this.Score = def.Score;
            this.InfoPage = def.InfoPage;
            this.ItemComplete = def.ItemComplete;
            this.LevelUp = def.LevelUp;
            this.ObjectiveKill = def.ObjectiveKill;
            this.BaronTimer = def.BaronTimer;
            this.ElderTimer = def.ElderTimer;
            this.FileVersion = CurrentVersion;

        }

        private IngameConfig CreateDefault()
        {
            return new IngameConfig()
            {
                FileVersion = CurrentVersion,
                Inhib = new InhibitorDisplayConfig() {
                    Position = new Vector2(0, 845),
                    Size = new Vector2(306, 118),
                    Font = new FontConfig()
                    {
                        Align = "right",
                        Color = "rgb(255,255,255)",
                        IsGoogleFont = true,
                        Name = "News Cycle",
                        Size = "15px",
                        Style = "Normal"
                    },
                    IconSize = 40f,
                    UseBackgroundImage = false,
                    UseBackgroundVideo = false,
                    Color = "rgba(19,24,63,255)",
                    HideWhenNoneDestroyed = true,
                    UseTeamColors = true,
                    BlueTeam = new InhibitorDisplayConfig.InhibTeamConfig() {
                        UseTeamColor = true,
                        Color = "rgb(0,0,0)",
                        IconOffset = new Vector2(10, -24),
                        LaneOffset = new Vector2(100, 0),
                        Position = new Vector2(40, 20)
                    },
                    RedTeam = new InhibitorDisplayConfig.InhibTeamConfig()
                    {
                        UseTeamColor = true,
                        Color = "rgb(255,255,255)",
                        IconOffset = new Vector2(10, -24),
                        LaneOffset = new Vector2(100, 0),
                        Position = new Vector2(40, 80)
                    },
                    LaneOrder = new List<string>() { "bot", "mid", "top" }
                },
                Score = new ScoreDisplayConfig()
                {
                    Position = new Vector2(960, 0),
                    Size = new Vector2(800, 100),
                    TimeFont = new FontConfig()
                    {
                        Name = "News Cycle",
                        IsGoogleFont = true,
                        Size = "22px",
                        Style = "Normal",
                        Color = "rgb(255,255,255)",
                        Align = "center"
                    },
                    TimePosition = new Vector2(0, 70),
                    BlueTeam = new ScoreDisplayConfig.TeamConfig()
                    {
                        Score = new ScoreDisplayConfig.TeamConfig.TeamScoreDisplay()
                        {
                            Position = new Vector2(-380, 70),
                            NumberFont = new FontConfig()
                            {
                                Align = "right",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "15px",
                                Style = "Normal"
                            },
                            UseCircleIcons = true,
                            CircleOffset = new Vector2(25, 0),
                            CircleRadius = 10,
                            UseTeamColor = true,
                            FillColor = "rgb(255,255,255)",
                            StrokeColor = "rgb(255,255,255)"
                        },
                        Drakes = new ScoreDisplayConfig.DrakeConfig()
                        {
                            Offset = new Vector2(-12, 0),
                            Position = new Vector2(-55, 71)
                        },
                        Gold = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "right",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "26px",
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "GoldIcon",
                                Offset = new Vector2(25, 10),
                                Size = new Vector2(40, 30)
                            },
                            Position = new Vector2(-150, 20)
                        },
                        Icon = new ScoreDisplayConfig.TeamConfig.TeamIcon()
                        {
                            Position = new Vector2(-430, 30),
                            Size = new Vector2(60, 60),
                            UseBackground = true,
                            BackgroundOffset = new Vector2(-10, 0)
                        },
                        Kills = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "right",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "50px",
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "KillsIcon",
                                Offset = new Vector2(0, 0),
                                Size = new Vector2(100, 100)
                            },
                            Position = new Vector2(-40, 6)
                        },
                        Name = new ScoreDisplayConfig.TeamConfig.TeamName()
                        {
                            Font = new FontConfig()
                            {
                                Align = "left",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "40px",
                                Style = "Normal"
                            },
                            Position = new Vector2(-390, 6),
                            MaxSize = new Vector2(100, 50),
                            UseTag = true,
                            AdaptiveFontSize = true
                        },
                        Towers = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "right",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "26px",
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "TowerIcon",
                                Offset = new Vector2(15, 13),
                                Size = new Vector2(25, 30)
                            },
                            Position = new Vector2(-270, 20)
                        }
                    },
                    RedTeam = new ScoreDisplayConfig.TeamConfig()
                    {
                        Score = new ScoreDisplayConfig.TeamConfig.TeamScoreDisplay()
                        {
                            Position = new Vector2(380, 70),
                            NumberFont = new FontConfig()
                            {
                                Align = "left",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "15px",
                                Style = "Normal"
                            },
                            UseCircleIcons = true,
                            CircleOffset = new Vector2(-25, 0),
                            CircleRadius = 10,
                            UseTeamColor = true,
                            FillColor = "rgb(255,255,255)",
                            StrokeColor = "rgb(255,255,255)"
                        },
                        Drakes = new ScoreDisplayConfig.DrakeConfig()
                        {
                            Offset = new Vector2(12, 0),
                            Position = new Vector2(50, 71)
                        },
                        Gold = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "left",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "26px",
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "GoldIcon",
                                Offset = new Vector2(-25, 10),
                                Size = new Vector2(40, 30)
                            },
                            Position = new Vector2(150, 20)
                        },
                        Icon = new ScoreDisplayConfig.TeamConfig.TeamIcon()
                        {
                            Position = new Vector2(430, 30),
                            Size = new Vector2(60, 60),
                            UseBackground = true,
                            BackgroundOffset = new Vector2(10, 0)
                        },
                        Kills = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "left",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "50px",
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "KillsIcon",
                                Offset = new Vector2(0, 0),
                                Size = new Vector2(100, 100)
                            },
                            Position = new Vector2(40, 6)
                        },
                        Name = new ScoreDisplayConfig.TeamConfig.TeamName()
                        {
                            Font = new FontConfig()
                            {
                                Align = "right",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "40px",
                                Style = "Normal"
                            },
                            Position = new Vector2(390, 6),
                            MaxSize = new Vector2(100, 50),
                            UseTag = true,
                            AdaptiveFontSize = true
                        },
                        Towers = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "left",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "26px",
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "TowerIcon",
                                Offset = new Vector2(-15, 13),
                                Size = new Vector2(25, 30)
                            },
                            Position = new Vector2(270, 20)
                        }
                    },
                    Background = new BackgroundDisplayConfig()
                    {
                        UseImage = false,
                        UseVideo = false,
                        UseAlpha = true,
                        FallbackColor = "rgba(19,24,63,255)"
                    },
                    Misc = new ScoreDisplayConfig.ExtraConfigs()
                    {
                        ShowCenterIcon = true,
                        //None, Simple, Fancy
                        Animation = "Simple",
                        DrakeIconSize = new Vector2(20, 20),
                        CenterIconPosition = new Vector2(0, 30),
                        CenterIconSize = new Vector2(40, 40)
                    }
                },
                InfoPage = new InfoPageDisplayConfig()
                {
                    Position = new Vector2(0, 135),
                    TitleHeight = 45f,
                    Background = new InfoPageDisplayConfig.InfoPageBackgroundConfig()
                    {
                        Size = new Vector2(300, 600),
                        FallbackColor = "rgb(19, 24, 63)",
                        UseAlpha = false,
                        UseImage = false,
                        UseVideo = false
                    },
                    Title = new InfoPageDisplayConfig.InfoTabTextElementConfig()
                    {
                        Enabled = true,
                        Font = new FontConfig()
                        {
                            Align = "center",
                            Color = "rgb(255,255,255)",
                            IsGoogleFont = true,
                            Name = "News Cycle",
                            Size = "26px",
                            Style = "Normal"
                        },
                        Position = new InfoPageDisplayConfig.InfoTabElementVector2()
                        {
                            XP = new Vector2(150, 15),
                            CSPM = new Vector2(150, 15),
                            Gold = new Vector2(150, 15)
                        }
                    },
                    TabConfig = new InfoPageDisplayConfig.InfoTabDisplayConfig()
                    {
                        TabSize = new Vector2(300, 55),
                        UseTeamColorsText = false,
                        OrderColor = "rgb(255,255,255)",
                        ChaosColor = "rgb(255,255,255)",
                        ChampIcon = new InfoPageDisplayConfig.InfoTabImageElementConfig()
                        {
                            Enabled = true,
                            Position = new InfoPageDisplayConfig.InfoTabElementVector2()
                            {
                                XP = new Vector2(4, 8),
                                CSPM = new Vector2(4, 8),
                                Gold = new Vector2(4, 8)
                            },
                            Size = new Vector2(42, 42)
                        },
                        PlayerName = new InfoPageDisplayConfig.InfoTabTextElementConfig()
                        {
                            Enabled = true,
                            Position = new InfoPageDisplayConfig.InfoTabElementVector2()
                            {
                                XP = new Vector2(54, 6),
                                CSPM = new Vector2(54, 6),
                                Gold = new Vector2(54, 6)
                            },
                            Font = new FontConfig()
                            {
                                Align = "left",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "15px",
                                Style = "Normal"
                            }
                        },
                        MinValue = new InfoPageDisplayConfig.InfoTabTextElementConfig()
                        {
                            Enabled = true,
                            Position = new InfoPageDisplayConfig.InfoTabElementVector2()
                            {
                                XP = new Vector2(54, 22),
                                CSPM = new Vector2(54, 22),
                                Gold = new Vector2(54, 22)
                            },
                            Font = new FontConfig()
                            {
                                Align = "left",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "15px",
                                Style = "Normal"
                            }
                        },
                        CurrentValue = new InfoPageDisplayConfig.InfoTabTextElementConfig()
                        {
                            Enabled = true,
                            Position = new InfoPageDisplayConfig.InfoTabElementVector2()
                            {
                                XP = new Vector2(280, 15),
                                CSPM = new Vector2(280, 15),
                                Gold = new Vector2(270, 15)
                            },
                            Font = new FontConfig()
                            {
                                Align = "center",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "25px",
                                Style = "Normal"
                            }
                        },
                        MaxValue = new InfoPageDisplayConfig.InfoTabTextElementConfig()
                        {
                            Enabled = true,
                            Position = new InfoPageDisplayConfig.InfoTabElementVector2()
                            {
                                XP = new Vector2(260, 22),
                                CSPM = new Vector2(260, 22),
                                Gold = new Vector2(230, 22)
                            },
                            Font = new FontConfig()
                            {
                                Align = "right",
                                Color = "rgb(255,255,255)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = "15px",
                                Style = "Normal"
                            }
                        },
                        ProgressBar = new InfoPageDisplayConfig.InfoTabProgressBarConfig()
                        {
                            Enabled = true,
                            UseTeamColors = true,
                            DefaultColor = "rgb(0,0,0)",
                            ChaosColor = "rgb(0,151,196)",
                            OrderColor = "rgb(222,40,70)",
                            Animate = true,
                            Position = new InfoPageDisplayConfig.InfoTabElementVector2()
                            {
                                XP = new Vector2(52, 40),
                                CSPM = new Vector2(52, 40),
                                Gold = new Vector2(52, 40)
                            },
                            Size = new InfoPageDisplayConfig.InfoTabElementVector2()
                            {
                                XP = new Vector2(210, 10),
                                CSPM = new Vector2(210, 10),
                                Gold = new Vector2(180, 10)
                            },
                        },
                        Separator = new InfoPageDisplayConfig.InfoTabImageElementConfig()
                        {
                            Enabled = true,
                            Size = new Vector2(300,2),
                            Position = new InfoPageDisplayConfig.InfoTabElementVector2()
                            {
                                XP = new Vector2(0, 0),
                                CSPM = new Vector2(0, 0),
                                Gold = new Vector2(0, 0)
                            }
                        }
                    }
                },
                ItemComplete = new ItemCompletedDisplayConfig() {
                    UseCustomVideo = false,
                    ShowItemName = true,
                    ShowOnChampionIndicator = true,
                    ShowOnScoreboard = false,
                    ItemAnimationStates = new List<VisualElementAnimationConfig>() {
                        new VisualElementAnimationConfig() {
                            Alpha = 0,
                            Position = new Vector2(0, 0),
                            Mirror = false,
                            Duration = 0,
                            Scale = 0,
                            Ease = "",
                            Delay = 0
                        },
                        new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(0, 0),
                            Mirror = false,
                            Duration = 400,
                            Scale = 1,
                            Ease = "Quad.easeIn",
                            Delay = 0
                        },
                        new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(0, 0),
                            Mirror = false,
                            Duration = 3200,
                            Scale = 1.1f,
                            Ease = "Linear",
                            Delay = 0
                        },
                        new VisualElementAnimationConfig() {
                            Alpha = 0,
                            Position = new Vector2(0, 0),
                            Mirror = false,
                            Duration = 400,
                            Scale = 2.5f,
                            Ease = "Quad.easeOut",
                            Delay = 0
                        }
                    },
                    InfoText = new FontConfig()
                    {
                        Align = "center",
                        Color = "rgb(255,255,255)",
                        IsGoogleFont = true,
                        Name = "News Cycle",
                        Size = "20px",
                        Style = "Normal"
                    },
                    InfoBackgroundAnimationStates = new List<VisualElementAnimationConfig>()
                    {
                        new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(0, 80),
                            Mirror = false,
                            Duration = 0,
                            Scale = 1f,
                            Ease = "",
                            Delay = 0
                        },
                        new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(0, 0),
                            Mirror = false,
                            Duration = 200,
                            Scale = 1f,
                            Ease = "Quad.easeOut",
                            Delay = 800
                        },
                        new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(0, -80),
                            Mirror = false,
                            Duration = 200,
                            Scale = 1f,
                            Ease = "Quad.easeIn",
                            Delay = 2200
                        }
                    },
                    InfoTextAnimationStates = new List<VisualElementAnimationConfig>()
                    {
                        new VisualElementAnimationConfig() {
                            Alpha = 0,
                            Position = new Vector2(55, 0),
                            Mirror = true,
                            Duration = 0,
                            Scale = 1f,
                            Ease = "",
                            Delay = 0
                        },
                        new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(55, 0),
                            Mirror = true,
                            Duration = 200,
                            Scale = 1f,
                            Ease = "Quad.easeOut",
                            Delay = 1000
                        },
                        new VisualElementAnimationConfig() {
                            Alpha = 0,
                            Position = new Vector2(55, 0),
                            Mirror = true,
                            Duration = 200,
                            Scale = 1f,
                            Ease = "Quad.easeIn",
                            Delay = 1800
                        }
                    }
                },
                LevelUp = new LevelUpDisplayConfig() {
                    UseCustomVideo = false,
                    ShowOnChampionIndicator = true,
                    ShowOnScoreboard = false,
                    UseTeamColors = true,
                    ChaosColor = "rgb(255,0,0)",
                    OrderColor = "rgb(0,0,255)",
                    LevelFont = new FontConfig()
                    {
                        Align = "center",
                        Color = "rgb(255,255,255)",
                        IsGoogleFont = true,
                        Name = "News Cycle",
                        Size = "60px",
                        Style = "Normal"
                    },
                    NumberAnimationStates = new List<VisualElementAnimationConfig>() {
                        new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(0, 70),
                            Mirror = true,
                            Duration = 0,
                            Scale = 1f,
                            Ease = "",
                            Delay = 0
                        }, new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(0, -30),
                            Mirror = true,
                            Duration = 300,
                            Scale = 1f,
                            Ease = "Quad.easeOut",
                            Delay = 250
                        }, new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(0, -130),
                            Mirror = true,
                            Duration = 400,
                            Scale = 1f,
                            Ease = "Quad.easeIn",
                            Delay = 3500
                        }
                    },
                    BackgroundAnimationStates = new List<VisualElementAnimationConfig>() {
                        new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(0, 100),
                            Mirror = false,
                            Duration = 0,
                            Scale = 1f,
                            Ease = "",
                            Delay = 0
                        }, new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(0, 0),
                            Mirror = false,
                            Duration = 300,
                            Scale = 1f,
                            Ease = "Quad.easeOut",
                            Delay = 250
                        }, new VisualElementAnimationConfig() {
                            Alpha = 1,
                            Position = new Vector2(-90, 0),
                            Mirror = true,
                            Duration = 400,
                            Scale = 1f,
                            Ease = "Quad.easeIn",
                            Delay = 4000
                        }
                    }
                },
                ObjectiveKill = new ObjectiveKillConfig()
                {
                    BaronScoreboardPopUp = new ScoreboardPopUpConfig() {
                        Enabled = false,
                        ShowSpawn = false,
                        ShowTeam = false
                    },
                    DrakeScoreboardPopUp = new ScoreboardPopUpConfig()
                    {
                        Enabled = false,
                        ShowSpawn = false,
                        ShowTeam = false
                    },
                    ElderScoreboardPopUp = new ScoreboardPopUpConfig()
                    {
                        Enabled = false,
                        ShowSpawn = false,
                        ShowTeam = false
                    },
                    HeraldScoreboardPopUp = new ScoreboardPopUpConfig()
                    {
                        Enabled = false,
                        ShowSpawn = false,
                        ShowTeam = false
                    },
                    SoulPointScoreboardPopUp = new ScoreboardPopUpConfig()
                    {
                        Enabled = false,
                        ShowSpawn = false,
                        ShowTeam = false
                    }
                },
                BaronTimer = new ObjectiveTimerDisplayConfig()
                {
                    Position = new Vector2(1800, 55),
                    MaskPosition = new Vector2(1750, 20),
                    MaskSize = new Vector2(150, 70),
                    Scale = 0.8f,
                    Animate = true,
                    ObjectiveIcon = true,
                    ShowGoldDiff = true,
                    ShowTimer = true,
                    Align = "right",
                    GoldFont = new FontConfig()
                    {
                        Name = "News Cycle",
                        IsGoogleFont = true,
                        Size = "22px",
                        Style = "Bold",
                        Color = "rgb(230,190,138)",
                        Align = "right"
                    },
                    GoldPosition = new Vector2(25, -25),
                    GoldIconPosition = new Vector2(-7, -25),
                    TimeFont = new FontConfig()
                    {
                        Name = "News Cycle",
                        IsGoogleFont = true,
                        Size = "22px",
                        Style = "Bold",
                        Color = "rgb(230,190,138)",
                        Align = "right"
                    },
                    TimePosition = new Vector2(25,0),
                    TimeIconPosition = new Vector2(-10,0),
                    IconPosition = new Vector2(0,0)
                },
                ElderTimer = new ObjectiveTimerDisplayConfig()
                {
                    Position = new Vector2(120, 55),
                    MaskPosition = new Vector2(50,20),
                    MaskSize = new Vector2(150, 70),
                    Scale = 0.8f,
                    Animate = true,
                    ObjectiveIcon = true,
                    ShowGoldDiff = true,
                    ShowTimer = true,
                    Align = "left",
                    GoldFont = new FontConfig()
                    {
                        Name = "News Cycle",
                        IsGoogleFont = true,
                        Size = "22px",
                        Style = "Bold",
                        Color = "rgb(230,190,138)",
                        Align = "left"
                    },
                    GoldPosition = new Vector2(18,-25),
                    GoldIconPosition = new Vector2(22, -25),
                    TimeFont = new FontConfig()
                    {
                        Name = "News Cycle",
                        IsGoogleFont = true,
                        Size = "22px",
                        Style = "Bold",
                        Color = "rgb(230,190,138)",
                        Align = "left"
                    },
                    TimePosition = new Vector2(18, 0),
                    TimeIconPosition = new Vector2(20, -2),
                    IconPosition = new Vector2(40, 0)
                }
            };
        }

        public override bool UpdateConfigVersion(string oldVersion, string oldValues)
        {
            //1.0 to 2.0
            if (oldVersion == "1.0" && CurrentVersion == "2.0")
            {
                Task t = new Task(async () =>
                {
                    await Task.Delay(500);
                    //Simpler and more usefull to just revert to default
                    Log.Info("Update changes too large to migrate. Reverting to default");
                    RevertToDefault();
                    JSONConfigProvider.Instance.WriteConfig(this);
                });
                t.Start();

                return false;
            }
            return true;
        }

        public class ObjectiveTimerDisplayConfig
        {
            public Vector2 Position;
            public Vector2 MaskPosition;
            public Vector2 MaskSize;
            public float Scale;
            public string Align;
            public bool ShowGoldDiff;
            public bool ShowTimer;
            public bool ObjectiveIcon;
            public bool Animate;

            public Vector2 IconPosition;

            public Vector2 GoldPosition;
            public Vector2 GoldIconPosition;
            public FontConfig GoldFont;

            public Vector2 TimePosition;
            public Vector2 TimeIconPosition;
            public FontConfig TimeFont;
        }

        public class InhibitorDisplayConfig
        {
            public Vector2 Position;
            public Vector2 Size;
            public bool UseBackgroundImage;
            public bool UseBackgroundVideo;
            public string Color;
            public FontConfig Font;
            public float IconSize;
            public bool HideWhenNoneDestroyed;
            public bool UseTeamColors;
            public InhibTeamConfig BlueTeam;
            public InhibTeamConfig RedTeam;
            public List<string> LaneOrder;
            
            public class InhibTeamConfig
            {
                public Vector2 Position;
                public Vector2 LaneOffset;
                public bool UseTeamColor;
                public string Color;
                public Vector2 IconOffset;
            }
        }

        public class ObjectiveKillConfig
        {
            public ScoreboardPopUpConfig DrakeScoreboardPopUp;
            public ScoreboardPopUpConfig SoulPointScoreboardPopUp;
            public ScoreboardPopUpConfig ElderScoreboardPopUp;
            public ScoreboardPopUpConfig HeraldScoreboardPopUp;
            public ScoreboardPopUpConfig BaronScoreboardPopUp;
        }

        public class ScoreboardPopUpConfig
        {
            public bool Enabled;
            public bool ShowTeam;
            public bool ShowSpawn;
        }

        public class ItemCompletedDisplayConfig
        {
            public bool UseCustomVideo;
            public bool ShowItemName;
            public bool ShowOnScoreboard;
            public bool ShowOnChampionIndicator;

            public FontConfig InfoText;

            public List<VisualElementAnimationConfig> ItemAnimationStates;
            public List<VisualElementAnimationConfig> InfoTextAnimationStates;
            public List<VisualElementAnimationConfig> InfoBackgroundAnimationStates;

        }

        public class LevelUpDisplayConfig
        {
            public bool UseCustomVideo;
            public bool ShowOnScoreboard;
            public bool ShowOnChampionIndicator;

            public FontConfig LevelFont;
            public bool UseTeamColors;
            public string OrderColor;
            public string ChaosColor;

            public List<VisualElementAnimationConfig> NumberAnimationStates;
            public List<VisualElementAnimationConfig> BackgroundAnimationStates;
        }

        public class VisualElementAnimationConfig
        {
            public float Scale;
            public float Alpha;
            public Vector2 Position;
            public bool Mirror;
            public float Duration;
            public string Ease;
            public float Delay;
        }

        public class ScoreDisplayConfig
        {
            public Vector2 Position;
            public Vector2 Size;
            public FontConfig TimeFont;
            public Vector2 TimePosition;
            public TeamConfig BlueTeam;
            public TeamConfig RedTeam;
            public BackgroundDisplayConfig Background;
            public ExtraConfigs Misc;

            public class TeamConfig
            {
                public ElementConfig Kills;
                public ElementConfig Towers;
                public ElementConfig Gold;
                public DrakeConfig Drakes;
                public TeamName Name;
                public TeamIcon Icon;
                public TeamScoreDisplay Score;

                public class TeamName
                {
                    public bool UseTag;
                    public Vector2 Position;
                    public bool AdaptiveFontSize;
                    public Vector2 MaxSize;
                    public FontConfig Font;
                }

                public class TeamIcon
                {
                    public Vector2 Position;
                    public Vector2 Size;
                    public bool UseBackground;
                    public Vector2 BackgroundOffset;
                }

                public class TeamScoreDisplay
                {
                    public Vector2 Position;
                    public FontConfig NumberFont;
                    public bool UseCircleIcons;
                    public Vector2 CircleOffset;
                    public float CircleRadius;
                    public bool UseTeamColor;
                    public string StrokeColor;
                    public string FillColor;
                }
            }

            public class DrakeConfig
            {
                public Vector2 Position;
                public Vector2 Offset;
            }
            public class ElementConfig
            {
                public Vector2 Position;
                public FontConfig Font;
                public IconSettings Icon;
            }

            public class IconSettings
            {
                public string Name;
                public Vector2 Size;
                public Vector2 Offset;
            }

            public class ExtraConfigs
            {
                public string Animation;
                public bool ShowCenterIcon;
                public Vector2 CenterIconPosition;
                public Vector2 CenterIconSize;
                public Vector2 DrakeIconSize;
            }
        }

        public class FontConfig
        {
            public string Style;
            public string Name;
            public bool IsGoogleFont;
            public string Size;
            public string Align;
            public string Color;
        }

        public class BackgroundDisplayConfig
        {
            public bool UseImage;
            public bool UseVideo;
            public bool UseAlpha;
            public string FallbackColor;
        }

        public class InfoPageDisplayConfig
        {
            public Vector2 Position;
            public InfoTabTextElementConfig Title;
            public float TitleHeight;
            public InfoTabDisplayConfig TabConfig;
            public InfoPageBackgroundConfig Background;

            public class InfoTabDisplayConfig
            {
                public Vector2 TabSize;
                public bool UseTeamColorsText;
                public string OrderColor;
                public string ChaosColor;
                public InfoTabImageElementConfig ChampIcon;
                public InfoTabImageElementConfig Separator;
                public InfoTabTextElementConfig PlayerName;
                public InfoTabTextElementConfig MinValue;
                public InfoTabTextElementConfig CurrentValue;
                public InfoTabTextElementConfig MaxValue;
                public InfoTabProgressBarConfig ProgressBar;
            }

            public class InfoTabTextElementConfig
            {
                public bool Enabled;
                public InfoTabElementVector2 Position;
                public FontConfig Font;
            }

            public class InfoTabImageElementConfig
            {
                public bool Enabled;
                public InfoTabElementVector2 Position;
                public Vector2 Size;
            }

            public class InfoTabElementVector2
            {
                public Vector2 Gold;
                public Vector2 XP;
                public Vector2 CSPM;
            }

            public class InfoTabProgressBarConfig
            {
                public bool Enabled;
                public InfoTabElementVector2 Position;
                public InfoTabElementVector2 Size;
                public bool UseTeamColors;
                public string DefaultColor;
                public string OrderColor;
                public string ChaosColor;
                public bool Animate;
            }

            public class InfoPageBackgroundConfig
            {
                public bool UseImage;
                public bool UseVideo;
                public bool UseAlpha;
                public string FallbackColor;
                public Vector2 Size;
            }
        }
    }
}
