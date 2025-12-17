import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'project';



  constructor() {
   // localStorage.removeItem('adminToken');
    //localStorage.removeItem('userToken');
   }


   checkAdminToken(): boolean {
    const token = localStorage.getItem('adminToken');
    return token !== null;
  }
  logout(): void {
    localStorage.removeItem('adminToken');
    localStorage.removeItem('userToken');
    window.location.href = '/';
  }

}

