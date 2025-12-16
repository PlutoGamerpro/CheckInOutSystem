import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
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

}

