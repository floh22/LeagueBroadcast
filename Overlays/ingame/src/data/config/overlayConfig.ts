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
    GlobalFont: GlobalFontSettings;
    Inhib: InhibitorDisplayConfig;
    Score: ScoreDisplayConfig;
    ObjectiveKill: ObjectiveKillConfig;
    ItemComplete: ItemCompletedDisplayConfig;
    LevelUp: LevelUpDisplayConfig;
    InfoPage: InhibitorDisplayConfig;
}

export interface InhibitorDisplayConfig {
    Location: Vector2;
    Size: Vector2;
    UseImage: boolean;
    Color: string;
    Font: FontConfig;
    IconSize: number;
    AlwaysShowWhenEnabled: boolean;
    BlueTeam: InhibTeamConfig;
    RedTeam: InhibTeamConfig;
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
    Location: Vector2;
    TimePosition: Vector2;
    BlueTeam: TeamConfig;
    RedTeam: TeamConfig;
    Misc: ExtraConfigs;
}

export interface TeamConfig {
    Kills: ElementConfig;
    Towers: ElementConfig;
    Gold: ElementConfig;
    Drakes: DrakeConfig;
    Name: TeamName;
    Icon: TeamIcon;
    Misc: MiscTeamSettings;
}

export interface TeamName {
    UseTag: boolean;
    Position: Vector2;
    Font: FontConfig;
}

export interface TeamIcon {
    AllowTransparency: boolean;
    Position: Vector2;
    Size: Vector2;
}

export interface MiscTeamSettings {
    UseSameFont: boolean;
    UseSameFontSize: boolean;
    UseSameFontStyle: boolean;
    UseSameFontColor: boolean;

    Font: string;
    IsGoogleFont: boolean;
    FontSize: number;
    FontStyle: string;
    FontColor: string;
}

export interface DrakeConfig {
    Position: Vector2;
    Offset: Vector2;
}

export interface ElementConfig {
    Position: Vector2;
    Size: number;
    Font: FontConfig;
    Icon: IconSettings;
}

export interface BackgroundConfig {
    UseImage: boolean;
    BackgroundImage: string;
    UseAlphaMask: boolean;
    BackgroundColor: string;
}

export interface IconSettings {
    Name: string;
    Size: number;
    Offset: Vector2;
}

export interface ExtraConfigs {
    UseAnimations: boolean;
    ShowCenterIcon: boolean;
}

export interface FontConfig {
    Style: string;
    Name: string;
    IsGoogleFont: boolean;
    Size: string;
    Align: string;
    Color: string;
}

export interface GlobalFontConfig {
    Name: string;
    IsGoogleFont: boolean;
    Style: string;
}

export interface InfoPageDisplayConfig {
    UseBackgroundImage: boolean;
    BackgroundColor: string;
    UseBackgroundColorTransparency: boolean;
    Font: FontConfig;
}

export interface GlobalFontSettings {
    UseGlobalFont: boolean;
    UseTeamColors: boolean;
    GlobalFont: GlobalFontConfig;
}