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
    ItemComplete: ItemCompletedDisplayConfig;
    LevelUp: LevelUpDisplayConfig;
    InfoPage: InhibitorDisplayConfig;
    BaronTimer: ObjectiveTimerDisplayConfig;
    ElderTimer: ObjectiveTimerDisplayConfig;
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
    DrakeScoreboardPopUp: ScoreboardPopUpConfig;
    SoulPointScoreboardPopUp: ScoreboardPopUpConfig;
    ElderScoreboardPopUp: ScoreboardPopUpConfig;
    HeraldScoreboardPopUp: ScoreboardPopUpConfig;
    BaronScoreboardPopUp: ScoreboardPopUpConfig;
}

export interface ScoreboardPopUpConfig {
    Enabled: boolean;
    ShowTeam: boolean;
    ShowSpawn: boolean;
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
    IsGoogleFont: boolean;
    Size: string;
    Align: string;
    Color: string;
}

export interface InfoPageDisplayConfig {

}

export interface BackgroundDisplayConfig {
    UseImage: boolean;
    UseVideo: boolean;
    UseAlpha: boolean;
    FallbackColor: string;
}