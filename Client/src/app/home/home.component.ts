import { Component, OnInit } from '@angular/core';
import { BudgetCreateDto, BudgetDetailsDto, BudgetReadDto } from '../models/api-models';
import { BudgetService } from '../services/api/budget.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  budgets: ReadonlyArray<BudgetReadDto> = [];
  currentBudgetId!: number;
  currentBudgetDetails!: BudgetDetailsDto;

  constructor(private budgetService: BudgetService, private authService: AuthService) { }

  ngOnInit(): void {

    this.getBudgets();
    this.authService.getIdToken().subscribe(x => console.log(x))
  }

  getBudgets(): void {
    this.budgetService.getBudgets().subscribe({
      next: response => {
        this.budgets = response,
        this.currentBudgetId = this.getCurrentBudgetId(this.budgets)
        if (this.currentBudgetId != 0) {
          this.budgetService.getBudget(this.currentBudgetId).subscribe({
            next: response => {
              this.currentBudgetDetails = response,
              console.log(this.currentBudgetDetails)
            },
            error: error => console.log(error)
          }
          )
        }
      },
      error: error => console.log(error)
    });
  }

  private getCurrentBudgetId(budgets: ReadonlyArray<BudgetReadDto>): number {
    const now = new Date();
    const currentYear = now.getFullYear();
    const currentMonth = now.getMonth();
    const currentDay = now.getDate();
  
    for (const budget of budgets) {
      const budgetStartDate = new Date(budget.startDate);
      const budgetEndDate = new Date(budget.endDate);

      if (budgetStartDate.getFullYear() === currentYear && 
          budgetStartDate.getMonth() === currentMonth && 
          budgetStartDate.getDate() <= currentDay && 
          budgetEndDate.getDate() >= currentDay) {
        return budget.id;
      }
    }

    return 0;
  }
}
