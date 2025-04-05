import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/apiService';
import { Router } from '@angular/router';

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

  constructor(private userService: UserService, private router: Router) {}

  goToRgister()
  {
    this.router.navigate(['/register']);
  }

  goToReset(){
    this.router.navigate(['/recover']);
  }

  onLogin() {

    this.userService.login(this.username, this.password).subscribe({
      next: (response: any) =>{
        const token = response.token;
        console.log('RESPONSE:', token);
        localStorage.setItem('token', token);
        const role = this.userService.getUserRole();

        if (role === 'Student') {
          this.router.navigate(['/student']);
        } else if (role === 'Teacher') {
          this.router.navigate(['/teacher']);
        } else {
          this.router.navigate(['/error']);
        }

        console.log(`LOGGED IN as ${role}`);
      },
      error: (err) =>{
        console.log("error when logged in")
      }
    })
    console.log('Email:', this.username);
    console.log('Password:', this.password);
  }
}
