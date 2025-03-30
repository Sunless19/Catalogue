import { Component } from '@angular/core';
import { UserService } from '../../services/apiService';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

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

  constructor(private userService: UserService) {}

  onRegister() {
    const payload = {
      username: this.username,
      email: this.email,
      password: this.password,
      role: this.role
    };

    this.userService.register(payload).subscribe({
      next: () => {
        console.log("User registered successfully!");
      },
      error: (err) => {
        console.error("Error registering user", err);
      }
    });
  }
}
