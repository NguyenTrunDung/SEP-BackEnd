# Disease Category Management API Usage Examples

This document provides practical examples of how to use the Disease Category Management APIs with sample data that demonstrates the system working properly.

## Authentication

All APIs require authentication. Include the JWT token in the Authorization header:

```http
Authorization: Bearer {your-jwt-token}
```

## 1. Disease Category Management

### Create a Disease Category

**Request:**

```http
POST /api/diseasecategories
Content-Type: application/json

{
    "name": "Diabetes Type 2",
    "code": "DM2",
    "description": "Type 2 diabetes mellitus requiring dietary carbohydrate management",
    "dietaryRestrictions": "Avoid high sugar foods, limit simple carbohydrates, avoid sugary drinks",
    "recommendedFoods": "Whole grains, lean proteins, vegetables, low-glycemic fruits",
    "severityLevel": 3,
    "requiresApproval": true,
    "colorCode": "#FF6B6B",
    "sortOrder": 1
}
```

**Response:**

```json
{
  "status": "success",
  "message": "Disease category created successfully",
  "data": {
    "id": 1,
    "branchId": 1,
    "name": "Diabetes Type 2",
    "code": "DM2",
    "description": "Type 2 diabetes mellitus requiring dietary carbohydrate management",
    "dietaryRestrictions": "Avoid high sugar foods, limit simple carbohydrates, avoid sugary drinks",
    "recommendedFoods": "Whole grains, lean proteins, vegetables, low-glycemic fruits",
    "isActive": true,
    "severityLevel": 3,
    "requiresApproval": true,
    "colorCode": "#FF6B6B",
    "sortOrder": 1,
    "branchName": "Can Tho General Hospital",
    "patientCount": 0,
    "foodCount": 0,
    "restrictionCount": 0
  }
}
```

### Get All Disease Categories

**Request:**

```http
GET /api/diseasecategories
```

**Response:**

```json
{
  "status": "success",
  "message": "Disease categories retrieved successfully",
  "data": [
    {
      "id": 1,
      "name": "Diabetes Type 2",
      "code": "DM2",
      "severityLevel": 3,
      "patientCount": 5,
      "restrictionCount": 12
    },
    {
      "id": 2,
      "name": "Hypertension",
      "code": "HTN",
      "severityLevel": 2,
      "patientCount": 8,
      "restrictionCount": 6
    }
  ]
}
```

### Add Food Restriction to Disease Category

**Request:**

```http
POST /api/diseasecategories/1/food-restrictions
Content-Type: application/json

{
    "foodId": 15,
    "restrictionLevel": 4,
    "reason": "Contains high sugar content that can cause dangerous blood glucose spikes",
    "alternativeRecommendations": "Sugar-free desserts, fresh fruits with low glycemic index",
    "requiresPhysicianOverride": true
}
```

**Response:**

```json
{
  "status": "success",
  "message": "Food restriction added successfully",
  "data": {
    "id": 1,
    "diseaseCategoryId": 1,
    "foodId": 15,
    "restrictionLevel": 4,
    "reason": "Contains high sugar content that can cause dangerous blood glucose spikes",
    "alternativeRecommendations": "Sugar-free desserts, fresh fruits with low glycemic index",
    "requiresPhysicianOverride": true,
    "diseaseCategoryName": "Diabetes Type 2",
    "foodName": "Chocolate Cake",
    "restrictionLevelText": "Dangerous"
  }
}
```

## 2. Patient Dietary Management

### Assign Disease Category to Patient

**Request:**

```http
POST /api/patientdietary/patient-123/disease-categories
Content-Type: application/json

{
    "diseaseCategoryId": 1,
    "diagnosedDate": "2024-01-15T00:00:00Z",
    "patientSeverityLevel": 3,
    "patientSpecificNotes": "Patient has difficulty controlling blood sugar, requires strict monitoring",
    "assignedByPhysician": "Dr. Nguyen Van A",
    "expiryDate": null
}
```

**Response:**

```json
{
  "status": "success",
  "message": "Disease category assigned to patient successfully",
  "data": {
    "id": 1,
    "patientId": "patient-123",
    "diseaseCategoryId": 1,
    "diagnosedDate": "2024-01-15T00:00:00Z",
    "patientSeverityLevel": 3,
    "patientSpecificNotes": "Patient has difficulty controlling blood sugar, requires strict monitoring",
    "assignedByPhysician": "Dr. Nguyen Van A",
    "patientName": "Nguyen Thi B",
    "patientCode": "BN2024001",
    "diseaseCategoryName": "Diabetes Type 2",
    "diseaseCategoryCode": "DM2",
    "roomNumber": "A301",
    "bedNumber": "02"
  }
}
```

### Get Patient Dietary Information

**Request:**

```http
GET /api/patientdietary/patient-123/dietary-info
```

**Response:**

```json
{
  "status": "success",
  "message": "Patient dietary information retrieved successfully",
  "data": {
    "patientId": "patient-123",
    "patientName": "Nguyen Thi B",
    "patientCode": "BN2024001",
    "roomNumber": "A301",
    "bedNumber": "02",
    "requiresDietarySupervision": true,
    "diseaseCategories": [
      {
        "id": 1,
        "diseaseCategoryName": "Diabetes Type 2",
        "diseaseCategoryCode": "DM2",
        "patientSeverityLevel": 3,
        "diagnosedDate": "2024-01-15T00:00:00Z",
        "assignedByPhysician": "Dr. Nguyen Van A"
      }
    ],
    "restrictedFoodIds": [15, 23, 31, 45],
    "dietaryRestrictions": [
      "Avoid high sugar foods, limit simple carbohydrates, avoid sugary drinks"
    ],
    "recommendedFoods": [
      "Whole grains, lean proteins, vegetables, low-glycemic fruits"
    ],
    "highestSeverityLevel": 3,
    "requiresApproval": true
  }
}
```

## 3. Food Validation for Patients

### Validate Single Food Item

**Request:**

```http
POST /api/patientdietary/patient-123/validate-food
Content-Type: application/json

{
    "foodId": 15,
    "quantity": 1
}
```

**Response (Prohibited Food):**

```json
{
  "status": "success",
  "message": "Food validation completed",
  "data": {
    "isAllowed": false,
    "validationStatus": "Dangerous",
    "warnings": [
      "This food contains high sugar content",
      "May cause dangerous blood glucose spikes"
    ],
    "restrictions": [
      "Contains high sugar content that can cause dangerous blood glucose spikes"
    ],
    "alternatives": [
      {
        "foodId": 67,
        "foodName": "Sugar-free Pudding",
        "foodNameVi": "Bánh pudding không đường",
        "price": 25000,
        "reason": "Low sugar alternative dessert"
      },
      {
        "foodId": 68,
        "foodName": "Fresh Fruit Salad",
        "foodNameVi": "Salad trái cây tươi",
        "price": 30000,
        "reason": "Natural sugars with fiber"
      }
    ],
    "requiresPhysicianApproval": true
  }
}
```

**Response (Safe Food):**

```json
{
  "status": "success",
  "message": "Food validation completed",
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

### Get Filtered Menu for Patient

**Request:**

```http
GET /api/patientdietary/patient-123/filtered-menu?menuDate=2024-01-20
```

**Response:**

```json
{
  "status": "success",
  "message": "Filtered menu retrieved successfully",
  "data": {
    "patientId": "patient-123",
    "patientName": "Nguyen Thi B",
    "menuDate": "2024-01-20",
    "safeFoods": [
      {
        "foodId": 1,
        "foodName": "Grilled Chicken Breast",
        "foodNameVi": "Ức gà nướng",
        "description": "Lean protein with herbs",
        "priceForPatient": 45000,
        "validationStatus": "Safe",
        "restrictionReasons": [],
        "alternatives": [],
        "requiresApproval": false
      },
      {
        "foodId": 5,
        "foodName": "Steamed Vegetables",
        "foodNameVi": "Rau củ hấp",
        "description": "Mixed seasonal vegetables",
        "priceForPatient": 25000,
        "validationStatus": "Safe",
        "restrictionReasons": [],
        "alternatives": [],
        "requiresApproval": false
      }
    ],
    "advisoryFoods": [
      {
        "foodId": 8,
        "foodName": "White Rice",
        "foodNameVi": "Cơm trắng",
        "description": "Steamed jasmine rice",
        "priceForPatient": 15000,
        "validationStatus": "Advisory",
        "restrictionReasons": [
          "High glycemic index - consider brown rice alternative"
        ],
        "alternatives": [
          {
            "foodId": 9,
            "foodName": "Brown Rice",
            "foodNameVi": "Cơm gạo lứt",
            "price": 18000,
            "reason": "Lower glycemic index"
          }
        ],
        "requiresApproval": false
      }
    ],
    "warningFoods": [
      {
        "foodId": 12,
        "foodName": "Fried Rice",
        "foodNameVi": "Cơm chiên",
        "description": "Wok-fried rice with vegetables",
        "priceForPatient": 35000,
        "validationStatus": "Warning",
        "restrictionReasons": ["High oil content and refined carbohydrates"],
        "alternatives": [
          {
            "foodId": 9,
            "foodName": "Brown Rice",
            "foodNameVi": "Cơm gạo lứt",
            "price": 18000,
            "reason": "Healthier carbohydrate option"
          }
        ],
        "requiresApproval": false
      }
    ],
    "prohibitedFoods": [
      {
        "foodId": 15,
        "foodName": "Chocolate Cake",
        "foodNameVi": "Bánh chocolate",
        "description": "Rich chocolate layer cake",
        "priceForPatient": 50000,
        "validationStatus": "Dangerous",
        "restrictionReasons": [
          "Contains high sugar content that can cause dangerous blood glucose spikes"
        ],
        "alternatives": [
          {
            "foodId": 67,
            "foodName": "Sugar-free Pudding",
            "foodNameVi": "Bánh pudding không đường",
            "price": 25000,
            "reason": "Low sugar alternative dessert"
          }
        ],
        "requiresApproval": true
      }
    ],
    "totalDiseaseCategories": 1,
    "highestSeverityLevel": 3
  }
}
```

## 4. Real-World Test Scenarios

### Scenario 1: Diabetic Patient Ordering

**Setup:**

1. Patient: Nguyen Thi B (ID: patient-123)
2. Disease: Diabetes Type 2 (Severity: High)
3. Room: A301, Bed: 02

**Test Case 1: Order Safe Food**

```bash
# Patient tries to order grilled chicken
curl -X POST "https://api.yourhotel.com/api/patientdietary/patient-123/validate-food" \
-H "Authorization: Bearer {token}" \
-H "Content-Type: application/json" \
-d '{"foodId": 1, "quantity": 1}'

# Expected: isAllowed: true, validationStatus: "Safe"
```

**Test Case 2: Order Prohibited Food**

```bash
# Patient tries to order chocolate cake
curl -X POST "https://api.yourhotel.com/api/patientdietary/patient-123/validate-food" \
-H "Authorization: Bearer {token}" \
-H "Content-Type: application/json" \
-d '{"foodId": 15, "quantity": 1}'

# Expected: isAllowed: false, validationStatus: "Dangerous", alternatives provided
```

### Scenario 2: Hypertensive Patient Ordering

**Setup:**

1. Patient: Tran Van C (ID: patient-456)
2. Disease: Hypertension (Severity: Medium)
3. Room: B205, Bed: 01

**Test Case: Order High-Sodium Food**

```bash
# Patient tries to order pho (high sodium)
curl -X POST "https://api.yourhotel.com/api/patientdietary/patient-456/validate-food" \
-H "Authorization: Bearer {token}" \
-H "Content-Type: application/json" \
-d '{"foodId": 25, "quantity": 1}'

# Expected: isAllowed: false, validationStatus: "Warning", low-sodium alternatives suggested
```

### Scenario 3: Multiple Disease Categories

**Setup:**

1. Patient: Le Thi D (ID: patient-789)
2. Diseases: Diabetes Type 2 + Hypertension
3. Room: C102, Bed: 03

**Test Case: Complex Validation**

```bash
# Get filtered menu for patient with multiple conditions
curl -X GET "https://api.yourhotel.com/api/patientdietary/patient-789/filtered-menu?menuDate=2024-01-20" \
-H "Authorization: Bearer {token}"

# Expected: Highly filtered menu with foods safe for both conditions
```

## 5. Vietnamese-Specific Test Data

### Sample Vietnamese Food Items with Restrictions

```json
{
  "vietnameseFoodsWithRestrictions": [
    {
      "foodId": 101,
      "name": "Phở Bò",
      "nameVi": "Phở bò",
      "restrictions": {
        "hypertension": {
          "level": "Warning",
          "reason": "High sodium content in broth",
          "alternatives": ["Low-sodium pho", "Bun bo hue (modified)"]
        }
      }
    },
    {
      "foodId": 102,
      "name": "Bánh Chưng",
      "nameVi": "Bánh chưng",
      "restrictions": {
        "diabetes": {
          "level": "Prohibited",
          "reason": "High starch and carbohydrate content",
          "alternatives": ["Vegetable spring rolls", "Grilled fish"]
        }
      }
    },
    {
      "foodId": 103,
      "name": "Chè Đậu Xanh",
      "nameVi": "Chè đậu xanh",
      "restrictions": {
        "diabetes": {
          "level": "Dangerous",
          "reason": "Very high sugar content",
          "alternatives": ["Fresh fruit", "Sugar-free pudding"]
        }
      }
    }
  ]
}
```

## 6. Error Handling Examples

### Invalid Disease Category Assignment

**Request:**

```http
POST /api/patientdietary/patient-123/disease-categories
Content-Type: application/json

{
    "diseaseCategoryId": 999,
    "diagnosedDate": "2024-01-15T00:00:00Z"
}
```

**Response:**

```json
{
  "status": "error",
  "message": "Disease category not found",
  "data": null
}
```

### Duplicate Assignment

**Response:**

```json
{
  "status": "error",
  "message": "Patient already has this disease category assigned",
  "data": null
}
```

## 7. Integration with Existing Order System

### Enhanced Food Controller Integration

When a patient places an order, the system should validate each item:

```csharp
// In your existing FoodController or OrderController
[HttpPost("validate-order")]
public async Task<IActionResult> ValidateOrder([FromBody] CreateOrderDto orderDto)
{
    // If patient has disease categories, validate each food item
    if (!string.IsNullOrEmpty(orderDto.PatientId))
    {
        var validationTasks = orderDto.Items.Select(item =>
            _dietaryValidationService.ValidateFoodForPatientAsync(new ValidateFoodForPatientDto
            {
                PatientId = orderDto.PatientId,
                FoodId = item.FoodId,
                Quantity = item.Quantity
            }));

        var validations = await Task.WhenAll(validationTasks);

        var prohibitedItems = validations.Where(v => !v.IsAllowed).ToList();
        if (prohibitedItems.Any())
        {
            return BadRequest(ApiResponseBase<object>.Error(
                "Order contains prohibited items for patient's dietary restrictions",
                prohibitedItems));
        }
    }

    // Continue with normal order processing...
}
```

This comprehensive API guide demonstrates how the disease category management system integrates seamlessly with your existing architecture while providing robust dietary management capabilities for hospital patients.
