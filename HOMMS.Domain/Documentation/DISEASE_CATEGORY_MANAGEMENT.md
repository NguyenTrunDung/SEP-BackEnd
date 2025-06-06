# Disease Category Management System for Hospital Canteen Orders

# Hệ Thống Quản Lý Danh Mục Bệnh Lý Cho Đặt Món Ăn Căng Tin Bệnh Viện

## Overview / Tổng Quan

This document outlines the disease category management system that has been integrated into your Hospital Canteen Order Food Management System (HOMMS). This system enables hospitals to manage patient dietary requirements based on their medical conditions, ensuring food safety and adherence to medical dietary restrictions.

**Tiếng Việt**: Tài liệu này mô tả hệ thống quản lý danh mục bệnh lý đã được tích hợp vào Hệ Thống Quản Lý Đặt Món Ăn Căng Tin Bệnh Viện (HOMMS). Hệ thống này cho phép các bệnh viện quản lý yêu cầu dinh dưỡng của bệnh nhân dựa trên tình trạng sức khỏe, đảm bảo an toàn thực phẩm và tuân thủ các hạn chế dinh dưỡng y tế.

## System Components / Các Thành Phần Hệ Thống

### 1. Core Entities / Các Thực Thể Chính

#### DiseaseCategory / Danh Mục Bệnh Lý

- **Purpose / Mục đích**: Defines medical conditions with specific dietary requirements / Định nghĩa các bệnh lý với yêu cầu dinh dưỡng cụ thể
- **Key Properties / Thuộc tính chính**:
  - `Name`: Disease category name / Tên danh mục bệnh lý (e.g., "Diabetes / Đái tháo đường", "Hypertension / Tăng huyết áp", "Cardiac Diet / Chế độ ăn tim mạch")
  - `Code`: Short identifier for quick reference / Mã định danh ngắn (e.g., "DM", "THA", "TIM")
  - `SeverityLevel`: 1=Low/Nhẹ, 2=Medium/Vừa, 3=High/Nặng, 4=Critical/Nguy kịch
  - `RequiresApproval`: Whether orders need physician approval / Có cần phê duyệt của bác sĩ không
  - `DietaryRestrictions`: Text description of foods to avoid / Mô tả các thực phẩm cần tránh
  - `RecommendedFoods`: Text description of recommended foods / Mô tả các thực phẩm khuyến khích
  - `ColorCode`: UI display color for visual categorization / Màu sắc hiển thị để phân loại trực quan

**Ví dụ thực tế tại Việt Nam**:

- `Đái tháo đường type 2` - Code: "DM2"
- `Tăng huyết áp` - Code: "THA"
- `Suy tim` - Code: "SUTIM"
- `Suy thận mãn tính` - Code: "SUTHAN"
- `Xơ gan` - Code: "XOGAN"
- `Viêm dạ dày` - Code: "VDDAY"

#### PatientDiseaseCategory / Bệnh Nhân - Danh Mục Bệnh Lý

- **Purpose / Mục đích**: Links patients to their assigned disease categories / Liên kết bệnh nhân với các danh mục bệnh lý được chỉ định
- **Key Properties / Thuộc tính chính**:
  - `PatientId`: Reference to ApplicationUser (patient) / Tham chiếu đến bệnh nhân
  - `DiseaseCategoryId`: Reference to DiseaseCategory / Tham chiếu đến danh mục bệnh lý
  - `DiagnosedDate`: When the condition was diagnosed / Ngày chẩn đoán bệnh
  - `PatientSeverityLevel`: Patient-specific severity override / Mức độ nghiêm trọng riêng của bệnh nhân
  - `AssignedByPhysician`: Doctor who assigned the category / Bác sĩ chỉ định danh mục
  - `ExpiryDate`: When assignment expires (for temporary conditions) / Ngày hết hạn (cho các bệnh tạm thời)

**Quy trình thực tế**:

1. Bệnh nhân nhập viện → Khám lâm sàng
2. Bác sĩ chỉ định danh mục bệnh lý trong hệ thống
3. Hệ thống tự động áp dụng chế độ dinh dưỡng phù hợp
4. Theo dõi và điều chỉnh theo tiến triển bệnh

#### DiseaseCategoryFoodRestriction / Hạn Chế Thực Phẩm Theo Bệnh Lý

- **Purpose / Mục đích**: Defines specific food restrictions for each disease category / Định nghĩa các hạn chế thực phẩm cụ thể cho từng danh mục bệnh lý
- **Key Properties / Thuộc tính chính**:
  - `DiseaseCategoryId`: Reference to DiseaseCategory / Tham chiếu đến danh mục bệnh lý
  - `FoodId`: Reference to Food item / Tham chiếu đến món ăn
  - `RestrictionLevel`: 1=Advisory/Khuyến cáo, 2=Warning/Cảnh báo, 3=Prohibited/Cấm, 4=Dangerous/Nguy hiểm
  - `Reason`: Medical reason for the restriction / Lý do y tế cho việc hạn chế
  - `AlternativeRecommendations`: Suggested alternatives / Các món thay thế được đề xuất

**Ví dụ hạn chế thực phẩm**:

- **Đái tháo đường**: Bánh ngọt, nước ngọt có ga → Thay bằng: Trái cây ít đường, nước lọc
- **Tăng huyết áp**: Đồ ăn mặn, dưa muối → Thay bằng: Rau luộc, canh nhạt
- **Suy thận**: Thịt đỏ, chuối → Thay bằng: Cá nước ngọt, táo

### 2. Enhanced Entities / Các Thực Thể Được Cải Tiến

#### ApplicationUser (Enhanced) / Người Dùng Ứng Dụng (Cải Tiến)

- Added patient-specific properties / Đã thêm các thuộc tính riêng cho bệnh nhân:
  - `RoomNumber`, `BedNumber`: Hospital location tracking / Theo dõi vị trí tại bệnh viện (Phòng, Giường)
  - `AdmissionDate`, `DischargeDate`: Stay duration tracking / Theo dõi thời gian nằm viện
  - `AttendingPhysician`: Responsible doctor / Bác sĩ điều trị chính
  - `RequiresDietarySupervision`: Flag for special dietary monitoring / Cờ đánh dấu cần giám sát dinh dưỡng đặc biệt

#### Food (Enhanced) / Thực Phẩm (Cải Tiến)

- Added disease category relationships / Đã thêm mối quan hệ với danh mục bệnh lý:
  - `DiseaseCategory`: Optional association with specific medical condition / Liên kết tùy chọn với bệnh lý cụ thể
  - `FoodRestrictions`: Collection of disease-based restrictions / Bộ sưu tập các hạn chế dựa trên bệnh lý

## Key Features / Tính Năng Chính

### 1. Multi-Level Dietary Management / Quản Lý Dinh Dưỡng Đa Cấp

- **Branch-Level / Cấp Chi Nhánh**: Each hospital can have its own disease categories / Mỗi bệnh viện có thể có danh mục bệnh lý riêng
- **Category-Level / Cấp Danh Mục**: Define general dietary guidelines per condition / Định nghĩa hướng dẫn dinh dưỡng chung cho từng bệnh
- **Patient-Level / Cấp Bệnh Nhân**: Individual severity and specific notes / Mức độ nghiêm trọng và ghi chú cụ thể cho từng cá nhân
- **Food-Level / Cấp Thực Phẩm**: Specific restrictions per food item / Hạn chế cụ thể cho từng món ăn

### 2. Severity-Based Controls / Kiểm Soát Dựa Trên Mức Độ Nghiêm Trọng

- **Low (1) / Nhẹ (1)**: Advisory guidelines only / Chỉ là hướng dẫn khuyến cáo
- **Medium (2) / Vừa (2)**: Should follow recommendations / Nên tuân theo khuyến nghị
- **High (3) / Nặng (3)**: Strict enforcement required / Yêu cầu tuân thủ nghiêm ngặt
- **Critical (4) / Nguy kịch (4)**: Life-threatening if violated / Có thể đe dọa tính mạng nếu vi phạm

### 3. Restriction Granularity / Mức Độ Chi Tiết Hạn Chế

- **Advisory / Khuyến cáo**: Not recommended but allowed / Không khuyến khích nhưng vẫn cho phép
- **Warning / Cảnh báo**: Should avoid but not blocked / Nên tránh nhưng không chặn
- **Prohibited / Cấm**: Not allowed to order / Không được phép đặt
- **Dangerous / Nguy hiểm**: Could cause serious complications / Có thể gây biến chứng nghiêm trọng

### 4. Physician Integration / Tích Hợp Với Bác Sĩ

- **Assignment Tracking / Theo Dõi Chỉ Định**: Record who assigned each disease category / Ghi nhận ai đã chỉ định từng danh mục bệnh lý
- **Approval Requirements / Yêu Cầu Phê Duyệt**: Flag orders that need physician approval / Đánh dấu đơn hàng cần phê duyệt của bác sĩ
- **Override Capabilities / Khả Năng Ghi Đè**: Allow physician overrides for restricted foods / Cho phép bác sĩ ghi đè các hạn chế thực phẩm

## Business Use Cases / Các Trường Hợp Sử Dụng Thực Tế

### 1. Patient Admission Process / Quy Trình Nhập Viện Bệnh Nhân

```
1. Patient admitted to hospital / Bệnh nhân nhập viện
2. Doctor reviews medical history / Bác sĩ xem xét tiền sử bệnh
3. Assign relevant disease categories / Chỉ định các danh mục bệnh lý liên quan
4. System automatically applies dietary restrictions / Hệ thống tự động áp dụng hạn chế dinh dưỡng
5. Patient can only order approved foods / Bệnh nhân chỉ có thể đặt các món ăn được phép
```

**Ví dụ cụ thể tại Việt Nam**:

- Bệnh nhân A, 55 tuổi, chẩn đoán: Đái tháo đường type 2 + Tăng huyết áp
- Bác sĩ chỉ định 2 danh mục bệnh lý trong hệ thống
- Hệ thống tự động loại bỏ: Cơm rang, nước ngọt, đồ chiên
- Hiển thị thực đơn phù hợp: Cơm gạo lứt, rau luộc, cá hấp

### 2. Food Ordering Workflow / Quy Trình Đặt Món Ăn

```
1. Patient/staff accesses menu / Bệnh nhân/nhân viên truy cập thực đơn
2. System filters foods based on patient's disease categories / Hệ thống lọc thực phẩm dựa trên bệnh lý
3. Warns about advisory/warning level foods / Cảnh báo về thực phẩm mức khuyến cáo/cảnh báo
4. Blocks prohibited/dangerous foods / Chặn thực phẩm bị cấm/nguy hiểm
5. Escalates to physician if approval required / Chuyển lên bác sĩ nếu cần phê duyệt
```

**Giao diện người dùng thực tế**:

- 🟢 **Xanh**: An toàn - "Cháo gà" (phù hợp cho tất cả bệnh nhân)
- 🟡 **Vàng**: Cần lưu ý - "Cơm trắng" (khuyến cáo hạn chế với bệnh nhân đái tháo đường)
- 🟠 **Cam**: Cảnh báo - "Thịt nướng" (nhiều muối, không tốt cho tim mạch)
- 🔴 **Đỏ**: Nguy hiểm - "Bánh kem" (cấm với bệnh nhân đái tháo đường)

### 3. Nutritionist Management / Quản Lý Dinh Dưỡng Viên

```
1. Nutritionist defines food restrictions per disease category / Dinh dưỡng viên định nghĩa hạn chế thực phẩm
2. Sets restriction levels and reasons / Đặt mức độ hạn chế và lý do
3. Provides alternative recommendations / Cung cấp khuyến nghị thay thế
4. Updates restrictions based on medical guidelines / Cập nhật hạn chế dựa trên hướng dẫn y tế
```

**Ví dụ công việc dinh dưỡng viên**:

- Xem xét thực đơn mới: "Bún bò Huế"
- Phân tích: Nước dùng mặn, dầu mỡ nhiều
- Đánh giá: Mức cảnh báo cho bệnh nhân tăng huyết áp
- Đề xuất thay thế: "Bún chả cá" với nước dùng nhạt

## Implementation Benefits / Lợi Ích Triển Khai

### 1. Patient Safety / An Toàn Bệnh Nhân

- **Automatic Filtering / Lọc Tự Động**: Prevents ordering of harmful foods / Ngăn chặn đặt món có hại cho sức khỏe
- **Medical Compliance / Tuân Thủ Y Tế**: Ensures adherence to dietary restrictions / Đảm bảo tuân thủ hạn chế dinh dưỡng
- **Real-time Alerts / Cảnh Báo Thời Gian Thực**: Warns about potential issues / Cảnh báo về các vấn đề tiềm ẩn

### 2. Staff Efficiency / Hiệu Quả Nhân Viên

- **Automated Checks / Kiểm Tra Tự Động**: Reduces manual verification / Giảm việc xác minh thủ công
- **Clear Guidelines / Hướng Dẫn Rõ Ràng**: Provides specific restriction reasons / Cung cấp lý do hạn chế cụ thể
- **Alternative Suggestions / Gợi Ý Thay Thế**: Helps staff recommend suitable options / Giúp nhân viên đề xuất các lựa chọn phù hợp

### 3. Hospital Administration / Quản Lý Bệnh Viện

- **Audit Trail / Dấu Vết Kiểm Toán**: Complete tracking of dietary assignments / Theo dõi hoàn chỉnh việc chỉ định chế độ ăn
- **Compliance Reporting / Báo Cáo Tuân Thủ**: Monitor adherence to medical guidelines / Giám sát việc tuân thủ hướng dẫn y tế
- **Multi-Branch Support / Hỗ Trợ Đa Chi Nhánh**: Each hospital manages its own categories / Mỗi bệnh viện quản lý danh mục riêng

### 4. Scalability / Khả Năng Mở Rộng

- **Flexible Categories / Danh Mục Linh Hoạt**: Easy to add new medical conditions / Dễ dàng thêm các bệnh lý mới
- **Granular Control / Kiểm Soát Chi Tiết**: Fine-tune restrictions per food item / Điều chỉnh hạn chế chi tiết cho từng món ăn
- **Temporal Management / Quản Lý Theo Thời Gian**: Handle temporary conditions with expiry dates / Xử lý các bệnh tạm thời với ngày hết hạn

## Vietnamese Hospital Context / Bối Cảnh Bệnh Viện Việt Nam

### Common Disease Categories in Vietnam / Các Danh Mục Bệnh Lý Phổ Biến Tại Việt Nam

1. **Đái tháo đường (Diabetes)** - Mã: DM

   - Hạn chế: Đồ ngọt, bánh kẹo, nước ngọt
   - Khuyến khích: Rau xanh, cá, thịt nạc

2. **Tăng huyết áp (Hypertension)** - Mã: THA

   - Hạn chế: Đồ mặn, dưa muối, thức ăn chế biến sẵn
   - Khuyến khích: Rau củ, trái cây, cá nước ngọt

3. **Bệnh tim mạch (Cardiovascular Disease)** - Mã: TIM

   - Hạn chế: Đồ chiên, thịt béo, dầu mỡ
   - Khuyến khích: Cá hồi, yến mạch, hạt óc chó

4. **Suy thận (Kidney Disease)** - Mã: THAN

   - Hạn chế: Protein cao, thực phẩm giàu kali
   - Khuyến khích: Cơm, rau ít kali, táo

5. **Viêm gan (Hepatitis)** - Mã: GAN
   - Hạn chế: Rượu bia, thức ăn cay, đồ chiên
   - Khuyến khích: Súp lơ, cà rốt, gà luộc

### Traditional Vietnamese Food Considerations / Cân Nhắc Về Món Ăn Truyền Thống Việt Nam

**Món ăn cần lưu ý**:

- **Phở**: Nước dùng mặn (cảnh báo cho THA), thịt bò (hạn chế cho suy thận)
- **Bún bò Huế**: Rất mặn và cay (cấm cho THA, viêm dạ dày)
- **Cơm tấm**: Thịt nướng nhiều dầu (cảnh báo cho tim mạch)
- **Chè**: Đường nhiều (cấm cho đái tháo đường)

**Món ăn khuyến khích**:

- **Cháo**: Dễ tiêu hóa, phù hợp đa số bệnh lý
- **Canh rau**: Ít muối, nhiều vitamin
- **Cá hấp**: Protein cao, ít béo
- **Rau luộc**: An toàn cho hầu hết bệnh nhân

## User Interface Examples / Ví Dụ Giao Diện Người Dùng

### Menu Display for Diabetic Patient / Hiển Thị Thực Đơn Cho Bệnh Nhân Đái Tháo Đường

```
🟢 Cháo gà - 45,000 VNĐ ✓ An toàn
🟡 Cơm trắng - 25,000 VNĐ ⚠️ Nên hạn chế, thay bằng cơm gạo lứt
🟠 Bánh mì - 20,000 VNĐ ⚠️ Chứa đường, không khuyến khích
🔴 Chè đậu xanh - 15,000 VNĐ ❌ CẤM - Chứa nhiều đường

Gợi ý thay thế:
- Thay cơm trắng → Cơm gạo lứt
- Thay bánh mì → Bánh mì nguyên cám
- Thay chè → Trái cây tươi
```

### Warning Messages / Thông Báo Cảnh Báo

```
⚠️ CẢNH BÁO DINH DƯỠNG
Bệnh nhân có chẩn đoán: Đái tháo đường Type 2
Món "Cơm rang thập cẩm" không phù hợp vì:
- Chứa nhiều dầu mỡ
- Có thể làm tăng đường huyết

Bạn có muốn:
1. ✅ Chọn món thay thế: Cơm gạo lứt với rau luộc
2. 📞 Liên hệ bác sĩ để xin phép đặc biệt
3. ❌ Hủy món này
```

## Technical Implementation Notes / Ghi Chú Triển Khai Kỹ Thuật

### Database Localization / Bản Địa Hóa Cơ Sở Dữ Liệu

```sql
-- Thêm cột hỗ trợ tiếng Việt
ALTER TABLE DiseaseCategories
ADD NameVi NVARCHAR(100),
    DescriptionVi NVARCHAR(1000),
    DietaryRestrictionsVi NVARCHAR(1000);

-- Thêm cột cho tên món ăn tiếng Việt
ALTER TABLE Foods
ADD NameVi NVARCHAR(255),
    DescriptionVi NVARCHAR(500);
```

### Sample Data Seeding / Dữ Liệu Mẫu

```csharp
// Seed data cho bệnh viện Việt Nam
new DiseaseCategory
{
    Name = "Diabetes Type 2",
    NameVi = "Đái tháo đường type 2",
    Code = "DM2",
    DescriptionVi = "Bệnh nhân cần hạn chế đường và tinh bột",
    DietaryRestrictionsVi = "Tránh đồ ngọt, nước ngọt, bánh kẹo, cơm trắng",
    RecommendedFoodsVi = "Ăn nhiều rau xanh, cá, thịt nạc, cơm gạo lứt"
}
```

## Migration Strategy / Chiến Lược Di Chuyển

### 1. Data Migration / Di Chuyển Dữ Liệu

1. Create new tables with proper constraints / Tạo bảng mới với ràng buộc phù hợp
2. Migrate existing food-disease relationships / Di chuyển mối quan hệ thức ăn-bệnh lý hiện có
3. Set up default disease categories per branch / Thiết lập danh mục bệnh lý mặc định cho từng chi nhánh
4. Assign existing patients to appropriate categories / Gán bệnh nhân hiện có vào danh mục phù hợp

### 2. Deployment Approach / Phương Pháp Triển Khai

1. Deploy database schema changes / Triển khai thay đổi lược đồ cơ sở dữ liệu
2. Seed initial disease category data / Khởi tạo dữ liệu danh mục bệnh lý ban đầu
3. Update application services / Cập nhật các dịch vụ ứng dụng
4. Train hospital staff on new features / Đào tạo nhân viên bệnh viện về tính năng mới
5. Monitor system performance and compliance / Giám sát hiệu suất hệ thống và tuân thủ

### Training Materials for Vietnamese Staff / Tài Liệu Đào Tạo Cho Nhân Viên Việt Nam

#### For Doctors / Dành cho Bác sĩ

- Cách chỉ định danh mục bệnh lý cho bệnh nhân
- Quy trình phê duyệt đơn hàng đặc biệt
- Cách ghi đè hạn chế khi cần thiết

#### For Nutritionists / Dành cho Dinh dưỡng viên

- Cách thiết lập hạn chế thực phẩm
- Đề xuất món thay thế phù hợp
- Cập nhật hướng dẫn dinh dưỡng

#### For Canteen Staff / Dành cho Nhân viên căng tin

- Hiểu các biểu tượng màu sắc
- Xử lý đơn hàng bị từ chối
- Tư vấn món ăn cho bệnh nhân

This disease category management system transforms your hospital canteen ordering system into a comprehensive, medically-aware food ordering platform that prioritizes patient safety while maintaining operational efficiency.

**Hệ thống quản lý danh mục bệnh lý này biến hệ thống đặt món ăn căng tin bệnh viện của bạn thành một nền tảng đặt món ăn toàn diện, có nhận thức y tế, ưu tiên an toàn bệnh nhân đồng thời duy trì hiệu quả hoạt động.**
