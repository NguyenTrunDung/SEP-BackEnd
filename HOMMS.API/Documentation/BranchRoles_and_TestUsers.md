# Branch Roles and Test Users Documentation

This document outlines all the branch roles and test user accounts that are automatically seeded in the HOMMS (Hospital Canteen Order Management System) for authentication and authorization testing.

## Branch Roles and Permissions

### 1. **Admin System** (Global)

- **Description**: Full system administration access
- **Default Role**: No
- **Permissions**: All system permissions
- **Use Case**: System-wide administration, can manage all branches and system settings

### 2. **Quản lý chi nhánh** (Branch Manager)

- **Description**: Branch-level management with comprehensive permissions
- **Default Role**: No
- **Key Permissions**:
  - All food and category management
  - Complete order management (view, add, edit, delete, approve, cancel)
  - Full menu management
  - Kitchen and delivery operations management
  - User management within branch
  - Patient management with dietary supervision
  - Branch settings management (can edit own branch)
  - Complete reporting and analytics
  - Wallet and financial management
  - Areas and locations management
- **Use Case**: Senior manager responsible for entire branch operations

### 3. **Quản lý** (Manager)

- **Description**: Standard management role
- **Default Role**: Yes
- **Key Permissions**:
  - Food and category management
  - Order management (view, add, edit, delete, approve)
  - Menu management
  - Basic kitchen and delivery oversight
  - User management
  - Patient management
  - Financial reporting
  - Wallet transactions
- **Use Case**: Day-to-day operational management

### 4. **Trưởng phòng Y tá** (Head Nurse)

- **Description**: Senior nursing staff with patient focus
- **Default Role**: No
- **Key Permissions**:
  - Order management for patients
  - Complete patient management including dietary supervision
  - Can manage other nurses
  - Patient-focused reporting
- **Use Case**: Senior nursing staff overseeing patient care and dietary needs

### 5. **Trưởng bếp** (Head Chef)

- **Description**: Kitchen management with food operations focus
- **Default Role**: No
- **Key Permissions**:
  - Complete kitchen operations
  - Food and category management
  - Menu planning and management
  - Can oversee kitchen staff
  - Order processing and status updates
- **Use Case**: Kitchen operations manager

### 6. **Nhân viên** (Staff)

- **Description**: General staff members
- **Default Role**: No
- **Key Permissions**:
  - Basic food and order management
  - Menu access
  - Patient interaction
  - Basic reporting
- **Use Case**: General purpose staff role

### 7. **Nhà bếp** (Kitchen Staff)

- **Description**: Kitchen workers focused on food preparation
- **Default Role**: No
- **Key Permissions**:
  - Kitchen operations
  - Order status updates
  - Food preparation tracking
  - Menu viewing
- **Use Case**: Kitchen staff responsible for food preparation

### 8. **Y tá** (Nurse)

- **Description**: Nursing staff with patient care focus
- **Default Role**: No
- **Key Permissions**:
  - Patient management and dietary oversight
  - Order placement for patients
  - Patient-focused operations
- **Use Case**: Nursing staff caring for patients

### 9. **Giao hàng** (Delivery Staff)

- **Description**: Delivery personnel
- **Default Role**: No
- **Key Permissions**:
  - Delivery management
  - Order tracking
  - Patient location access
- **Use Case**: Staff responsible for food delivery

### 10. **Kế toán** (Accountant)

- **Description**: Financial management focus
- **Default Role**: No
- **Key Permissions**:
  - Financial reporting and analytics
  - Wallet management
  - Revenue tracking
  - Order financial data
- **Use Case**: Financial operations and reporting

### 11. **Nhân viên lễ tân** (Receptionist)

- **Description**: Front desk and customer service
- **Default Role**: No
- **Key Permissions**:
  - Basic order management
  - Patient registration and basic info
  - Menu viewing
  - Wallet viewing
- **Use Case**: Front desk staff handling customer interactions

### 12. **Thực tập sinh** (Intern)

- **Description**: Trainee with limited access
- **Default Role**: No
- **Key Permissions**:
  - View-only access to most features
  - Basic overview and learning access
- **Use Case**: Interns and trainees learning the system

## Test User Accounts

The following test user accounts are automatically created with their respective roles:

| Email                      | Password            | Name              | Role              | Description                             |
| -------------------------- | ------------------- | ----------------- | ----------------- | --------------------------------------- |
| `branch.manager@homms.com` | `BranchManager@123` | Nguyễn Chi Nhánh  | Quản lý chi nhánh | Branch manager with full branch control |
| `manager@homms.com`        | `Manager@123`       | Lê Quản Lý        | Quản lý           | Standard manager role                   |
| `staff@homms.com`          | `Staff@123`         | Trần Nhân Viên    | Nhân viên         | General staff member                    |
| `chef@homms.com`           | `Chef@123`          | Phạm Đầu Bếp      | Trưởng bếp        | Head chef managing kitchen              |
| `kitchen@homms.com`        | `Kitchen@123`       | Võ Bếp Phụ        | Nhà bếp           | Kitchen worker                          |
| `nurse.head@homms.com`     | `NurseHead@123`     | Hoàng Y Tá Trưởng | Trưởng phòng Y tá | Head nurse managing patient care        |
| `nurse@homms.com`          | `Nurse@123`         | Đặng Y Tá         | Y tá              | Nursing staff                           |
| `delivery@homms.com`       | `Delivery@123`      | Bùi Giao Hàng     | Giao hàng         | Delivery personnel                      |
| `accountant@homms.com`     | `Accountant@123`    | Phan Kế Toán      | Kế toán           | Financial staff                         |
| `receptionist@homms.com`   | `Receptionist@123`  | Lý Lễ Tân         | Nhân viên lễ tân  | Front desk staff                        |
| `intern@homms.com`         | `Intern@123`        | Cao Thực Tập      | Thực tập sinh     | Intern with limited access              |

## Additional Admin Account

| Email             | Password       | Name       | Role         | Description                           |
| ----------------- | -------------- | ---------- | ------------ | ------------------------------------- |
| `admin@homms.com` | `Admin@123456` | Admin User | Admin System | System administrator with full access |

## Usage Instructions

### For Testing Authentication:

1. **Login with different roles** to test permission-based access
2. **Use branch.manager@homms.com** to test branch management features
3. **Use specific role accounts** to test role-specific workflows
4. **Test permission boundaries** by attempting unauthorized actions

### For Development:

1. **Role-based UI** should show/hide features based on user permissions
2. **API endpoints** should validate permissions from database
3. **Permission checks** should use the `permissions` field from login response
4. **Branch context** should be maintained throughout user session

### For API Testing:

1. Login with any test account to get access token and permissions
2. Use returned permissions to test API authorization
3. Test branch selection with different roles
4. Verify permission-based access control

## Permission Categories Reference

- **overview**: Dashboard and general viewing
- **foods**: Food item management
- **foodcategories**: Food category management
- **orders**: Order processing and management
- **menus**: Menu planning and management
- **kitchen**: Kitchen operations and food preparation
- **delivery**: Delivery operations
- **users**: User account management
- **patients**: Patient management and care
- **branches**: Branch management and settings
- **reports**: Analytics and reporting
- **wallet**: Financial and wallet operations
- **system**: System administration
- **areas**: Area/zone management
- **locations**: Location management

## Security Notes

- **Test passwords** should be changed in production
- **Permission changes** take effect immediately without re-login
- **Branch isolation** ensures users only access assigned branches
- **Role hierarchy** is enforced through permission sets
- **Database-driven permissions** allow real-time permission updates

This comprehensive role and user system provides a realistic testing environment for the hospital canteen management system with proper separation of concerns and realistic organizational roles.
