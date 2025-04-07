import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from '../services/auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://localhost:5063/User';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensure no unmatched requests
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('#login', () => {
    it('should post login data and store token in localStorage', () => {
      const mockResponse = { token: 'mock-jwt-token' };

      service.login('test@example.com', 'password123').subscribe(response => {
        expect(response.token).toEqual('mock-jwt-token');
        expect(localStorage.getItem('token')).toEqual('mock-jwt-token');
      });

      const req = httpMock.expectOne(`${apiUrl}/login`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({
        username: 'test@example.com',
        password: 'password123'
      });

      req.flush(mockResponse);
    });
  });

  describe('#register', () => {
    it('should post registration data', () => {
      const payload = { name: 'Test', email: 'test@example.com', password: 'pass123' };

      service.register(payload).subscribe(response => {
        expect(response).toEqual('User registered successfully');
      });

      const req = httpMock.expectOne(`${apiUrl}/register`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(payload);

      req.flush('User registered successfully');
    });
  });

  describe('#resetPassword', () => {
    it('should send reset password request', () => {
      const email = 'test@example.com';

      service.resetPassword(email).subscribe(response => {
        expect(response).toEqual({ success: true });
      });

      const req = httpMock.expectOne(`${apiUrl}/reset-password`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ email });

      req.flush({ success: true });
    });
  });

  describe('#sendResetEmail', () => {
    it('should send reset email', () => {
      const email = 'test@example.com';

      service.sendResetEmail(email).subscribe(response => {
        expect(response).toEqual({ sent: true });
      });

      const req = httpMock.expectOne(`${apiUrl}/send-reset-email`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ email });

      req.flush({ sent: true });
    });
  });

  describe('#confirmPasswordReset', () => {
    it('should confirm password reset', () => {
      const encodedId = 'dXNlcmlk'; // base64
      const newPassword = 'newPass123';

      service.confirmPasswordReset(encodedId, newPassword).subscribe(response => {
        expect(response).toEqual({ success: true });
      });

      const req = httpMock.expectOne(`${apiUrl}/reset-password`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ encodedId, newPassword });

      req.flush({ success: true });
    });
  });
});
