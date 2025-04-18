import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AuthService } from '@services/auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  encodedId: string = '';
  newPassword: string = '';
  confirmPassword: string = '';

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.encodedId = this.route.snapshot.queryParamMap.get('id') || '';
    console.log('Encoded ID:', this.encodedId);
  }

  onSubmit(): void {
    if (this.newPassword !== this.confirmPassword) {
      return;
    }

    this.authService.confirmPasswordReset(this.encodedId, this.newPassword).subscribe({
      next: (res) =>{ 
          alert('Password has been successfully reset!');
          this.router.navigate(['/login']);
      }
    });
  }
}
