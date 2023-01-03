//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming



export interface BudgetReadDto {
    id: number;
    startDate: Date;
    endDate: Date;
    amount: number;
}

export interface BudgetDetailsDto {
    id: number;
    startDate: Date;
    endDate: Date;
    amount: number;
    totalSpent: number;
    categorySpendingLimits: CategorySpendingLimitDto[];
}

export interface CategorySpendingLimitDto {
    id: number;
    name: string;
    limit: number;
    spent: number;
}

export interface ProblemDetails {
    type?: string | null;
    title?: string | null;
    status?: number | null;
    detail?: string | null;
    instance?: string | null;
    extensions: { [key: string]: any; };

    [key: string]: any;
}

export interface BudgetCreateDto {
    startDate: Date;
    endDate: Date;
    amount: number;
}

export interface OperationBase {
    path?: string | null;
    op?: string | null;
    from?: string | null;
}

export interface Operation extends OperationBase {
    value?: any | null;
}

export function isOperation(object: any): object is Operation {
    return object && object[''] === 'Operation';
}

export interface CategoryReadDto {
    id: number;
    name: string;
    color: string;
}

export interface CategoryCreateDto {
    name: string;
    color: string;
}

export interface ExpenseReadDto {
    id: number;
    amount: number;
    date: Date;
    details: string;
    categoryId?: number | null;
}

export enum Direction {
    Ascending = 1,
    Descending = -1,
}

export interface ExpenseCreateDto {
    amount: number;
    date: Date;
    details: string;
    categoryId?: number | null;
}

export interface SpendingLimitCreateDto {
    amount: number;
    categoryId?: number | null;
    budgetId: number;
}