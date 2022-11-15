import { Injectable } from '@angular/core';
import { AngularFireAuth } from '@angular/fire/compat/auth';
import { Router } from '@angular/router';
import firebase from 'firebase/compat/app';
import { BehaviorSubject } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private fa: AngularFireAuth, private router: Router) { }

  public googleSignIn() {
    this.fa.signInWithPopup(new firebase.auth.GoogleAuthProvider()).then(response => {
      console.log(response)
      // TODO: navigate somewhere
    }).catch(error => {
      console.log(error)
    });
  }

  public signOut() {
    this.fa.signOut().then(response => {
      console.log(response)
    }).catch(error => {
      // TODO: send message to UI
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
