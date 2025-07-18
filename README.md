# ProjectSEP490

Đồ án tốt nghiệp SU25

---

## 3. Quy tắc code & pattern

- **Clean Architecture:** Tách biệt rõ ràng các layer (API, Application, Domain, Infrastructure, Common).
- **Repository Pattern:** Mọi truy vấn DB đều qua repository, không truy cập DbContext trực tiếp ngoài repository.
- **Unit of Work:** Sử dụng nếu cần transaction nhiều repository.
- **Dependency Injection:** Mọi service/repository đều inject qua constructor.
- **SOLID Principles:** Mỗi class chỉ 1 nhiệm vụ, code dễ mở rộng, dễ test.
- **DTO & Mapping:** Controller chỉ nhận/trả về DTO, không expose entity trực tiếp.
- **ApiResponseBase:** Tất cả API trả về theo chuẩn `{ status, message, data }`.
- **Exception Handling:** Dùng middleware để xử lý lỗi toàn cục, không throw lỗi thô ra ngoài.
- **Authorization:** Sử dụng policy-based RBAC, claim-based JWT, không hardcode quyền trong controller.

---

## 4. Quy trình làm việc

### 4.1. Chuẩn bị môi trường

- Cài đặt .NET 8 SDK, SQL Server, Visual Studio/VS Code.
- Clone repo về máy.
- Cấu hình `appsettings.json` (connection string, JWT secret, ...).
- Chạy migration và seed dữ liệu:
  ```bash
  dotnet ef database update --project HOMMS.Infrastructure --startup-project HOMMS.API
  ```

### 4.2. Chạy dự án

```bash
dotnet run --project HOMMS.API
```

- API docs: `/swagger`

### 4.3. Thêm mới tính năng

1. **Xác định layer cần sửa:**

   - Logic nghiệp vụ: Application/Domain
   - Truy vấn DB: Infrastructure/Repositories
   - API: Controllers

2. **Tạo entity mới:**

   - Thêm vào `HOMMS.Domain/Entities/`
   - Cấu hình Fluent API ở `HOMMS.Infrastructure/Configurations/`

3. **Tạo repository:**

   - Interface ở `Repositories/Interfaces/`
   - Implementation ở `Repositories/Implementations/`
   - Đăng ký DI trong `Program.cs`

4. **Tạo service:**

   - Interface ở `Application/Interfaces/`
   - Implementation ở `Application/Implementations/`

5. **Tạo controller:**

   - Đặt ở `HOMMS.API/Controllers/`
   - Chỉ inject service, không inject repository trực tiếp.

6. **Mapping DTO:**

   - Định nghĩa DTO ở `Domain/Dtos/`
   - Mapping bằng AutoMapper (nếu dùng).

7. **Bảo vệ endpoint:**

   - Dùng `[Authorize(Policy = "...")]` theo permission.

8. **Trả về chuẩn:**
   - Luôn trả về `ApiResponseBase<T>`

### 4.4. Seed dữ liệu

- Sửa file seed ở `HOMMS.Infrastructure/Seeds/`
- Chạy lại seed bằng cách khởi động lại app (nếu seed trong startup).

### 4.5. Chạy migration (Tạo/Cập nhật database)

**Tạo migration mới khi có thay đổi entity hoặc cấu hình:**

```bash
dotnet ef migrations add TenMigrationMoi --project HOMMS.Infrastructure --startup-project HOMMS.API
```

- `TenMigrationMoi`: Đặt tên migration theo chức năng bạn vừa thay đổi, ví dụ: `AddOrderEntity`, `UpdateUserRole`, ...

**Áp dụng migration để cập nhật database:**

```bash
dotnet ef database update --project HOMMS.Infrastructure --startup-project HOMMS.API
```

**Lưu ý:**

- Luôn kiểm tra kỹ các file migration được tạo ra trước khi chạy lệnh update.
- Nếu gặp lỗi migration, hãy kiểm tra lại các quan hệ, ràng buộc trong entity/configuration.
- Không xóa migration đã được deploy lên production DB, chỉ rollback hoặc tạo migration mới để sửa.

**Tóm tắt quy trình migration:**

1. Thay đổi entity/configuration.
2. Tạo migration mới.
3. Kiểm tra file migration.
4. Chạy update để cập nhật DB.
5. Kiểm tra lại dữ liệu và các chức năng liên quan.

### 4.6. Quy trình quản lý migration khi làm việc nhóm

**Migration là code, phải quản lý bằng git.**  
Tất cả các file migration (trong thư mục `Migrations/`) đều phải được commit lên git và review như code bình thường.

#### **A. Khi phát triển tính năng mới**

1. **Luôn pull code mới nhất từ nhánh chính (develop) trước khi tạo migration mới:**
   ```bash
   git pull origin develop
   ```
2. **Chỉ tạo migration khi đã hoàn thành thay đổi entity/configuration.**
3. **Kiểm tra kỹ migration được tạo ra (so sánh với các migration trước đó).**
4. **Commit migration cùng với code thay đổi entity/configuration.**
5. **Push lên remote, tạo pull request để review.**

#### **B. Khi có nhiều người cùng thay đổi entity**

- **Luôn tạo migration trên code mới nhất.**
- **Nếu có xung đột migration (hai người cùng tạo migration trên hai nhánh khác nhau):**
  - Merge code, xóa migration cũ trên nhánh của mình (nếu chưa merge lên main).
  - Pull migration mới nhất từ nhánh chính về.
  - Chạy lại migration mới trên code đã merge, tạo migration mới nếu cần.
  - Chỉ giữ lại migration hợp lệ, không để trùng lặp hoặc mất migration.

#### **C. Khi deploy lên production**

- **Chỉ deploy migration đã được review và test kỹ.**
- **Không xóa migration đã deploy lên production.**
- **Nếu cần rollback, tạo migration mới để revert thay vì xóa migration cũ.**

#### **D. Một số lưu ý kỹ thuật**

- **Đặt tên migration rõ ràng, có ý nghĩa.**
- **Không để migration chứa thay đổi không liên quan.**
- **Không để migration sinh ra tự động khi chưa kiểm tra kỹ.**
- **Có thể dùng lệnh `dotnet ef migrations remove` để xóa migration chưa commit nếu phát hiện sai sót.**

#### **E. Checklist cho team**

1. Trước khi tạo migration mới:
   - `git pull origin main`
   - Đảm bảo code và migration local là mới nhất.
2. Sau khi tạo migration:
   - Commit migration cùng code thay đổi.
   - Push lên remote, tạo pull request.
3. Khi review/merge:
   - Nếu có xung đột migration, người merge phải giải quyết bằng cách rebase hoặc tạo migration mới trên code đã merge.
4. Sau khi merge:
   - Các thành viên khác phải pull migration mới nhất về, chạy lại `dotnet ef database update` để đồng bộ DB local.

---

**Lưu ý:**

- Sử dụng CI/CD để kiểm tra migration tự động (chạy lệnh update DB trên môi trường test/staging).
- Không commit file cấu hình nhạy cảm, secret lên git.

---

## 5. Quy tắc commit & review

- **Commit message rõ ràng:** `[Feature]`, `[Fix]`, `[Refactor]`, `[Docs]`, ...
- **Pull Request:**
  - Mô tả rõ thay đổi, ảnh hưởng, cách test.
  - Được review trước khi merge.
- **Không commit file cấu hình nhạy cảm, secret lên git.**

---

## 6. Một số lưu ý

- **Không sửa trực tiếp vào entity Identity của ASP.NET, luôn mở rộng qua ApplicationUser/ApplicationRole.**
- **Không truy cập DbContext ngoài repository.**
- **Không hardcode string permission, luôn dùng constant hoặc enum.**
- **Luôn test kỹ các endpoint có phân quyền.**
- **Đọc kỹ các file seed, migration trước khi deploy lên production.**

---

## 7. Tài liệu tham khảo

- [ASP.NET Core Docs](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)
- [Repository Pattern](https://deviq.com/design-patterns/repository-pattern)
- [JWT Authentication](https://jwt.io/introduction/)

---

## 8. Liên hệ & hỗ trợ

- Nếu gặp vấn đề, hãy tạo issue trên GitHub hoặc liên hệ leader.
- Đọc kỹ README trước khi hỏi để tiết kiệm thời gian cho cả team.

---

Chúc các bạn code vui vẻ và hiệu quả!
