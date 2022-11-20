import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { map, Observable, of } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) { }

  canActivate(): Observable<boolean> {
    return this.authService.getAuthState().pipe(
      map((authState) => {
        if (authState) {
          return true;
        }
        this.router.navigateByUrl('/signup');
        return false;
      })
    )
  }
}
