import IBan from "./IBan";
import IPick from "./IPick";

export default interface ITeamState {
    bans: IBan[];
    picks: IPick[];
    isActive: boolean;
}
