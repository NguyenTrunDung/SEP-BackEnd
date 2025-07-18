# Frontend Flow Guide: Patient Food Ordering Based on Disease Categories

## Overview

This guide explains the complete frontend flow for ordering food meals for patients based on their disease categories in the Hospital Canteen Order Management System (HOMMS).

## Flow Diagram

```
Patient Login → Disease Category Check → Browse Menu → Validate Food Items → Add to Cart → Checkout → Order Confirmation
```

## Step-by-Step Implementation

### Step 1: Patient Authentication & Profile Setup

```javascript
// 1. Patient logs in
const loginResponse = await fetch("/api/auth/login", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  body: JSON.stringify({
    email: "patient@hospital.com",
    password: "password123",
  }),
});

const { data: authData } = await loginResponse.json();
const token = authData.token;
```

### Step 2: Get Patient's Disease Categories

```javascript
// 2. Fetch patient's assigned disease categories
const patientDietaryResponse = await fetch("/api/patient-dietary", {
  headers: { Authorization: `Bearer ${token}` },
});

const { data: patientDietary } = await patientDietaryResponse.json();

// Example response:
// {
//   "data": {
//     "patientId": "12345",
//     "patientName": "Nguyễn Văn A",
//     "diseaseCategories": [
//       {
//         "id": 1,
//         "name": "Tiểu đường",
//         "code": "DIABETES",
//         "description": "Bệnh tiểu đường type 2"
//       },
//       {
//         "id": 2,
//         "name": "Tăng huyết áp",
//         "code": "HYPERTENSION",
//         "description": "Tăng huyết áp nguyên phát"
//       }
//     ],
//     "isActive": true
//   }
// }
```

### Step 3: Browse Available Menu with Dietary Validation

```javascript
// 3. Get today's menu with dietary validation for the patient
const menuResponse = await fetch(
  `/api/patient-dietary/filtered-menu?date=${todayDate}`,
  {
    headers: { Authorization: `Bearer ${token}` },
  }
);

const { data: filteredMenu } = await menuResponse.json();

// Example response:
// {
//   "data": {
//     "menuDate": "2024-01-15",
//     "hospitalId": 1,
//     "categories": [
//       {
//         "name": "Món chính",
//         "items": [
//           {
//             "id": 101,
//             "name": "Cơm gà luộc",
//             "description": "Cơm trắng với thịt gà luộc nhạt",
//             "price": 35000,
//             "validationResult": {
//               "level": "Safe",
//               "message": "An toàn cho bệnh nhân tiểu đường và tăng huyết áp",
//               "recommendations": [
//                 "Món ăn phù hợp với chế độ ăn kiêng"
//               ]
//             }
//           },
//           {
//             "id": 102,
//             "name": "Phở bò",
//             "description": "Phở bò truyền thống",
//             "price": 45000,
//             "validationResult": {
//               "level": "Warning",
//               "message": "Cần cẩn thận với bệnh nhân tăng huyết áp",
//               "recommendations": [
//                 "Hạn chế nước dùng do chứa nhiều natri",
//                 "Ăn ít bánh phở để kiểm soát đường"
//               ]
//             }
//           }
//         ]
//       }
//     ]
//   }
// }
```

### Step 4: Display Menu with Visual Indicators

```javascript
// 4. Frontend component to display menu items with validation levels
function MenuItemCard({ item }) {
  const getValidationStyle = (level) => {
    switch (level) {
      case "Safe":
        return { color: "green", icon: "✓", bg: "#e8f5e8" };
      case "Advisory":
        return { color: "blue", icon: "ℹ", bg: "#e8f4fd" };
      case "Warning":
        return { color: "orange", icon: "⚠", bg: "#fff3cd" };
      case "Dangerous":
        return { color: "red", icon: "✗", bg: "#f8d7da" };
      default:
        return { color: "gray", icon: "?", bg: "#f8f9fa" };
    }
  };

  const validation = item.validationResult;
  const style = getValidationStyle(validation.level);

  return (
    <div
      className="menu-item-card"
      style={{ borderLeft: `4px solid ${style.color}` }}
    >
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
          validation.level === "Dangerous" ? "disabled" : ""
        }`}
        onClick={() => handleAddToCart(item)}
        disabled={validation.level === "Dangerous"}
      >
        {validation.level === "Dangerous" ? "Không được phép" : "Thêm vào giỏ"}
      </button>
    </div>
  );
}
```

### Step 5: Validate Individual Food Items Before Adding to Cart

```javascript
// 5. Validate specific food item before adding to cart
async function handleAddToCart(menuItem) {
  // Show confirmation dialog for Warning items
  if (menuItem.validationResult.level === "Warning") {
    const confirmed = await showConfirmationDialog({
      title: "Cảnh báo dinh dưỡng",
      message: menuItem.validationResult.message,
      recommendations: menuItem.validationResult.recommendations,
      confirmText: "Vẫn thêm vào giỏ",
      cancelText: "Hủy bỏ",
    });

    if (!confirmed) return;
  }

  // Validate the specific item
  const validationResponse = await fetch("/api/patient-dietary/validate-food", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      menuItemId: menuItem.id,
    }),
  });

  const { data: validation } = await validationResponse.json();

  if (validation.level !== "Dangerous") {
    addItemToCart(menuItem, validation);
    showSuccessMessage("Đã thêm vào giỏ hàng");
  } else {
    showErrorMessage(
      "Món ăn này không phù hợp với tình trạng sức khỏe của bạn"
    );
  }
}
```

### Step 6: Shopping Cart with Dietary Summary

```javascript
// 6. Display cart with overall dietary validation
function ShoppingCart({ cartItems }) {
  const [cartValidation, setCartValidation] = useState(null);

  useEffect(() => {
    if (cartItems.length > 0) {
      validateCart();
    }
  }, [cartItems]);

  const validateCart = async () => {
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

    const { data } = await response.json();
    setCartValidation(data);
  };

  return (
    <div className="shopping-cart">
      <h2>Giỏ hàng của bạn</h2>

      {cartValidation && (
        <div className="cart-validation-summary">
          <h3>Tóm tắt dinh dưỡng:</h3>
          <p>
            <strong>Tổng calories:</strong> {cartValidation.totalCalories}
          </p>
          <p>
            <strong>Natri:</strong> {cartValidation.totalSodium}mg
          </p>
          <p>
            <strong>Đường:</strong> {cartValidation.totalSugar}g
          </p>

          {cartValidation.overallWarnings.length > 0 && (
            <div className="overall-warnings">
              <h4>Lưu ý:</h4>
              <ul>
                {cartValidation.overallWarnings.map((warning, idx) => (
                  <li key={idx}>{warning}</li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}

      {cartItems.map((item) => (
        <CartItemRow key={item.id} item={item} />
      ))}

      <button
        className="checkout-btn"
        onClick={handleCheckout}
        disabled={cartValidation?.hasBlockingIssues}
      >
        Đặt hàng ({calculateTotal().toLocaleString("vi-VN")} VNĐ)
      </button>
    </div>
  );
}
```

### Step 7: Final Order Validation and Placement

```javascript
// 7. Final checkout with comprehensive validation
async function handleCheckout() {
  try {
    // Final validation before order placement
    const finalValidationResponse = await fetch(
      "/api/patient-dietary/validate-order",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          menuItemIds: cartItems.map((item) => item.id),
          deliveryTime: selectedDeliveryTime,
          specialInstructions: specialInstructions,
        }),
      }
    );

    const { data: finalValidation } = await finalValidationResponse.json();

    if (finalValidation.canProceed) {
      // Place the order
      const orderResponse = await fetch("/api/orders", {
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
          deliveryTime: selectedDeliveryTime,
          specialInstructions: specialInstructions,
          dietaryValidationId: finalValidation.validationId,
        }),
      });

      const { data: order } = await orderResponse.json();

      // Redirect to order confirmation
      navigate(`/orders/${order.id}/confirmation`);
    } else {
      showErrorDialog({
        title: "Không thể đặt hàng",
        message:
          "Có một số món ăn không phù hợp với tình trạng sức khỏe của bạn",
        details: finalValidation.blockingReasons,
      });
    }
  } catch (error) {
    showErrorMessage("Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại.");
  }
}
```

### Step 8: Order Confirmation with Dietary Information

```javascript
// 8. Order confirmation page with dietary summary
function OrderConfirmation({ orderId }) {
  const [order, setOrder] = useState(null);

  useEffect(() => {
    fetchOrderDetails();
  }, [orderId]);

  const fetchOrderDetails = async () => {
    const response = await fetch(`/api/orders/${orderId}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    const { data } = await response.json();
    setOrder(data);
  };

  return (
    <div className="order-confirmation">
      <h1>Đặt hàng thành công!</h1>
      <p>
        Mã đơn hàng: <strong>{order?.orderNumber}</strong>
      </p>

      <div className="dietary-summary">
        <h2>Thông tin dinh dưỡng</h2>
        <p>Đơn hàng này đã được kiểm tra và phù hợp với:</p>
        <ul>
          {order?.validatedDiseaseCategories.map((category) => (
            <li key={category.id}>{category.name}</li>
          ))}
        </ul>

        {order?.dietaryNotes && (
          <div className="dietary-notes">
            <h3>Lưu ý dinh dưỡng:</h3>
            <ul>
              {order.dietaryNotes.map((note, idx) => (
                <li key={idx}>{note}</li>
              ))}
            </ul>
          </div>
        )}
      </div>

      <div className="order-items">
        <h2>Món ăn đã đặt</h2>
        {order?.items.map((item) => (
          <div key={item.id} className="order-item">
            <span>
              {item.name} x{item.quantity}
            </span>
            <span>
              {(item.price * item.quantity).toLocaleString("vi-VN")} VNĐ
            </span>
          </div>
        ))}
      </div>

      <div className="delivery-info">
        <p>
          <strong>Thời gian giao:</strong> {order?.deliveryTime}
        </p>
        <p>
          <strong>Địa điểm:</strong> {order?.deliveryLocation}
        </p>
      </div>
    </div>
  );
}
```

## Key Frontend Considerations

### 1. Visual Indicators

- **Green (Safe)**: ✓ icon, green border, normal ordering
- **Blue (Advisory)**: ℹ icon, blue border, show recommendations
- **Orange (Warning)**: ⚠ icon, orange border, confirmation dialog required
- **Red (Dangerous)**: ✗ icon, red border, disable ordering button

### 2. User Experience

- Show clear dietary information before ordering
- Provide recommendations for each validation level
- Allow users to proceed with Warning items after confirmation
- Block Dangerous items completely
- Show comprehensive dietary summary in cart

### 3. Error Handling

```javascript
// Global error handler for dietary validation
function handleDietaryError(error, context) {
  if (error.code === "DIETARY_RESTRICTION_VIOLATION") {
    showDietaryErrorDialog({
      title: "Hạn chế dinh dưỡng",
      message: error.message,
      suggestions: error.suggestions,
      contactNutritionist: true,
    });
  } else {
    showGenericErrorMessage(error.message);
  }
}
```

### 4. Offline Support

```javascript
// Cache dietary information for offline use
const cacheDietaryInfo = async (patientId, diseaseCategories) => {
  await localForage.setItem(`dietary_${patientId}`, {
    diseaseCategories,
    cachedAt: new Date().toISOString(),
    expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(),
  });
};
```

## API Endpoints Summary

| Endpoint                              | Method | Purpose                          |
| ------------------------------------- | ------ | -------------------------------- |
| `/api/patient-dietary`                | GET    | Get patient's disease categories |
| `/api/patient-dietary/filtered-menu`  | GET    | Get menu with dietary validation |
| `/api/patient-dietary/validate-food`  | POST   | Validate single food item        |
| `/api/patient-dietary/validate-cart`  | POST   | Validate entire cart             |
| `/api/patient-dietary/validate-order` | POST   | Final order validation           |
| `/api/orders`                         | POST   | Place order with dietary info    |

This flow ensures patients receive appropriate food recommendations based on their medical conditions while maintaining a smooth user experience.
