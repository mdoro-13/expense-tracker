import { Injectable } from '@angular/core';
import { AngularFireAuth } from '@angular/fire/compat/auth';
import { Router } from '@angular/router';
import firebase from 'firebase/compat/app';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private fa: AngularFireAuth, private router: Router) { }

  public googleSignIn() {
    this.fa.signInWithPopup(new firebase.auth.GoogleAuthProvider()).then(response => {
      this.router.navigateByUrl('/home')
    }).catch(error => {
      // TODO: maybe display error in UI
      console.log(error)
    });
  }

  public signOut() {
    this.fa.signOut().then(response => {
      console.log(response)
    }).catch(error => {
      // TODO: maybe display error in UI
      console.log(error)
    })
  }

  public getAuthState() {
    return this.fa.authState;
  }

  public getIdToken() {
    return this.fa.idToken;
  }
}
