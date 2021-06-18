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

        public GlobalFontSettings GlobalFont;
        public InhibitorDisplayConfig Inhib;
        public ScoreDisplayConfig Score;
        public ObjectiveKillConfig ObjectiveKill;
        public ItemCompletedDisplayConfig ItemComplete;
        public LevelUpDisplayConfig LevelUp;
        public InfoPageDisplayConfig InfoPage;

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
            GlobalFont = Cfg.GlobalFont;
            InfoPage = Cfg.InfoPage;
            ItemComplete = Cfg.ItemComplete;
            LevelUp = Cfg.LevelUp;
            ObjectiveKill = Cfg.ObjectiveKill;
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
            this.GlobalFont = def.GlobalFont;
            this.InfoPage = def.InfoPage;
            this.ItemComplete = def.ItemComplete;
            this.LevelUp = def.LevelUp;
            this.ObjectiveKill = def.ObjectiveKill;
            this.FileVersion = CurrentVersion;

        }

        private IngameConfig CreateDefault()
        {
            return new IngameConfig()
            {
                FileVersion = CurrentVersion,
                Inhib = new InhibitorDisplayConfig() { Location = new Vector2(0, 845), Size = new Vector2(306, 118) },
                Score = new ScoreDisplayConfig()
                {
                    Location = new Vector2(906, 0),
                    TimePosition = new Vector2(0, 0),
                    BlueTeam = new ScoreDisplayConfig.TeamConfig()
                    {
                        Drakes = new ScoreDisplayConfig.DrakeConfig()
                        {
                            Offset = new Vector2(0, 0),
                            Position = new Vector2(0, 0)
                        },
                        Gold = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "Right",
                                Color = "rgb(100,100,100)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = 15f,
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "GoldIcon",
                                Offset = new Vector2(0, 0),
                                Size = 100f
                            },
                            Position = new Vector2(0, 0),
                            Size = 1f
                        },
                        Icon = new ScoreDisplayConfig.TeamConfig.TeamIcon()
                        {
                            AllowTransparency = true,
                            Position = new Vector2(0, 0),
                            Size = new Vector2(100, 100)
                        },
                        Kills = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "Right",
                                Color = "rgb(100,100,100)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = 15f,
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "KillsIcon",
                                Offset = new Vector2(0, 0),
                                Size = 100f
                            },
                            Position = new Vector2(0, 0),
                            Size = 1f
                        },
                        Name = new ScoreDisplayConfig.TeamConfig.TeamName()
                        {
                            Font = new FontConfig()
                            {
                                Align = "Right",
                                Color = "rgb(100,100,100)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = 15f,
                                Style = "Normal"
                            },
                            Position = new Vector2(0, 0),
                            UseTag = true
                        },
                        Towers = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "Right",
                                Color = "rgb(100,100,100)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = 15f,
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "TowerIcon",
                                Offset = new Vector2(0, 0),
                                Size = 100f
                            },
                            Position = new Vector2(0, 0),
                            Size = 1f
                        },
                        Misc = new ScoreDisplayConfig.TeamConfig.MiscSettings() {
                            UseSameFont = false,
                            UseSameFontStyle = false,
                            UseSameFontColor = false,
                            UseSameFontSize = false,
                            Font = "News Cylce",
                            IsGoogleFont = true,
                            FontColor = "rgb(0,0,0)",
                            FontSize = 10,
                            FontStyle = "Normal"
                        }
                    },
                    RedTeam = new ScoreDisplayConfig.TeamConfig()
                    {
                        Drakes = new ScoreDisplayConfig.DrakeConfig()
                        {
                            Offset = new Vector2(0, 0),
                            Position = new Vector2(0, 0)
                        },
                        Gold = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "Right",
                                Color = "rgb(100,100,100)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = 15f,
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "GoldIcon",
                                Offset = new Vector2(0, 0),
                                Size = 100f
                            },
                            Position = new Vector2(0, 0),
                            Size = 1f
                        },
                        Icon = new ScoreDisplayConfig.TeamConfig.TeamIcon()
                        {
                            AllowTransparency = true,
                            Position = new Vector2(0, 0),
                            Size = new Vector2(100, 100)
                        },
                        Kills = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "Right",
                                Color = "rgb(100,100,100)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = 15f,
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "KillsIcon",
                                Offset = new Vector2(0, 0),
                                Size = 100f
                            },
                            Position = new Vector2(0, 0),
                            Size = 1f
                        },
                        Name = new ScoreDisplayConfig.TeamConfig.TeamName()
                        {
                            Font = new FontConfig()
                            {
                                Align = "Right",
                                Color = "rgb(100,100,100)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = 15f,
                                Style = "Normal"
                            },
                            Position = new Vector2(0, 0),
                            UseTag = true
                        },
                        Towers = new ScoreDisplayConfig.ElementConfig()
                        {
                            Font = new FontConfig()
                            {
                                Align = "Right",
                                Color = "rgb(100,100,100)",
                                IsGoogleFont = true,
                                Name = "News Cycle",
                                Size = 15f,
                                Style = "Normal"
                            },
                            Icon = new ScoreDisplayConfig.IconSettings()
                            {
                                Name = "TowerIcon",
                                Offset = new Vector2(0, 0),
                                Size = 100f
                            },
                            Position = new Vector2(0, 0),
                            Size = 1f
                        },
                        Misc = new ScoreDisplayConfig.TeamConfig.MiscSettings()
                        {
                            UseSameFont = false,
                            UseSameFontStyle = false,
                            UseSameFontColor = false,
                            UseSameFontSize = false,
                            Font = "News Cylce",
                            IsGoogleFont = true,
                            FontColor = "rgb(0,0,0)",
                            FontSize = 10,
                            FontStyle = "Normal"
                        }
                    },
                    Misc = new ScoreDisplayConfig.ExtraConfigs()
                    {
                        ShowCenterIcon = true,
                        UseAnimations = true
                    }
                },
                GlobalFont = new GlobalFontSettings()
                {
                    GlobalFont = new GlobalFontConfig()
                    {
                        IsGoogleFont = true,
                        Name = "News Cycle",
                        Style = "Normal"
                    },
                    UseGlobalFont = false,
                    UseTeamColors = true
                },
                InfoPage = new InfoPageDisplayConfig()
                {
                    BackgroundColor = "rbg(100,100,100)",
                    Font = new FontConfig()
                    {
                        Align = "Right",
                        Color = "rgb(100,100,100)",
                        IsGoogleFont = true,
                        Name = "News Cycle",
                        Size = 15f,
                        Style = "Normal"
                    },
                    UseBackgroundColorTransparency = true,
                    UseBackgroundImage = false
                },
                ItemComplete = new ItemCompletedDisplayConfig() {
                    ShowItemName = false,
                    ShowOnChampionIndicator = true,
                    ShowOnScoreboard = false
                },
                LevelUp = new LevelUpDisplayConfig() {
                    ShowOnChampionIndicator = true,
                    ShowOnScoreboard = false
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

        public class InhibitorDisplayConfig
        {
            public Vector2 Location;
            public Vector2 Size;
            public bool UseImage;
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
            public bool ShowItemName;
            public bool ShowOnScoreboard;
            public bool ShowOnChampionIndicator;
        }

        public class LevelUpDisplayConfig
        {
            public bool ShowOnScoreboard;
            public bool ShowOnChampionIndicator;
        }

        public class ScoreDisplayConfig
        {
            public Vector2 Location;
            public Vector2 TimePosition;
            public TeamConfig BlueTeam;
            public TeamConfig RedTeam;
            public ExtraConfigs Misc;


            public class TeamConfig
            {
                public ElementConfig Kills;
                public ElementConfig Towers;
                public ElementConfig Gold;
                public DrakeConfig Drakes;
                public TeamName Name;
                public TeamIcon Icon;
                public MiscSettings Misc;

                public class TeamName
                {
                    public bool UseTag;
                    public Vector2 Position;
                    public FontConfig Font;
                }

                public class TeamIcon
                {
                    public bool AllowTransparency;
                    public Vector2 Position;
                    public Vector2 Size;
                }

                public class MiscSettings
                {
                    public bool UseSameFont;
                    public bool UseSameFontSize;
                    public bool UseSameFontStyle;
                    public bool UseSameFontColor;

                    public string Font;
                    public bool IsGoogleFont;
                    public float FontSize;
                    public string FontStyle;
                    public string FontColor;
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
                public float Size;
                public FontConfig Font;
                public IconSettings Icon;
            }

            public class BackgroundConfig
            {
                public bool UseImage;
                public string BackgroundImage;
                public bool UseAlphaMask;
                public string BackgroundColor;
            }

            public class IconSettings
            {
                public string Name;
                public float Size;
                public Vector2 Offset;
            }

            public class ExtraConfigs
            {
                public bool UseAnimations;
                public bool ShowCenterIcon;
            }
        }

        public class FontConfig
        {
            public string Style;
            public string Name;
            public bool IsGoogleFont;
            public float Size;
            public string Align;
            public string Color;
        }

        public class GlobalFontConfig
        {
            public string Name;
            public string Style;
            public bool IsGoogleFont;
        }

        public class InfoPageDisplayConfig
        {
            public bool UseBackgroundImage;
            public string BackgroundColor;
            public bool UseBackgroundColorTransparency;
            public FontConfig Font;
        }


        public class GlobalFontSettings
        {
            public bool UseGlobalFont;
            public bool UseTeamColors;
            public GlobalFontConfig GlobalFont;
        }
    }
}
