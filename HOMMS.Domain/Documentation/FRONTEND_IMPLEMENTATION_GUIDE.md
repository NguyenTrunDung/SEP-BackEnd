# Frontend Implementation Guide: Patient Food Ordering System

## 🎯 Overview

This guide shows how to implement food ordering for patients with disease-based dietary restrictions. The system validates food choices in real-time and provides visual feedback.

## 🔄 Complete Flow

### 1. **Patient Authentication**

```javascript
// Get patient info and disease categories
const patient = await fetch("/api/patient-dietary", {
  headers: { Authorization: `Bearer ${token}` },
});
// Response includes: patientId, diseaseCategories, isActive
```

### 2. **Load Menu with Validation**

```javascript
// Get menu with pre-validated items
const menu = await fetch("/api/patient-dietary/filtered-menu?date=2024-01-15", {
  headers: { Authorization: `Bearer ${token}` },
});

// Each menu item includes validation:
// {
//   "id": 101,
//   "name": "Cơm gà luộc",
//   "price": 35000,
//   "validationResult": {
//     "level": "Safe|Advisory|Warning|Dangerous",
//     "message": "Detailed message",
//     "recommendations": ["List of suggestions"]
//   }
// }
```

### 3. **Display Items with Visual Indicators**

```jsx
function MenuItemCard({ item }) {
  const validationStyles = {
    Safe: { color: "green", icon: "✅", border: "green" },
    Advisory: { color: "blue", icon: "ℹ️", border: "blue" },
    Warning: { color: "orange", icon: "⚠️", border: "orange" },
    Dangerous: { color: "red", icon: "❌", border: "red" },
  };

  const style = validationStyles[item.validationResult.level];

  return (
    <div
      className="menu-item"
      style={{ borderLeft: `4px solid ${style.border}` }}
    >
      <h3>
        {item.name} <span style={{ color: style.color }}>{style.icon}</span>
      </h3>
      <p>{item.description}</p>
      <div className="validation-info">
        <strong>{item.validationResult.level}</strong>:{" "}
        {item.validationResult.message}
        {item.validationResult.recommendations && (
          <ul>
            {item.validationResult.recommendations.map((rec, idx) => (
              <li key={idx}>{rec}</li>
            ))}
          </ul>
        )}
      </div>
      <button
        onClick={() => handleAddToCart(item)}
        disabled={item.validationResult.level === "Dangerous"}
        className={
          item.validationResult.level === "Warning" ? "warning-btn" : ""
        }
      >
        {item.validationResult.level === "Dangerous"
          ? "Không được phép"
          : "Thêm vào giỏ"}
      </button>
    </div>
  );
}
```

### 4. **Add to Cart with Validation**

```javascript
async function handleAddToCart(item) {
  // For Warning items, show confirmation dialog
  if (item.validationResult.level === "Warning") {
    const confirmed = confirm(
      `${item.validationResult.message}\n\nBạn có muốn tiếp tục?`
    );
    if (!confirmed) return;
  }

  // For Dangerous items, block completely
  if (item.validationResult.level === "Dangerous") {
    alert("Món ăn này không phù hợp với tình trạng sức khỏe của bạn");
    return;
  }

  // Add to cart
  addToCart(item);
}
```

### 5. **Cart Validation**

```javascript
// Validate entire cart before checkout
async function validateCart(cartItems) {
  const response = await fetch("/api/patient-dietary/validate-cart", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      menuItemIds: cartItems.map((item) => item.id),
    }),
  });

  const validation = await response.json();

  // Show nutritional summary
  displayNutritionalSummary(validation.data);

  return validation.data.canProceed;
}

function displayNutritionalSummary(summary) {
  // Display: totalCalories, totalSodium, totalSugar, overallWarnings
  console.log("Tổng calories:", summary.totalCalories);
  console.log("Tổng natri:", summary.totalSodium, "mg");
  console.log("Cảnh báo:", summary.overallWarnings);
}
```

### 6. **Final Order Placement**

```javascript
async function placeOrder(cartItems, deliveryInfo) {
  // Final validation
  const finalValidation = await fetch("/api/patient-dietary/validate-order", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      menuItemIds: cartItems.map((item) => item.id),
      deliveryTime: deliveryInfo.deliveryTime,
    }),
  });

  const validation = await finalValidation.json();

  if (!validation.data.canProceed) {
    alert("Có món ăn không phù hợp. Vui lòng kiểm tra lại giỏ hàng.");
    return;
  }

  // Place order
  const order = await fetch("/api/orders", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      menuItemIds: cartItems.map((item) => ({
        menuItemId: item.id,
        quantity: item.quantity,
      })),
      deliveryTime: deliveryInfo.deliveryTime,
      dietaryValidationId: validation.data.validationId,
    }),
  });

  const orderResult = await order.json();
  return orderResult.data;
}
```

## 🎨 UI/UX Guidelines

### Visual Indicators

| Level         | Color     | Icon | Action               |
| ------------- | --------- | ---- | -------------------- |
| **Safe**      | 🟢 Green  | ✅   | Normal ordering      |
| **Advisory**  | 🔵 Blue   | ℹ️   | Show recommendations |
| **Warning**   | 🟠 Orange | ⚠️   | Require confirmation |
| **Dangerous** | 🔴 Red    | ❌   | Block ordering       |

### CSS Examples

```css
.menu-item {
  border-radius: 8px;
  padding: 16px;
  margin: 8px 0;
  border-left: 4px solid;
}

.validation-info {
  background: #f5f5f5;
  padding: 8px;
  border-radius: 4px;
  margin: 8px 0;
}

.warning-btn {
  background: orange;
  color: white;
}

.disabled-btn {
  background: #ccc;
  cursor: not-allowed;
}
```

## 📱 Mobile Considerations

```javascript
// Touch-friendly confirmation dialogs
function showMobileConfirmation(item) {
  return new Promise((resolve) => {
    const modal = document.createElement("div");
    modal.innerHTML = `
      <div class="modal-overlay">
        <div class="modal-content">
          <h3>⚠️ Cảnh báo dinh dưỡng</h3>
          <p>${item.validationResult.message}</p>
          <ul>
            ${item.validationResult.recommendations
              .map((rec) => `<li>${rec}</li>`)
              .join("")}
          </ul>
          <div class="modal-buttons">
            <button onclick="resolve(false)" class="cancel-btn">Hủy bỏ</button>
            <button onclick="resolve(true)" class="confirm-btn">Vẫn đặt</button>
          </div>
        </div>
      </div>
    `;
    document.body.appendChild(modal);
  });
}
```

## 🔧 Error Handling

```javascript
// Global error handler for dietary issues
function handleDietaryError(error) {
  if (error.code === "DIETARY_RESTRICTION_VIOLATION") {
    showAlert({
      title: "Hạn chế dinh dưỡng",
      message: error.message,
      type: "warning",
      actions: [
        {
          text: "Liên hệ chuyên gia dinh dưỡng",
          action: () => contactNutritionist(),
        },
        { text: "Đóng", action: () => {} },
      ],
    });
  } else {
    showAlert({
      title: "Lỗi",
      message: "Có lỗi xảy ra. Vui lòng thử lại.",
      type: "error",
    });
  }
}
```

## 📊 State Management Example (React)

```javascript
// Using React hooks for cart and validation
function usePatientOrdering() {
  const [cart, setCart] = useState([]);
  const [validationSummary, setValidationSummary] = useState(null);
  const [patientInfo, setPatientInfo] = useState(null);

  const addToCart = useCallback(async (item) => {
    // Validate item first
    const validation = await validateItem(item.id);

    if (validation.level === "Dangerous") {
      throw new Error("Món ăn không phù hợp");
    }

    if (validation.level === "Warning") {
      const confirmed = await showConfirmationDialog(validation);
      if (!confirmed) return;
    }

    setCart((prev) => [...prev, { ...item, validation }]);
  }, []);

  const validateCart = useCallback(async () => {
    if (cart.length === 0) return;

    const summary = await fetch("/api/patient-dietary/validate-cart", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({
        menuItemIds: cart.map((item) => item.id),
      }),
    }).then((r) => r.json());

    setValidationSummary(summary.data);
  }, [cart]);

  useEffect(() => {
    validateCart();
  }, [cart, validateCart]);

  return {
    cart,
    validationSummary,
    patientInfo,
    addToCart,
    removeFromCart: (itemId) =>
      setCart((prev) => prev.filter((item) => item.id !== itemId)),
  };
}
```

## 🚀 Quick Start Checklist

- [ ] Implement patient authentication
- [ ] Fetch patient disease categories
- [ ] Load menu with validation levels
- [ ] Display visual indicators for each validation level
- [ ] Handle add-to-cart with confirmations
- [ ] Implement cart validation
- [ ] Add final order validation
- [ ] Create order confirmation page
- [ ] Test all validation scenarios
- [ ] Add proper error handling

This implementation ensures patients receive appropriate dietary guidance while maintaining a smooth user experience!
