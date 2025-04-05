import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/apiService';

@Component({
  selector: 'app-recover-password',
  imports: [CommonModule, FormsModule],
  templateUrl: './recover-password.component.html',
  styleUrl: './recover-password.component.css'
})
export class RecoverPasswordComponent {
  email: string = '';
  message: string = '';

  constructor(private userService: UserService){}

  onSubmit() {
    this.userService.sendResetEmail(this.email).subscribe({
      next: () => {
        console.log('Recovery email sent to:', this.email);
        this.message = 'If this email is registered, a password reset link has been sent.';
      },
      error: (err) => {
        console.error('Failed to send reset email:', err);
        this.message = 'Something went wrong. Please try again later.';
      }
    }); 
    console.log('Recovery email sent to:', this.email);
    this.message = 'If this email is registered, a password reset link has been sent.';
  }
}
