//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.18.0.0 (NJsonSchema v10.8.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming



export interface CategoryReadDto {
    id: number;
    name: string;
    color: string;
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

export interface CategoryCreateDto {
    name: string;
    color: string;
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

export interface ExpenseReadDto {
    id: number;
    amount: number;
    date: Date;
    details: string;
    categoryId?: number | null;
}

export interface ExpenseCreateDto {
    amount: number;
    date: Date;
    details: string;
    categoryId?: number | null;
}