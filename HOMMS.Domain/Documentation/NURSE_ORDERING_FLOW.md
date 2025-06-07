# Nurse Ordering Flow: Ordering Meals for Patients

## 🏥 Overview

This guide shows how nurses can order food meals for patients based on their disease categories. Nurses can order for multiple patients and manage dietary restrictions on their behalf.

## 🔄 Modified Flow for Nurses

### Step 1: **Nurse Authentication with Enhanced Permissions**

```javascript
// 1. Nurse logs in with appropriate role
const loginResponse = await fetch("/api/auth/login", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({
    email: "nurse@hospital.com",
    password: "password123",
  }),
});

const { data: authData } = await loginResponse.json();
const token = authData.token;

// Verify nurse has permission to order for patients
const permissions = authData.permissions; // Should include 'ORDER_FOR_PATIENTS'
```

### Step 2: **Patient Selection Interface**

```javascript
// 2. Get list of patients assigned to this nurse
const patientsResponse = await fetch("/api/nurse/assigned-patients", {
  headers: { Authorization: `Bearer ${token}` },
});

const { data: assignedPatients } = await patientsResponse.json();

// Example response:
// {
//   "data": [
//     {
//       "patientId": "P001",
//       "name": "Nguyễn Văn A",
//       "roomNumber": "101",
//       "bedNumber": "A",
//       "diseaseCategories": ["DIABETES", "HYPERTENSION"],
//       "dietaryRestrictions": ["Low Sodium", "Low Sugar"],
//       "hasActiveOrders": false,
//       "lastOrderTime": "2024-01-14T10:30:00Z"
//     },
//     {
//       "patientId": "P002",
//       "name": "Trần Thị B",
//       "roomNumber": "102",
//       "bedNumber": "B",
//       "diseaseCategories": ["HEART_DISEASE"],
//       "dietaryRestrictions": ["Low Fat"],
//       "hasActiveOrders": true,
//       "lastOrderTime": "2024-01-15T08:15:00Z"
//     }
//   ]
// }
```

### Step 3: **Patient Selection Component**

```jsx
function PatientSelector({ patients, onPatientSelect }) {
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [searchTerm, setSearchTerm] = useState("");

  const filteredPatients = patients.filter(
    (patient) =>
      patient.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      patient.roomNumber.includes(searchTerm)
  );

  return (
    <div className="patient-selector">
      <h2>Chọn bệnh nhân để đặt món</h2>

      <input
        type="text"
        placeholder="Tìm theo tên hoặc phòng..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        className="search-input"
      />

      <div className="patient-list">
        {filteredPatients.map((patient) => (
          <div
            key={patient.patientId}
            className={`patient-card ${
              selectedPatient?.patientId === patient.patientId ? "selected" : ""
            }`}
            onClick={() => {
              setSelectedPatient(patient);
              onPatientSelect(patient);
            }}
          >
            <div className="patient-info">
              <h3>{patient.name}</h3>
              <p>
                Phòng: {patient.roomNumber} - Giường: {patient.bedNumber}
              </p>
              <div className="disease-tags">
                {patient.diseaseCategories.map((disease) => (
                  <span key={disease} className="disease-tag">
                    {disease}
                  </span>
                ))}
              </div>
              {patient.hasActiveOrders && (
                <span className="active-order-badge">🍽️ Có đơn hàng</span>
              )}
            </div>
          </div>
        ))}
      </div>

      {selectedPatient && (
        <button
          className="continue-btn"
          onClick={() => onPatientSelect(selectedPatient)}
        >
          Đặt món cho {selectedPatient.name}
        </button>
      )}
    </div>
  );
}
```

### Step 4: **Get Patient's Dietary Information**

```javascript
// 3. Get specific patient's dietary information
async function getPatientDietaryInfo(patientId) {
  const response = await fetch(`/api/nurse/patient-dietary/${patientId}`, {
    headers: { Authorization: `Bearer ${token}` },
  });

  const { data: patientDietary } = await response.json();

  // Example response:
  // {
  //   "data": {
  //     "patientId": "P001",
  //     "patientName": "Nguyễn Văn A",
  //     "diseaseCategories": [
  //       {
  //         "id": 1,
  //         "name": "Tiểu đường",
  //         "code": "DIABETES",
  //         "severity": "Moderate"
  //       },
  //       {
  //         "id": 2,
  //         "name": "Tăng huyết áp",
  //         "code": "HYPERTENSION",
  //         "severity": "Mild"
  //       }
  //     ],
  //     "specialDietaryNotes": "Hạn chế muối và đường",
  //     "allergens": ["Seafood"],
  //     "isActive": true,
  //     "nurseCanOrder": true
  //   }
  // }

  return patientDietary;
}
```

### Step 5: **Enhanced Menu Display for Nurses**

```jsx
function NurseMenuItemCard({ item, patient }) {
  const getValidationStyle = (level) => {
    switch (level) {
      case "Safe":
        return { color: "green", icon: "✅", bg: "#e8f5e8" };
      case "Advisory":
        return { color: "blue", icon: "ℹ️", bg: "#e8f4fd" };
      case "Warning":
        return { color: "orange", icon: "⚠️", bg: "#fff3cd" };
      case "Dangerous":
        return { color: "red", icon: "❌", bg: "#f8d7da" };
      default:
        return { color: "gray", icon: "?", bg: "#f8f9fa" };
    }
  };

  const validation = item.validationResult;
  const style = getValidationStyle(validation.level);

  return (
    <div
      className="nurse-menu-item-card"
      style={{ borderLeft: `4px solid ${style.color}` }}
    >
      <div className="patient-context">
        <small>
          Đặt cho: <strong>{patient.name}</strong> - Phòng {patient.roomNumber}
        </small>
      </div>

      <div className="item-header">
        <h3>{item.name}</h3>
        <span className="price">{item.price.toLocaleString("vi-VN")} VNĐ</span>
      </div>

      <p className="description">{item.description}</p>

      <div className="validation-info" style={{ backgroundColor: style.bg }}>
        <span className="validation-icon">{style.icon}</span>
        <div>
          <strong className="validation-level" style={{ color: style.color }}>
            {validation.level === "Safe"
              ? "An toàn"
              : validation.level === "Advisory"
              ? "Tham khảo"
              : validation.level === "Warning"
              ? "Cảnh báo"
              : "Nguy hiểm"}
          </strong>
          <p className="validation-message">{validation.message}</p>

          {validation.nurseNotes && (
            <div className="nurse-notes">
              <strong>Ghi chú y tá:</strong> {validation.nurseNotes}
            </div>
          )}

          {validation.recommendations && (
            <ul className="recommendations">
              {validation.recommendations.map((rec, idx) => (
                <li key={idx}>{rec}</li>
              ))}
            </ul>
          )}
        </div>
      </div>

      <button
        className={`add-to-cart ${
          validation.level === "Dangerous" ? "professional-override" : ""
        }`}
        onClick={() => handleNurseAddToCart(item, patient)}
      >
        {validation.level === "Dangerous"
          ? "Cần Override Chuyên Môn"
          : `Đặt cho ${patient.name}`}
      </button>
    </div>
  );
}
```

### Step 6: **Professional Override for Dangerous Items**

```javascript
// 5. Handle adding items with professional nurse judgment
async function handleNurseAddToCart(item, patient) {
  if (item.validationResult.level === "Dangerous") {
    // Show professional override dialog for nurses
    const override = await showNurseProfessionalOverride({
      item,
      patient,
      validation: item.validationResult,
    });

    if (!override.confirmed) return;

    // Log the professional override decision
    await logNurseOverride({
      nurseId: currentNurse.id,
      patientId: patient.patientId,
      menuItemId: item.id,
      overrideReason: override.reason,
      timestamp: new Date().toISOString(),
    });

    item.isOverride = true;
    item.overrideReason = override.reason;
  }

  if (item.validationResult.level === "Warning") {
    const confirmed = await showNurseConfirmation({
      title: "Xác nhận đặt món",
      message: `Bạn có chắc chắn muốn đặt "${item.name}" cho bệnh nhân ${patient.name}?`,
      warning: item.validationResult.message,
      recommendations: item.validationResult.recommendations,
    });

    if (!confirmed) return;
  }

  // Add to cart with patient context
  addToNurseCart({
    ...item,
    patientId: patient.patientId,
    patientName: patient.name,
    roomNumber: patient.roomNumber,
    orderedBy: currentNurse.name,
    orderTime: new Date().toISOString(),
  });
}

// Professional override dialog for dangerous items
async function showNurseProfessionalOverride({ item, patient, validation }) {
  return new Promise((resolve) => {
    const modal = document.createElement("div");
    modal.innerHTML = `
      <div class="modal-overlay">
        <div class="professional-override-modal">
          <h3>⚠️ Xác nhận chuyên môn</h3>
          <p><strong>Món ăn:</strong> ${item.name}</p>
          <p><strong>Bệnh nhân:</strong> ${patient.name}</p>
          <p><strong>Cảnh báo:</strong> ${validation.message}</p>
          
          <div class="override-section">
            <label for="override-reason">Lý do chuyên môn:</label>
            <textarea 
              id="override-reason" 
              placeholder="Nhập lý do chuyên môn để override cảnh báo này..."
              required
            ></textarea>
          </div>
          
          <div class="modal-buttons">
            <button onclick="handleCancel()" class="cancel-btn">Hủy bỏ</button>
            <button onclick="handleConfirm()" class="override-btn">Xác nhận Override</button>
          </div>
        </div>
      </div>
    `;

    modal.querySelector(".cancel-btn").onclick = () => {
      document.body.removeChild(modal);
      resolve({ confirmed: false });
    };

    modal.querySelector(".override-btn").onclick = () => {
      const reason = modal.querySelector("#override-reason").value.trim();
      if (!reason) {
        alert("Vui lòng nhập lý do chuyên môn");
        return;
      }
      document.body.removeChild(modal);
      resolve({ confirmed: true, reason });
    };

    document.body.appendChild(modal);
  });
}
```

### Step 7: **Multi-Patient Cart Management**

```jsx
function NurseShoppingCart({ cartItems }) {
  // Group items by patient
  const itemsByPatient = cartItems.reduce((acc, item) => {
    if (!acc[item.patientId]) {
      acc[item.patientId] = {
        patient: {
          id: item.patientId,
          name: item.patientName,
          roomNumber: item.roomNumber,
        },
        items: [],
      };
    }
    acc[item.patientId].items.push(item);
    return acc;
  }, {});

  return (
    <div className="nurse-shopping-cart">
      <h2>Đơn hàng của bạn</h2>
      <p>Đang đặt cho {Object.keys(itemsByPatient).length} bệnh nhân</p>

      {Object.values(itemsByPatient).map(({ patient, items }) => (
        <div key={patient.id} className="patient-order-section">
          <h3>
            🏥 {patient.name} - Phòng {patient.roomNumber}
          </h3>

          <div className="patient-items">
            {items.map((item) => (
              <div key={item.id} className="cart-item">
                <span>{item.name}</span>
                <span>{item.price.toLocaleString("vi-VN")} VNĐ</span>
                {item.isOverride && (
                  <span className="override-badge" title={item.overrideReason}>
                    Override
                  </span>
                )}
                <button
                  onClick={() => removeFromCart(item.id)}
                  className="remove-btn"
                >
                  ✕
                </button>
              </div>
            ))}
          </div>

          <div className="patient-total">
            <strong>
              Tổng cho {patient.name}:{" "}
              {items
                .reduce((sum, item) => sum + item.price, 0)
                .toLocaleString("vi-VN")}{" "}
              VNĐ
            </strong>
          </div>
        </div>
      ))}

      <div className="cart-summary">
        <h3>
          Tổng cộng:{" "}
          {cartItems
            .reduce((sum, item) => sum + item.price, 0)
            .toLocaleString("vi-VN")}{" "}
          VNĐ
        </h3>
      </div>

      <div className="cart-actions">
        <button onClick={handleBulkCheckout} className="checkout-all-btn">
          Đặt tất cả ({cartItems.length} món)
        </button>
      </div>
    </div>
  );
}
```

### Step 8: **Bulk Order Placement**

```javascript
// 6. Place orders for multiple patients
async function handleBulkCheckout(cartItems) {
  // Group by patient
  const ordersByPatient = cartItems.reduce((acc, item) => {
    if (!acc[item.patientId]) {
      acc[item.patientId] = [];
    }
    acc[item.patientId].push(item);
    return acc;
  }, {});

  const orderPromises = Object.entries(ordersByPatient).map(
    async ([patientId, items]) => {
      // Final validation for each patient
      const finalValidation = await fetch(
        `/api/nurse/validate-patient-order/${patientId}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            menuItemIds: items.map((item) => item.id),
            nurseOverrides: items
              .filter((item) => item.isOverride)
              .map((item) => ({
                menuItemId: item.id,
                overrideReason: item.overrideReason,
              })),
          }),
        }
      );

      const validation = await finalValidation.json();

      if (validation.data.canProceed) {
        // Place order for this patient
        const orderResponse = await fetch("/api/nurse/orders", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            patientId: patientId,
            menuItemIds: items.map((item) => ({
              menuItemId: item.id,
              quantity: item.quantity || 1,
            })),
            orderedByNurse: true,
            nurseId: currentNurse.id,
            dietaryValidationId: validation.data.validationId,
            specialInstructions: `Đặt bởi Y tá ${currentNurse.name}`,
            overrides: items
              .filter((item) => item.isOverride)
              .map((item) => ({
                menuItemId: item.id,
                reason: item.overrideReason,
              })),
          }),
        });

        return await orderResponse.json();
      } else {
        throw new Error(
          `Không thể đặt hàng cho bệnh nhân ${
            items[0].patientName
          }: ${validation.data.blockingReasons.join(", ")}`
        );
      }
    }
  );

  try {
    const orderResults = await Promise.all(orderPromises);

    // Show success summary
    showNurseOrderSuccess(orderResults.map((r) => r.data));

    // Clear cart
    clearNurseCart();
  } catch (error) {
    alert(`Có lỗi khi đặt hàng: ${error.message}`);
  }
}
```

## 🔑 **Key Differences for Nurse Role:**

### **1. Enhanced Permissions**

- ✅ Access to multiple patient records
- ✅ Professional override capabilities for dangerous items
- ✅ Bulk ordering for multiple patients
- ✅ Audit trail for all actions

### **2. Patient Selection Interface**

- 🔍 Search and filter assigned patients
- 📋 Display patient dietary restrictions
- 📊 Show existing orders status
- 🏥 Room and bed information

### **3. Professional Judgment Features**

- ⚕️ Override dangerous items with medical justification
- 📝 Required reason for all overrides
- 📊 Enhanced confirmation dialogs with patient context
- 🔒 All override decisions logged for compliance

### **4. Multi-Patient Management**

- 👥 Cart grouped by patient
- 📦 Bulk validation and ordering
- 📄 Individual patient summaries
- 💰 Per-patient cost tracking

## 📋 **API Endpoints for Nurses:**

| Endpoint                                        | Method | Purpose                             |
| ----------------------------------------------- | ------ | ----------------------------------- |
| `/api/nurse/assigned-patients`                  | GET    | Get patients assigned to nurse      |
| `/api/nurse/patient-dietary/{patientId}`        | GET    | Get patient's dietary info          |
| `/api/nurse/patient-menu/{patientId}`           | GET    | Get validated menu for patient      |
| `/api/nurse/validate-patient-order/{patientId}` | POST   | Validate order for specific patient |
| `/api/nurse/orders`                             | POST   | Place order as nurse for patient    |

## ✅ **Implementation Checklist for Nurses:**

- [ ] Patient selection interface with search
- [ ] Enhanced menu display with patient context
- [ ] Professional override system for dangerous items
- [ ] Multi-patient cart management
- [ ] Bulk order placement
- [ ] Audit logging for compliance
- [ ] Order confirmation with patient details

This approach maintains all safety validations while giving nurses the professional flexibility they need! 🏥👩‍⚕️
