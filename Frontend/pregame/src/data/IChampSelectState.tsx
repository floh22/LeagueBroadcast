import IBan from "./IBan";
import IPick from "./IPick";
import IPreGameDisplayConfig from "./IPreGameDisplayConfig";
import ITeamState from "./ITeamState";

export default interface IChampSelectState {
    champSelectActive: boolean;
    leagueConnected: boolean;
    blueTeam: ITeamState;
    redTeam: ITeamState;
    timer: number;
    state: string;
    config: IPreGameDisplayConfig;
}