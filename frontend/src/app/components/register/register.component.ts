import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AuthService } from '@services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

  username: string = '';
  email: string = '';
  password: string = '';
  role: string = 'Student'; 

  constructor(
    private authService: AuthService,
    private router: Router
   ) {}

  onRegister() {
    const payload = {
      username: this.username,
      email: this.email,
      password: this.password,
      role: this.role
    };

    this.authService.register(payload).subscribe({
      next: () => {
        this.router.navigate(['/login']);
        console.log("User registered successfully!");
      },
      error: (err) => {
        console.error("Error registering user", err);
      }
    });
  }
}
