# GradeBook Application

## Overview

GradeBook is a comprehensive web application built with Angular that enables efficient management of classes, students, and grades for educational institutions. The platform provides separate interfaces for teachers and students with role-specific functionalities.

## Key Features

### For Teachers

- **Class Management**: Teachers can view all classes they're assigned to teach
- **Student Management**: Add new students to classes or remove existing ones
- **Grade Management**: Record individual or multiple grades for students with detailed information:
  - Numerical grade value (1-10 scale)
  - Assignment descriptions
  - Date tracking
- **Advanced Grading Features**:
  - Sort grades by date
  - Edit existing grades
  - Delete grades
  - Bulk operations (add, edit, or delete grades for multiple students at once)
- **Student Selection**: Select multiple students to perform batch operations
- **Grade Selection**: Select multiple grades for bulk editing or deletion

### For Students

- **Subject Overview**: Students can view all subjects they're enrolled in
- **Grade Visibility**: Access all grades recorded for each subject
- **Performance Metrics**: 
  - View average grade for each subject
  - See overall grade average across all subjects
- **Grade Organization**:
  - Toggle between viewing grades from newest to oldest or oldest to newest
  - Expand grade details to see assignment descriptions
  - View date information for each grade

## User Authentication

- **Login System**: Secure authentication with username and password
- **Role-Based Access**: Different interfaces for teachers and students
- **Registration**: New users can register as either teachers or students
- **Password Recovery**: Reset password functionality through email

## User Experience

- **Interactive Interface**: Expandable panels, toggles, and intuitive navigation
- **Loading States**: Clear indication when data is loading
- **Error Handling**: User-friendly error messages when operations fail
- **Responsive Design**: Accessible across different devices

## Technical Details

The application is built with Angular, featuring:
- Component-based architecture
- Service-oriented design for API communication
- Reactive programming with Observables
- Form handling and validation
- Dynamic data manipulation

## Getting Started

1. Register for an account (choose either Teacher or Student role)
2. Log in with your credentials
3. Access your personalized dashboard based on your role
4. For teachers: Start managing classes and recording grades
5. For students: Monitor your academic performance across all subjects
