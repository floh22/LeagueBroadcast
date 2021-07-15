import Vector2 from "~/util/Vector2";

export default class OverlayConfigEvent {
    type: number;
    eventType: string;
    config: OverlayConfig;

    constructor(message: any) {
        this.type = message.type;
        this.eventType = message.eventType;
        this.config = message.config;
    }
}

export interface OverlayConfig {
    FileVersion: string;
    Inhib: InhibitorDisplayConfig;
    Score: ScoreDisplayConfig;
    ObjectiveKill: ObjectiveKillConfig;
    ObjectiveSpawn: ObjectiveSpawnConfig;
    ItemComplete: ItemCompletedDisplayConfig;
    LevelUp: LevelUpDisplayConfig;
    InfoPage: InfoPageDisplayConfig;
    BaronTimer: ObjectiveTimerDisplayConfig;
    ElderTimer: ObjectiveTimerDisplayConfig;
    GoldGraph: GoldGraphDisplayConfig;
    GoogleFonts: string[];
}

export interface ObjectiveTimerDisplayConfig {
    Position: Vector2;
    MaskPosition: Vector2;
    MaskSize: Vector2;
    Scale: number;
    Align: string;
    ShowGoldDiff: boolean;
    ShowTimer: boolean;
    ObjectiveIcon: boolean;
    Animate: boolean;

    IconPosition: Vector2;

    GoldPosition: Vector2;
    GoldIconPosition: Vector2;
    GoldFont: FontConfig;

    TimePosition: Vector2;
    TimeIconPosition: Vector2;
    TimeFont: FontConfig;
}

export interface InhibitorDisplayConfig {
    Position: Vector2;
    Size: Vector2;
    UseBackgroundImage: boolean;
    UseBackgroundVideo: boolean;
    Color: string;
    Font: FontConfig;
    IconSize: number;
    HideWhenNoneDestroyed: boolean;
    UseTeamColors: boolean;
    BlueTeam: InhibTeamConfig;
    RedTeam: InhibTeamConfig;
    LaneOrder: string[];
}

export interface InhibTeamConfig {
    Position: Vector2;
    LaneOffset: Vector2;
    UseTeamColor: boolean;
    Color: string;
    IconOffset: Vector2;
}

export interface ObjectiveKillConfig {
    ShowTeamIcon: boolean;
    SoulPointScoreboardPopUp: ScoreboardPopUpConfig;
    ElderKillScoreboardPopUp: ScoreboardPopUpConfig;
    DragonKillScoreboardPopUp: ScoreboardPopUpConfig;
    BaronKillScoreboardPopUp: ScoreboardPopUpConfig;
    HeraldKillScoreboardPopUp: ScoreboardPopUpConfig;
}

export interface ObjectiveSpawnConfig {
    BaronSpawnScoreboardPopUp: ScoreboardPopUpConfig;
    DrakeSpawnScoreboardPopUp: ScoreboardPopUpConfig;
    HeraldSpawnScoreboardPopUp: ScoreboardPopUpConfig;
}

export interface ScoreboardPopUpConfig {
    Enabled: boolean;
    UseImage: boolean;
    UseVideo: boolean;
    UseAlpha: boolean;
    ForceDisplayDurationForVideo: boolean;
    DisplayDuration: number;
    AnimationDuration: number;
}

export interface ItemCompletedDisplayConfig {
    UseCustomVideo: boolean;
    ShowItemName: boolean;
    ShowOnScoreboard: boolean;
    ShowOnChampionIndicator: boolean;
    ItemAnimationStates: VisualElementAnimationConfig[];
    InfoText: FontConfig;
    InfoBackgroundAnimationStates: VisualElementAnimationConfig[];
    InfoTextAnimationStates: VisualElementAnimationConfig[];
}

export interface VisualElementAnimationConfig {
    Scale: number;
    Alpha: number;
    Position: Vector2;
    Mirror: boolean;
    Duration: number;
    Ease: string;
    Delay: number;
}

export interface LevelUpDisplayConfig {
    UseCustomVideo: boolean;
    ShowOnScoreboard: boolean;
    ShowOnChampionIndicator: boolean;
    LevelFont: FontConfig;
    UseTeamColors: boolean;
    OrderColor: string;
    ChaosColor: string;

    NumberAnimationStates: VisualElementAnimationConfig[];
    BackgroundAnimationStates: VisualElementAnimationConfig[];
}

export interface ScoreDisplayConfig {
    Position: Vector2;
    Size: Vector2;
    TimeFont: FontConfig;
    TimePosition: Vector2;
    BlueTeam: TeamConfig;
    RedTeam: TeamConfig;
    Background: BackgroundDisplayConfig;
    Misc: ExtraConfigs;
}

export interface TeamConfig {
    Kills: ElementConfig;
    Towers: ElementConfig;
    Gold: ElementConfig;
    Drakes: DrakeConfig;
    Score: TeamScoreDisplay;
    Name: TeamName;
    Icon: TeamIcon;
}

export interface TeamName {
    UseTag: boolean;
    Position: Vector2;
    Font: FontConfig;
    MaxSize: Vector2;
    AdaptiveFontSize: boolean;
}

export interface TeamIcon {
    Position: Vector2;
    Size: Vector2;
    UseBackground: boolean;
    BackgroundOffset: Vector2;
}

export interface TeamScoreDisplay {
    Position: Vector2;
    NumberFont: FontConfig;
    UseCircleIcons: boolean;
    CircleOffset: Vector2;
    CircleRadius: number;
    UseTeamColor: boolean;
    StrokeColor: string;
    FillColor: string;
}

export interface DrakeConfig {
    Position: Vector2;
    Offset: Vector2;
}

export interface ElementConfig {
    Position: Vector2;
    Font: FontConfig;
    Icon: IconSettings;
}

export interface IconSettings {
    Name: string;
    Size: Vector2;
    Offset: Vector2;
}

export interface ExtraConfigs {
    Animation: string;
    ShowCenterIcon: boolean;
    CenterIconPosition: Vector2;
    CenterIconSize: Vector2;
    DrakeIconSize: Vector2;
}

export interface FontConfig {
    Style: string;
    Name: string;
    Size: string;
    Align: string;
    Color: string;
}

export interface InfoPageDisplayConfig {
    Position: Vector2;
    Title: InfoTabTextElementConfig;
    TitleHeight: number;
    TabConfig: InfoTabDisplayConfig;
    Background: InfoPageBackgroundConfig;
}

export interface InfoTabDisplayConfig {
    TabSize: Vector2;
    UseTeamColorsText: boolean;
    OrderColor: string;
    ChaosColor: string;
    ChampIcon: InfoTabImageElementConfig;
    Separator: InfoTabImageElementConfig;
    PlayerName: InfoTabTextElementConfig;
    MinValue: InfoTabTextElementConfig;
    CurrentValue: InfoTabTextElementConfig;
    MaxValue: InfoTabTextElementConfig;
    ProgressBar: InfoTabProgressBarConfig;
}

export interface InfoTabProgressBarConfig {
    Enabled: boolean;
    Position: InfoTabElementVector2;
    Size: InfoTabElementVector2;
    UseTeamColors: boolean;
    DefaultColor: string;
    OrderColor: string;
    ChaosColor: string;
    Animate: boolean;
}

export interface InfoTabImageElementConfig {
    Enabled: boolean;
    Position: InfoTabElementVector2;
    Size: Vector2;
}

export interface InfoTabTextElementConfig {
    Enabled: boolean;
    Position: InfoTabElementVector2;
    Font: FontConfig;
}

export interface InfoTabElementVector2 {
    Gold: Vector2;
    XP: Vector2;
    CSPM: Vector2;
}

export interface InfoPageBackgroundConfig{
    UseImage: boolean;
    UseVideo: boolean;
    UseAlpha: boolean;
    FallbackColor: string;
    Size: Vector2;
}

export interface BackgroundDisplayConfig {
    UseImage: boolean;
    UseVideo: boolean;
    UseAlpha: boolean;
    FallbackColor: string;
}

export interface GoldGraphDisplayConfig {
    Position: Vector2;
    Size: Vector2;
    Title: GoldGraphTitleConfig;
    Background: BackgroundDisplayConfig;
    Graph: GraphDisplayConfig;
}

export interface GraphDisplayConfig {
    Position: Vector2;
    Size: Vector2;
    InfoFont: FontConfig;
    BorderUseTeamColors: boolean;
    BorderChaosColor: string;
    BorderOrderColor: string;
    FillUseTeamColors: boolean;
    FillChaosColor: string;
    FillOrderColor: string;
    GridColor: string;
    GridEdgeColor: string;
    ShowHorizontalGrid: boolean;
    ShowVerticalGrid: boolean;
    LineTension: number;
    TimeStepSize: number;
    ShowTimeStepIndicators: boolean;
}

export interface GoldGraphTitleConfig {
    Enabled: boolean;
    Position: Vector2;
    Font: FontConfig;
    Text: string;
}