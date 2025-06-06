# Patient Disease Category Assignment - Audit-Based Approach

## Overview

The system has been refactored to remove the dedicated `AssignedByPhysician` field and instead leverage the existing audit system for tracking who assigns and modifies patient disease categories. This approach is more flexible and aligns with the actual hospital workflow where nurses are the primary personnel managing patient dietary assignments.

## Key Changes

### 1. Removed AssignedByPhysician Field

- **Before**: Dedicated `AssignedByPhysician` string field
- **After**: Uses `CreatedBy` and `LastModifiedBy` from the audit system

### 2. Enhanced Role-Based Access Control

- **Nurse**: Can assign, update, and remove disease categories from patients
- **Doctor**: Can assign, update, and remove disease categories from patients
- **Manager**: Can assign, update, and remove disease categories from patients
- **Admin**: Can assign, update, and remove disease categories from patients
- **Nutritionist**: Can view patients by disease category

### 3. Improved Audit Trail

```csharp
public class PatientDiseaseCategoryDto
{
    // Audit information
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }           // Who assigned this disease category
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }      // Who last modified this assignment

    // Computed display properties
    public string AssignedByUserName => CreatedBy ?? "System";
    public string LastModifiedByUserName => LastModifiedBy ?? CreatedBy ?? "System";
}
```

## Benefits

### 1. **Leverages Existing Infrastructure**

- Uses the established `BaseAuditableEntity` system
- No need for custom tracking logic
- Consistent with other entities in the system

### 2. **More Flexible Authorization**

- Any authorized user can assign disease categories
- Role-based permissions control access
- No hardcoded assumptions about who can assign categories

### 3. **Better Hospital Workflow Alignment**

- Reflects the reality that nurses typically manage patient dietary assignments
- Supports collaborative care where multiple staff members may update assignments
- Maintains full audit trail of all changes

### 4. **Improved Data Integrity**

- Uses user IDs/emails from the actual user system
- Links to real user accounts rather than free-text names
- Enables lookup of user details, roles, and permissions

## Hospital Workflow Integration

### Typical Workflow

1. **Patient Admission**: Nurse reviews medical orders and assigns initial disease categories
2. **Daily Updates**: Nurses update dietary restrictions based on patient condition changes
3. **Medical Review**: Doctors can review and modify assignments during rounds
4. **Nutritionist Consultation**: Nutritionists can view all patients by disease category for specialized care

### Audit Trail Example

```json
{
  "id": 1,
  "patientId": "CTH-P001",
  "diseaseCategoryId": 1,
  "patientSpecificNotes": "Patient has difficulty controlling blood sugar, requires close monitoring",
  "createdAt": "2024-01-15T08:30:00Z",
  "createdBy": "nurse.tran@hospital.com", // Nurse Trần Thị Hoa assigned this
  "lastModifiedAt": "2024-01-16T14:20:00Z",
  "lastModifiedBy": "doctor.duc@hospital.com", // Doctor reviewed and updated
  "assignedByUserName": "nurse.tran@hospital.com",
  "lastModifiedByUserName": "doctor.duc@hospital.com"
}
```

## Migration Strategy

### Database Changes

```sql
-- Remove the AssignedByPhysician column
ALTER TABLE PatientDiseaseCategories DROP COLUMN AssignedByPhysician;

-- Add index for better query performance on audit fields
CREATE INDEX IX_PatientDiseaseCategories_CreatedBy ON PatientDiseaseCategories(CreatedBy);
```

### Application Changes

1. **Entity Model**: Remove `AssignedByPhysician` property
2. **DTOs**: Add audit properties and computed display properties
3. **Controllers**: Update authorization to include `Nurse` role
4. **Seed Data**: Use actual user emails/IDs instead of free-text names

## API Usage Examples

### Assigning Disease Category (Nurse)

```http
POST /api/PatientDietary/CTH-P001/disease-categories
Authorization: Bearer {nurse_jwt_token}
Content-Type: application/json

{
  "diseaseCategoryId": 1,
  "diagnosedDate": "2024-01-15",
  "patientSeverityLevel": 3,
  "patientSpecificNotes": "Patient requires strict carbohydrate monitoring",
  "isActive": true
}
```

### Response with Audit Information

```json
{
  "success": true,
  "message": "Disease category assigned to patient successfully",
  "data": {
    "id": 5,
    "patientId": "CTH-P001",
    "diseaseCategoryId": 1,
    "createdAt": "2024-01-15T08:30:00Z",
    "createdBy": "nurse.tran@hospital.com",
    "assignedByUserName": "nurse.tran@hospital.com",
    "patientName": "Nguyễn Văn An",
    "diseaseCategoryName": "Diabetes Type 2"
  }
}
```

## Security Considerations

1. **User Authentication**: All assignments are linked to authenticated users
2. **Role-Based Authorization**: Only authorized personnel can modify assignments
3. **Audit Trail**: Complete history of who made what changes and when
4. **Data Validation**: All assignments go through proper validation workflows

## Future Enhancements

1. **User Lookup Integration**: Display full user names and roles in DTOs
2. **Notification System**: Alert relevant staff when assignments change
3. **Approval Workflows**: Implement approval chains for critical disease categories
4. **Analytics**: Track assignment patterns by user role and department
