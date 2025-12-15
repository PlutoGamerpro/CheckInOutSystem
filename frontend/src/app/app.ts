import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected title = 'project';

  constructor() {
    //localStorage.removeItem('adminToken');
  //  localStorage.removeItem('userToken');

   }


   checkAdminToken(): boolean {
    const token = localStorage.getItem('adminToken');
    return token !== null;
  }

}

