import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BudgetDetailsDto, BudgetReadDto } from 'src/app/models/api-models';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BudgetService {

  constructor(private http: HttpClient) { }

  public getBudgets(): Observable<ReadonlyArray<BudgetReadDto>> {
    return this.http.get<ReadonlyArray<BudgetReadDto>>(environment.rootURL + '/Budget');
  }

  public getBudget(id: number): Observable<BudgetDetailsDto> {
    return this.http.get<BudgetDetailsDto>(environment.rootURL + '$/Budget/{id}')
  }
}
