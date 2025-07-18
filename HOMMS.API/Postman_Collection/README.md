# HOMMS API Postman Collection

This Postman collection contains all the API endpoints for the Hospital Canteen Order Management System (HOMMS).

## Files Included

- `postman_collections.json` - Main Postman collection with all API endpoints
- `postman_environment.json` - Environment variables for easy configuration
- `README.md` - This documentation file

## Setup Instructions

1. **Import the Collection:**

   - Open Postman
   - Click "Import" → "Choose Files"
   - Select `postman_collections.json`

2. **Import the Environment:**

   - Click "Import" → "Choose Files"
   - Select `postman_environment.json`
   - Set this environment as active in the top-right corner

3. **Configure Environment Variables:**
   - Click the eye icon (👁️) in the top-right corner
   - Edit the environment variables as needed:
     - `baseUrl`: Update to your API server URL (default: `https://localhost:7001`)
     - `branchId`: Set to your default branch ID
     - `branchCode`: Set to your default branch code

## Authentication

1. **Login:**

   - Use the "Auth" → "Login" endpoint
   - Default admin credentials are already set in the request body
   - Copy the token from the response

2. **Set JWT Token:**
   - Paste the token in the `jwtToken` environment variable
   - All subsequent requests will use this token automatically

## Available Endpoint Categories

### 🔐 Auth

- **Register** - Create new user account
- **Login** - Authenticate and get JWT token
- **Confirm Email** - Confirm user email address
- **Forgot Password** - Request password reset
- **Reset Password** - Reset user password
- **Get Profile** - Get current user profile
- **Select Branch** - Select working branch for user

### 🏢 Branches

- **Get All Branches** - List all hospital branches
- **Get Default Branch** - Get the default branch
- **Set Current Branch** - Set current working branch
- **Get Current Branch** - Get current working branch
- **Secure Action** - Test permission-based action
- **Get Admin System User** - Get admin user for branch
- **Assign Admin System** - Assign admin role to user
- **List Admin System Users** - List all admin users

### 🍽️ Foods

- **Get All (by Branch)** - List all foods for a branch
- **Get By Id** - Get specific food details
- **Create** - Add new food item
- **Update** - Update existing food item
- **Delete** - Remove food item

### 📂 FoodCategories

- **Get All (by Branch)** - List all food categories for a branch
- **Get By Id** - Get specific category details
- **Create** - Add new food category
- **Update** - Update existing category
- **Delete** - Remove food category

### 📋 Order

- **Get Chef Order List** - Get orders for kitchen/chef
- **Update Order Status By Chef** - Update order status from kitchen
- **Get Orders By BranchId** - Get all orders for a branch
- **Search Orders** - Search orders by keyword
- **Filter Orders** - Filter orders by date range

### 📝 OrderDetails

- **Get Order Details By OrderId** - Get detailed order information

### 💰 Revenue

- **Get Revenue By Day** - Daily revenue report
- **Get Revenue By Week** - Weekly revenue report
- **Get Revenue By Month** - Monthly revenue report
- **Get Revenue By Year** - Yearly revenue report

### 🍴 PublicMenuController

- **Get Foods and Categories by Date** - Get menu for current branch by date
- **Get Categories by Date** - Get food categories by date
- **Get Foods by Category and Date** - Get foods in specific category by date
- **Get Menus By Branch** - Get all menus for a branch
- **Delete Menu** - Remove a menu
- **Get Menu By Id** - Get specific menu details
- **Get Foods by Branch Date** - Get foods for specific branch and date
- **Get Categories by Branch Date** - Get categories for specific branch and date
- **Get Foods by Branch Category Date** - Get foods by branch, category, and date
- **Search Foods** - Search foods with filters

### 📋 MenuDetail

- **Get All Menu Details** - List all menu details
- **Get Menu Detail By Id** - Get specific menu detail
- **Update Menu Detail** - Update menu detail information
- **Add Menu Detail** - Create new menu detail

### 🏥 Patients

- **Get All Patients** - List all patients for a branch
- **Get Patient By Id** - Get specific patient details
- **Create Patient** - Add new patient record
- **Update Patient** - Update patient information
- **Link Patient to User** - Link patient record to user account

### 💳 UserWallet

- **Get Wallet Balance** - Get user's wallet balance
- **Add Money to Wallet** - Add funds to user wallet
- **Get Transaction History** - Get wallet transaction history
- **Create Customer Account** - Create new customer account with wallet
- **Wallet Adjustment** - Adjust wallet balance (admin)

### 🛒 PatientOrder

- **Create Patient Order** - Create order for patient with dietary validation
- **Validate Patient Order** - Validate order against patient's dietary restrictions

## Request Body Examples

### Food Creation

```json
{
  "name": "Bánh mì",
  "categoryId": 1,
  "description": "Bánh mì đặc biệt",
  "priceForGuest": 20000,
  "priceForPatient": 15000,
  "priceForStaff": 12000,
  "imageUrl": "images/Foods/banh-mi.jpg",
  "isSetDish": false,
  "isAddOn": false,
  "sort": 1
}
```

### Patient Order Creation

```json
{
  "patientId": "PAT001",
  "receiveDate": "2025-01-21T12:00:00.000Z",
  "receiveTime": "12:00",
  "receiveType": "Delivery",
  "paymentMethod": 1,
  "walletAmountUsed": 50000,
  "note": "Giao đúng giờ, bệnh nhân cần ăn đúng giờ",
  "orderDetails": [
    {
      "foodId": 1,
      "quantity": 2,
      "note": "Không cay"
    }
  ]
}
```

### Wallet Add Money

```json
{
  "userId": "{{userId}}",
  "amount": 500000,
  "description": "Nạp tiền vào ví từ admin"
}
```

## Common HTTP Status Codes

- **200 OK** - Request successful
- **201 Created** - Resource created successfully
- **400 Bad Request** - Invalid request data
- **401 Unauthorized** - Authentication required or failed
- **403 Forbidden** - Insufficient permissions
- **404 Not Found** - Resource not found
- **500 Internal Server Error** - Server error

## Notes

- All monetary amounts are in VND (Vietnamese Dong)
- Dates should be in ISO 8601 format (YYYY-MM-DDTHH:mm:ss.sssZ)
- The `X-Branch-Id` header is required for many endpoints
- Some endpoints require specific permissions based on user roles
- Patient orders include dietary validation and restriction checking

## Support

For questions about the API endpoints or collection usage, please refer to the main project documentation or contact the development team.
