import { User } from './user';

export class Vote {
    id: number;
    title: string;
    notvoted: number[];
    consented: number[];
    rejected: number[];
    nvvote: number;
    cvote: number;
    rvote: number;
    ratio: number;
    halfmajority: number;
    absmajority: number;
    active: boolean;
}