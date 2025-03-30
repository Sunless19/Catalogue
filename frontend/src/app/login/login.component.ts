import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/apiService';

@Component({
  selector: 'app-login2',
  standalone: true,
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  imports: [CommonModule, FormsModule],
})
export class LoginComponent {
  username: string = '';
  password: string = '';

  constructor(private userService: UserService) {}

  onLogin() {

    this.userService.login(this.username, this.password).subscribe({
      next: (token) =>{
        console.log("LOGGED IN")
      },
      error: (err) =>{
        console.log("error when logged in")
      }
    })
    console.log('Email:', this.username);
    console.log('Password:', this.password);
  }
}
