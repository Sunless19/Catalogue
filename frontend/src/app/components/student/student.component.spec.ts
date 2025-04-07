import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { StudentComponent } from './student.component';
import { UserService } from '@services/user.service';
import { ClassService } from '@services/class.service';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

describe('StudentComponent', () => {
  let component: StudentComponent;
  let fixture: ComponentFixture<StudentComponent>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockClassService: jasmine.SpyObj<ClassService>;

  const mockData = [
    {
      className: 'Math',
      grades: [
        { value: 10, date: '2024-05-01', assignments: 'Homework 1', expanded: false },
        { value: 8, date: '2024-04-01', assignments: 'Homework 2', expanded: false },
      ],
    },
    {
      className: 'Science',
      grades: [{ value: 7, date: '2024-03-10', assignments: '', expanded: false }],
    }
  ];

  beforeEach(async () => {
    mockUserService = jasmine.createSpyObj('UserService', ['getTeacherId']);
    mockClassService = jasmine.createSpyObj('ClassService', ['getStudentClassesAndGrades']);

    await TestBed.configureTestingModule({
      imports: [StudentComponent],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: ClassService, useValue: mockClassService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(StudentComponent);
    component = fixture.componentInstance;
  });

  it('should fetch and display student classes and grades on init', fakeAsync(() => {
    mockUserService.getTeacherId.and.returnValue(1);
    mockClassService.getStudentClassesAndGrades.and.returnValue(of(mockData));

    fixture.detectChanges(); // triggers ngOnInit
    tick(); // simulate async

    expect(component.studentClasses.length).toBe(2);
    expect(component.isLoading).toBeFalse();
    expect(component.errorMessage).toBe('');
    expect(component.studentClasses[0].expanded).toBeFalse();
  }));

  it('should show error message on API failure', fakeAsync(() => {
    mockUserService.getTeacherId.and.returnValue(1);
    mockClassService.getStudentClassesAndGrades.and.returnValue(throwError(() => new Error('Network error')));

    fixture.detectChanges();
    tick();

    expect(component.studentClasses.length).toBe(0);
    expect(component.errorMessage).toBe('Failed to load subjects or grades.');
    expect(component.isLoading).toBeFalse();
  }));

  it('should toggle subject expansion', () => {
    component.studentClasses = [{ className: 'Math', grades: [], expanded: false }];
    component.toggleSubject(0);
    expect(component.studentClasses[0].expanded).toBeTrue();
  });

  it('should toggle assignment expansion', () => {
    const grade = { value: 10, date: '2024-05-01', expanded: false };
    component.toggleAssignment(grade);
    expect(grade.expanded).toBeTrue();
  });


  it('should calculate overall average correctly', () => {
    component.studentClasses = [
      { grades: [{ value: 10 }, { value: 8 }] },
      { grades: [{ value: 7 }] }
    ];
    expect(component.overallAverage).toBeCloseTo(8.33, 1);
  });

  it('should sort grades by date descending then ascending', () => {
    const grades = [
      { value: 10, date: '2024-04-01' },
      { value: 8, date: '2024-05-01' }
    ];
    component.studentClasses = [{
      grades: [...grades],
      sortDescending: true
    }];

    component.toggleSort(0);
    expect(component.studentClasses[0].sortDescending).toBeFalse();
    expect(component.studentClasses[0].grades[0].value).toBe(10);

    component.toggleSort(0);
    expect(component.studentClasses[0].sortDescending).toBeTrue();
    expect(component.studentClasses[0].grades[0].value).toBe(8);
  });
});
