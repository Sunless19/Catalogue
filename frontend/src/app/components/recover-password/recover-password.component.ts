import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-recover-password',
  imports: [CommonModule, FormsModule],
  templateUrl: './recover-password.component.html',
  styleUrl: './recover-password.component.css'
})
export class RecoverPasswordComponent {
  email: string = '';
  message: string = '';

  onSubmit() {
    console.log('Recovery email sent to:', this.email);
    this.message = 'If this email is registered, a password reset link has been sent.';
  }
}
