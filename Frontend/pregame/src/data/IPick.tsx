import IPickBan from "./IPickBan";
import ISummonerSpell from "./SummonerSpell";

export default interface IPick extends IPickBan {
    id: number;
    spell1: ISummonerSpell;
    spell2: ISummonerSpell;
    isActive: boolean;
    displayName: string;
}