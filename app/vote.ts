import { User } from './user';

export class Vote {
    id: number;
    title: string;
    notvoted: number[];
    consented: number[];
    rejected: number[];
    abstained: number[];
    nvvote: number;
    cvote: number;
    rvote: number;
    avote: number;
    ratio: number;
    halfmajority: number;
    absmajority: number;
    active: boolean;
    type: number;
    passvote: number;
}