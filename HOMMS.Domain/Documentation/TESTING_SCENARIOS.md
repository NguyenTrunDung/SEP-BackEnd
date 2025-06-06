# Disease Category Management Testing Scenarios

This document outlines comprehensive testing scenarios to verify that the Disease Category Management system works correctly with your existing HOMMS codebase.

## Prerequisites

1. **Database Setup**: Run EF migrations to create new tables
2. **Seed Data**: Execute the seed data to populate test data
3. **Authentication**: Ensure JWT authentication is working
4. **Multi-Tenancy**: Verify branch context is correctly set

```bash
# Run migrations
dotnet ef migrations add "AddDiseaseCategoryTables" --project HOMMS.Infrastructure
dotnet ef database update --project HOMMS.Infrastructure

# Seed test data
# (Seed data will be automatically applied on application startup)
```

## Test Scenario 1: Basic Disease Category CRUD Operations

### Test 1.1: Create Disease Category

```http
POST /api/diseasecategories
Authorization: Bearer {admin-token}
Content-Type: application/json

{
    "name": "Diabetes Type 2",
    "code": "DM2",
    "description": "Type 2 diabetes requiring carbohydrate management",
    "dietaryRestrictions": "Avoid high sugar foods, limit carbohydrates",
    "recommendedFoods": "Lean proteins, vegetables, whole grains",
    "severityLevel": 3,
    "requiresApproval": true,
    "colorCode": "#FF6B6B"
}
```

**Expected Result:**

- Status: 201 Created
- Response contains new disease category with ID
- Branch ID is automatically set to current branch
- CreatedAt and CreatedBy are populated

### Test 1.2: Get All Disease Categories

```http
GET /api/diseasecategories
Authorization: Bearer {user-token}
```

**Expected Result:**

- Status: 200 OK
- Returns only disease categories for current branch
- Inactive categories excluded by default
- Patient count and restriction count populated

### Test 1.3: Update Disease Category

```http
PUT /api/diseasecategories/1
Authorization: Bearer {nutritionist-token}
Content-Type: application/json

{
    "name": "Diabetes Type 2 - Updated",
    "code": "DM2",
    "description": "Updated description",
    "severityLevel": 4,
    "requiresApproval": true
}
```

**Expected Result:**

- Status: 200 OK
- Disease category updated with new values
- LastModifiedAt and LastModifiedBy populated

## Test Scenario 2: Food Restriction Management

### Test 2.1: Add Food Restriction

```http
POST /api/diseasecategories/1/food-restrictions
Authorization: Bearer {nutritionist-token}
Content-Type: application/json

{
    "foodId": 15,
    "restrictionLevel": 4,
    "reason": "High sugar content dangerous for diabetics",
    "alternativeRecommendations": "Sugar-free alternatives, fresh fruit",
    "requiresPhysicianOverride": true
}
```

**Expected Result:**

- Status: 200 OK
- Food restriction created and linked to disease category
- Validates that food exists and belongs to same branch

### Test 2.2: Get Food Restrictions for Disease Category

```http
GET /api/diseasecategories/1/food-restrictions
Authorization: Bearer {user-token}
```

**Expected Result:**

- Status: 200 OK
- Returns all active food restrictions for the disease category
- Includes food names and restriction level text

## Test Scenario 3: Patient Disease Category Assignment

### Test 3.1: Assign Disease Category to Patient

```http
POST /api/patientdietary/patient-123/disease-categories
Authorization: Bearer {doctor-token}
Content-Type: application/json

{
    "diseaseCategoryId": 1,
    "diagnosedDate": "2024-01-15T00:00:00Z",
    "patientSeverityLevel": 3,
    "patientSpecificNotes": "Requires strict blood sugar monitoring",
    "assignedByPhysician": "Dr. Nguyen Van A"
}
```

**Expected Result:**

- Status: 200 OK
- Disease category assigned to patient
- Validates patient exists and belongs to same branch
- Prevents duplicate assignments

### Test 3.2: Get Patient Dietary Information

```http
GET /api/patientdietary/patient-123/dietary-info
Authorization: Bearer {staff-token}
```

**Expected Result:**

- Status: 200 OK
- Complete dietary profile with all disease categories
- List of restricted food IDs
- Aggregated dietary restrictions and recommendations
- Highest severity level and approval requirements

## Test Scenario 4: Food Validation System

### Test 4.1: Validate Safe Food for Patient

```http
POST /api/patientdietary/patient-123/validate-food
Authorization: Bearer {user-token}
Content-Type: application/json

{
    "foodId": 1,
    "quantity": 1
}
```

**Expected Result (Safe Food):**

```json
{
  "status": "success",
  "data": {
    "isAllowed": true,
    "validationStatus": "Safe",
    "warnings": [],
    "restrictions": [],
    "alternatives": [],
    "requiresPhysicianApproval": false
  }
}
```

### Test 4.2: Validate Restricted Food for Patient

```http
POST /api/patientdietary/patient-123/validate-food
Authorization: Bearer {user-token}
Content-Type: application/json

{
    "foodId": 15,
    "quantity": 1
}
```

**Expected Result (Dangerous Food):**

```json
{
  "status": "success",
  "data": {
    "isAllowed": false,
    "validationStatus": "Dangerous",
    "warnings": [
      "This food contains high sugar content",
      "May cause dangerous blood glucose spikes"
    ],
    "restrictions": ["High sugar content dangerous for diabetics"],
    "alternatives": [
      {
        "foodId": 67,
        "foodName": "Sugar-free Pudding",
        "price": 25000,
        "reason": "Low sugar alternative"
      }
    ],
    "requiresPhysicianApproval": true
  }
}
```

## Test Scenario 5: Filtered Menu Generation

### Test 5.1: Get Filtered Menu for Diabetic Patient

```http
GET /api/patientdietary/patient-123/filtered-menu?menuDate=2024-01-20
Authorization: Bearer {user-token}
```

**Expected Result:**

- Status: 200 OK
- Menu items categorized by safety level (Safe, Advisory, Warning, Prohibited)
- Each item includes validation status and restriction reasons
- Alternative recommendations provided for restricted items
- Vietnamese food names included where available

## Test Scenario 6: Integration with Existing Order System

### Test 6.1: Order Validation Integration

```http
POST /api/orders/validate
Authorization: Bearer {user-token}
Content-Type: application/json

{
    "patientId": "patient-123",
    "items": [
        {
            "foodId": 1,
            "quantity": 1
        },
        {
            "foodId": 15,
            "quantity": 1
        }
    ]
}
```

**Expected Result:**

- Status: 400 Bad Request (if dangerous items included)
- Detailed validation results for each item
- List of prohibited items with alternatives
- Clear error message about dietary restrictions

### Test 6.2: Successful Order Placement

```http
POST /api/orders
Authorization: Bearer {user-token}
Content-Type: application/json

{
    "patientId": "patient-123",
    "items": [
        {
            "foodId": 1,
            "quantity": 1
        },
        {
            "foodId": 5,
            "quantity": 1
        }
    ]
}
```

**Expected Result:**

- Status: 201 Created
- Order created successfully (all items are safe)
- Dietary validation passed silently

## Test Scenario 7: Multi-Tenancy Verification

### Test 7.1: Branch Isolation Test

```http
# Switch to different branch context
GET /api/diseasecategories
Authorization: Bearer {branch2-token}
```

**Expected Result:**

- Returns only disease categories for Branch 2
- No access to Branch 1 data
- Multi-tenancy filters working correctly

### Test 7.2: Cross-Branch Access Prevention

```http
# Try to access Branch 1 disease category from Branch 2
GET /api/diseasecategories/1
Authorization: Bearer {branch2-token}
```

**Expected Result:**

- Status: 404 Not Found
- Branch-level security enforced

## Test Scenario 8: Role-Based Access Control

### Test 8.1: Unauthorized Access Test

```http
POST /api/diseasecategories
Authorization: Bearer {patient-token}
Content-Type: application/json

{
    "name": "Test Category",
    "code": "TEST"
}
```

**Expected Result:**

- Status: 403 Forbidden
- Only Admin/Manager/Nutritionist can create disease categories

### Test 8.2: Doctor Assignment Rights

```http
POST /api/patientdietary/patient-123/disease-categories
Authorization: Bearer {nurse-token}
Content-Type: application/json

{
    "diseaseCategoryId": 1
}
```

**Expected Result:**

- Status: 403 Forbidden
- Only doctors can assign disease categories

## Test Scenario 9: Vietnamese-Specific Testing

### Test 9.1: Vietnamese Food Validation

```http
POST /api/patientdietary/patient-123/validate-food
Authorization: Bearer {user-token}
Content-Type: application/json

{
    "foodId": 101,
    "quantity": 1
}
```

**Expected Result for Bánh Chưng (Vietnamese Rice Cake):**

```json
{
  "status": "success",
  "data": {
    "isAllowed": false,
    "validationStatus": "Prohibited",
    "warnings": ["Bánh chưng chứa nhiều tinh bột và đường"],
    "restrictions": [
      "Bánh chưng chứa nhiều tinh bột và đường, có thể làm tăng đường huyết nguy hiểm"
    ],
    "alternatives": [
      {
        "foodId": 203,
        "foodName": "Brown Rice",
        "foodNameVi": "Cơm gạo lứt",
        "price": 18000,
        "reason": "Healthier carbohydrate option"
      }
    ],
    "requiresPhysicianApproval": false
  }
}
```

## Test Scenario 10: Performance and Edge Cases

### Test 10.1: Patient with Multiple Disease Categories

```http
GET /api/patientdietary/patient-789/dietary-info
Authorization: Bearer {user-token}
```

**Expected Result:**

- Patient has both Diabetes and Hypertension
- Restrictions from both conditions are merged
- Highest severity level is used
- All restrictions are properly aggregated

### Test 10.2: Large Menu Filtering Performance

```http
GET /api/patientdietary/patient-123/filtered-menu?menuDate=2024-01-20
Authorization: Bearer {user-token}
```

**Performance Expectations:**

- Response time < 2 seconds for menu with 100+ items
- Proper indexing on disease categories and food restrictions
- Efficient database queries with minimal N+1 problems

## Test Scenario 11: Error Handling and Edge Cases

### Test 11.1: Invalid Disease Category Code

```http
POST /api/diseasecategories
Authorization: Bearer {admin-token}
Content-Type: application/json

{
    "name": "Test Category",
    "code": "DM2",
    "severityLevel": 1
}
```

**Expected Result:**

- Status: 400 Bad Request
- Error message: "Disease category code already exists"

### Test 11.2: Non-Existent Patient

```http
GET /api/patientdietary/non-existent-patient/dietary-info
Authorization: Bearer {user-token}
```

**Expected Result:**

- Status: 404 Not Found
- Error message: "Patient not found or no dietary information available"

### Test 11.3: Expired Disease Category Assignment

```http
# Create assignment with expiry date in the past
POST /api/patientdietary/patient-123/disease-categories
Authorization: Bearer {doctor-token}
Content-Type: application/json

{
    "diseaseCategoryId": 5,
    "expiryDate": "2023-12-31T00:00:00Z"
}
```

**Expected Result:**

- Assignment created but marked as expired
- Not included in active dietary restrictions
- Dietary validation ignores expired assignments

## Automated Testing Scripts

### PowerShell Test Script Example

```powershell
# Test script to run all scenarios
$baseUrl = "https://localhost:7001/api"
$adminToken = "your-admin-jwt-token"

# Test 1: Create Disease Category
$response = Invoke-RestMethod -Uri "$baseUrl/diseasecategories" -Method Post -Headers @{
    "Authorization" = "Bearer $adminToken"
    "Content-Type" = "application/json"
} -Body (@{
    name = "Test Diabetes"
    code = "TEST_DM"
    severityLevel = 3
} | ConvertTo-Json)

Write-Host "Disease Category Created: $($response.data.id)"

# Test 2: Validate Food
$validateResponse = Invoke-RestMethod -Uri "$baseUrl/patientdietary/patient-123/validate-food" -Method Post -Headers @{
    "Authorization" = "Bearer $adminToken"
    "Content-Type" = "application/json"
} -Body (@{
    foodId = 15
    quantity = 1
} | ConvertTo-Json)

Write-Host "Food Validation Result: $($validateResponse.data.validationStatus)"
```

## Summary of Testing Coverage

✅ **CRUD Operations**: All basic operations for disease categories  
✅ **Food Restrictions**: Adding and managing food restrictions  
✅ **Patient Assignments**: Disease category assignments to patients  
✅ **Food Validation**: Real-time validation of food items  
✅ **Menu Filtering**: Generating safe menus for patients  
✅ **Integration**: Works with existing order system  
✅ **Multi-Tenancy**: Branch-level data isolation  
✅ **Authorization**: Role-based access control  
✅ **Vietnamese Context**: Local food and cultural considerations  
✅ **Performance**: Handling large datasets efficiently  
✅ **Error Handling**: Graceful handling of edge cases

This comprehensive testing approach ensures that the disease category management system integrates seamlessly with your existing HOMMS architecture while providing robust dietary management capabilities for Vietnamese hospital environments.
