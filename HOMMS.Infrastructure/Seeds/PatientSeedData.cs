using HOMMS.Domain.Entities;
using HOMMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HOMMS.Infrastructure.Seeds
{
    /// <summary>
    /// Seed data for Patient entities
    /// </summary>
    public static class PatientSeedData
    {
        /// <summary>
        /// Seeds sample patient data for testing
        /// </summary>
        /// <param name="services">Service provider</param>
        public static async Task SeedPatientsAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("PatientSeeding");
            {
                var patients = new List<Patient>
            {
                // Bệnh viện Cần Thơ - Active patients
                new Patient
                {
                    Id = "CTH-P001",
                    BranchId = 1,
                    MedicalRecordNumber = "BN001-2024",
                    FullName = "Nguyễn Văn An",
                    DateOfBirth = new DateTime(1980, 5, 15),
                    Gender = "Nam",
                    RoomNumber = "101",
                    BedNumber = "A",
                    AdmissionDate = DateTime.UtcNow.AddDays(-5),
                    AttendingPhysician = "BS. Trần Thị Hoa",
                    RequiresDietarySupervision = true,
                    IsActive = true,
                    ExternalSystemId = "HMS-CTH-001",
                    LastSyncAt = DateTime.UtcNow,
                    Notes = "Bệnh nhân tiểu đường, cần kiểm soát đường huyết",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Patient
                {
                    Id = "CTH-P002",
                    BranchId = 1,
                    MedicalRecordNumber = "BN002-2024",
                    FullName = "Lê Thị Bình",
                    DateOfBirth = new DateTime(1975, 8, 22),
                    Gender = "Nữ",
                    RoomNumber = "102",
                    BedNumber = "B",
                    AdmissionDate = DateTime.UtcNow.AddDays(-3),
                    AttendingPhysician = "BS. Nguyễn Minh Đức",
                    RequiresDietarySupervision = true,
                    IsActive = true,
                    ExternalSystemId = "HMS-CTH-002",
                    LastSyncAt = DateTime.UtcNow,
                    Notes = "Cao huyết áp, hạn chế muối",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Patient
                {
                    Id = "CTH-P003",
                    BranchId = 1,
                    MedicalRecordNumber = "BN003-2024",
                    FullName = "Phạm Minh Châu",
                    DateOfBirth = new DateTime(1965, 12, 8),
                    Gender = "Nam",
                    RoomNumber = "103",
                    BedNumber = "A",
                    AdmissionDate = DateTime.UtcNow.AddDays(-7),
                    AttendingPhysician = "BS. Võ Thị Mai",
                    RequiresDietarySupervision = false,
                    IsActive = true,
                    ExternalSystemId = "HMS-CTH-003",
                    LastSyncAt = DateTime.UtcNow,
                    Notes = "Phẫu thuật ruột thừa, hồi phục tốt",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Patient
                {
                    Id = "CTH-P004",
                    BranchId = 1,
                    MedicalRecordNumber = "BN004-2024",
                    FullName = "Trần Thị Dung",
                    DateOfBirth = new DateTime(1990, 3, 17),
                    Gender = "Nữ",
                    RoomNumber = "201",
                    BedNumber = "C",
                    AdmissionDate = DateTime.UtcNow.AddDays(-2),
                    AttendingPhysician = "BS. Lê Văn Thành",
                    RequiresDietarySupervision = true,
                    IsActive = true,
                    ExternalSystemId = "HMS-CTH-004",
                    LastSyncAt = DateTime.UtcNow,
                    Notes = "Dị ứng thức ăn biển, tránh tôm cua",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                
                // Discharged patient
                new Patient
                {
                    Id = "CTH-P005",
                    BranchId = 1,
                    MedicalRecordNumber = "BN005-2024",
                    FullName = "Hoàng Văn Em",
                    DateOfBirth = new DateTime(1985, 7, 10),
                    Gender = "Nam",
                    RoomNumber = null,
                    BedNumber = null,
                    AdmissionDate = DateTime.UtcNow.AddDays(-10),
                    DischargeDate = DateTime.UtcNow.AddDays(-1),
                    AttendingPhysician = "BS. Đặng Thị Lan",
                    RequiresDietarySupervision = false,
                    IsActive = false,
                    ExternalSystemId = "HMS-CTH-005",
                    LastSyncAt = DateTime.UtcNow,
                    Notes = "Ra viện khỏe mạnh",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    CreatedBy = "System"
                },
                
                // If there's a second branch (Hospital Chợ Rẫy example)
                new Patient
                {
                    Id = "SGH-P001",
                    BranchId = 2,
                    MedicalRecordNumber = "BN001-CR-2024",
                    FullName = "Nguyễn Thị Phương",
                    DateOfBirth = new DateTime(1978, 11, 25),
                    Gender = "Nữ",
                    RoomNumber = "301",
                    BedNumber = "A",
                    AdmissionDate = DateTime.UtcNow.AddDays(-4),
                    AttendingPhysician = "PGS.TS. Lương Văn Khải",
                    RequiresDietarySupervision = true,
                    IsActive = true,
                    ExternalSystemId = "HMS-SGH-001",
                    LastSyncAt = DateTime.UtcNow,
                    Notes = "Bệnh thận mạn, hạn chế protein",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };

                try
                {
                    await context.Patients.AddRangeAsync(patients);
                    await context.SaveChangesAsync();
                    logger.LogInformation($"Successfully seeded {patients.Count} patients.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while seeding patients.");
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Seeds sample patient data for testing using ModelBuilder (for migrations)
        /// </summary>
        /// <param name="modelBuilder">Model builder</param>
        //public static void SeedPatients(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Patient>().HasData(
        //        // Bệnh viện Cần Thơ - Active patients
        //        new Patient
        //        {
        //            Id = "CTH-P001",
        //            BranchId = 1,
        //            MedicalRecordNumber = "BN001-2024",
        //            FullName = "Nguyễn Văn An",
        //            DateOfBirth = new DateTime(1980, 5, 15),
        //            Gender = "Nam",
        //            RoomNumber = "101",
        //            BedNumber = "A",
        //            AdmissionDate = DateTime.UtcNow.AddDays(-5),
        //            AttendingPhysician = "BS. Trần Thị Hoa",
        //            RequiresDietarySupervision = true,
        //            IsActive = true,
        //            ExternalSystemId = "HMS-CTH-001",
        //            LastSyncAt = DateTime.UtcNow,
        //            Notes = "Bệnh nhân tiểu đường, cần kiểm soát đường huyết",
        //            CreatedAt = DateTime.UtcNow,
        //            CreatedBy = "System"
        //        },
        //        new Patient
        //        {
        //            Id = "CTH-P002", 
        //            BranchId = 1,
        //            MedicalRecordNumber = "BN002-2024",
        //            FullName = "Lê Thị Bình",
        //            DateOfBirth = new DateTime(1975, 8, 22),
        //            Gender = "Nữ",
        //            RoomNumber = "102",
        //            BedNumber = "B",
        //            AdmissionDate = DateTime.UtcNow.AddDays(-3),
        //            AttendingPhysician = "BS. Nguyễn Minh Đức",
        //            RequiresDietarySupervision = true,
        //            IsActive = true,
        //            ExternalSystemId = "HMS-CTH-002",
        //            LastSyncAt = DateTime.UtcNow,
        //            Notes = "Cao huyết áp, hạn chế muối",
        //            CreatedAt = DateTime.UtcNow,
        //            CreatedBy = "System"
        //        },
        //        new Patient
        //        {
        //            Id = "CTH-P003",
        //            BranchId = 1,
        //            MedicalRecordNumber = "BN003-2024",
        //            FullName = "Phạm Minh Châu",
        //            DateOfBirth = new DateTime(1965, 12, 8),
        //            Gender = "Nam",
        //            RoomNumber = "103",
        //            BedNumber = "A",
        //            AdmissionDate = DateTime.UtcNow.AddDays(-7),
        //            AttendingPhysician = "BS. Võ Thị Mai",
        //            RequiresDietarySupervision = false,
        //            IsActive = true,
        //            ExternalSystemId = "HMS-CTH-003",
        //            LastSyncAt = DateTime.UtcNow,
        //            Notes = "Phẫu thuật ruột thừa, hồi phục tốt",
        //            CreatedAt = DateTime.UtcNow,
        //            CreatedBy = "System"
        //        },
        //        new Patient
        //        {
        //            Id = "CTH-P004",
        //            BranchId = 1,
        //            MedicalRecordNumber = "BN004-2024",
        //            FullName = "Trần Thị Dung",
        //            DateOfBirth = new DateTime(1990, 3, 17),
        //            Gender = "Nữ",
        //            RoomNumber = "201",
        //            BedNumber = "C",
        //            AdmissionDate = DateTime.UtcNow.AddDays(-2),
        //            AttendingPhysician = "BS. Lê Văn Thành",
        //            RequiresDietarySupervision = true,
        //            IsActive = true,
        //            ExternalSystemId = "HMS-CTH-004",
        //            LastSyncAt = DateTime.UtcNow,
        //            Notes = "Dị ứng thức ăn biển, tránh tôm cua",
        //            CreatedAt = DateTime.UtcNow,
        //            CreatedBy = "System"
        //        },
                
        //        // Discharged patient
        //        new Patient
        //        {
        //            Id = "CTH-P005",
        //            BranchId = 1,
        //            MedicalRecordNumber = "BN005-2024",
        //            FullName = "Hoàng Văn Em",
        //            DateOfBirth = new DateTime(1985, 7, 10),
        //            Gender = "Nam",
        //            RoomNumber = null,
        //            BedNumber = null,
        //            AdmissionDate = DateTime.UtcNow.AddDays(-10),
        //            DischargeDate = DateTime.UtcNow.AddDays(-1),
        //            AttendingPhysician = "BS. Đặng Thị Lan",
        //            RequiresDietarySupervision = false,
        //            IsActive = false,
        //            ExternalSystemId = "HMS-CTH-005",
        //            LastSyncAt = DateTime.UtcNow,
        //            Notes = "Ra viện khỏe mạnh",
        //            CreatedAt = DateTime.UtcNow.AddDays(-10),
        //            CreatedBy = "System"
        //        },
                
        //        // If there's a second branch (Hospital Chợ Rẫy example)
        //        new Patient
        //        {
        //            Id = "SGH-P001",
        //            BranchId = 2,
        //            MedicalRecordNumber = "BN001-CR-2024",
        //            FullName = "Nguyễn Thị Phương",
        //            DateOfBirth = new DateTime(1978, 11, 25),
        //            Gender = "Nữ",
        //            RoomNumber = "301",
        //            BedNumber = "A",
        //            AdmissionDate = DateTime.UtcNow.AddDays(-4),
        //            AttendingPhysician = "PGS.TS. Lương Văn Khải",
        //            RequiresDietarySupervision = true,
        //            IsActive = true,
        //            ExternalSystemId = "HMS-SGH-001",
        //            LastSyncAt = DateTime.UtcNow,
        //            Notes = "Bệnh thận mạn, hạn chế protein",
        //            CreatedAt = DateTime.UtcNow,
        //            CreatedBy = "System"
        //        }
        //    );
        //}
    }
} 