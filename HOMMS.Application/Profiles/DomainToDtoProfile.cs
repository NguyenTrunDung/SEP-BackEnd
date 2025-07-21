using AutoMapper;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Entities;

namespace HOMMS.Application.Profiles
{
    /// <summary>
    /// AutoMapper profile for mapping domain entities to DTOs
    /// </summary>
    public class DomainToDtoProfile : Profile
    {
        public DomainToDtoProfile()
        {
            // Food -> FoodDto
            CreateMap<Food, FoodDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
            CreateMap<FoodDto, Food>();

            // FoodCategory -> FoodCategoryDto
            CreateMap<FoodCategory, FoodCategoryDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.Sort, opt => opt.MapFrom(src => src.Sort ?? 0));
            
            // FoodCategoryDto -> FoodCategory (reverse mapping)
            CreateMap<FoodCategoryDto, FoodCategory>()
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.Sort, opt => opt.MapFrom(src => src.Sort));

            // DiseaseCategory mappings
            CreateMap<DiseaseCategory, DiseaseCategoryDto>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null))
                .ForMember(dest => dest.TotalPatients, opt => opt.Ignore()) // Populated manually in service
                .ForMember(dest => dest.TotalFoodRestrictions, opt => opt.Ignore()); // Populated manually in service
            
            CreateMap<CreateDiseaseCategoryDto, DiseaseCategory>()
                .ForMember(dest => dest.Code, opt => opt.Ignore()) // Code will be set manually in service
                .ForMember(dest => dest.BranchId, opt => opt.Ignore()) // BranchId will be set manually in service
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedBy, opt => opt.Ignore());
            
            CreateMap<UpdateDiseaseCategoryDto, DiseaseCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

            // Menu -> MenuDto
            CreateMap<Menu, MenuDto>();

            // TODO: Not used right now, using manual mapping in service
            // Menu -> MenuDetailViewDto (for the new GetMenuList API)
            CreateMap<Menu, MenuDetailViewDto>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastModifiedAt))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.LastModifiedBy))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.MenuDetails));

            // TODO: Not used right now, using manual mapping in service
            // MenuDetail -> MenuDetailsDto (enhanced mapping)
            CreateMap<MenuDetail, MenuDetailsDto>()
                .ForMember(dest => dest.FoodName, opt => opt.MapFrom(src => src.Food!.Name))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastModifiedAt))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.LastModifiedBy))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Food, opt => opt.MapFrom(src => src.Food));

            // MenuDetail -> MenuDetailDto (existing mapping)
            CreateMap<MenuDetail, MenuDetailDto>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Qty));

            // Branch -> BranchDto
            CreateMap<Branch, BranchDto>();

            // Order -> OrderDto
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDtoV2, Order>();
            CreateMap<Order, CreatePatientOrderDto>();
           
            CreateMap<CreatePatientOrderDto, Order>();
            CreateMap<CreateOrderDetailDto, OrderDetails>();
            CreateMap<UpdateOrderDto, Order>();

            // OrderDetail -> OrderDetailDto
            CreateMap<OrderDetails, OrderDetailsDto>()
                .ForMember(dest => dest.MenuId, opt => opt.MapFrom(src => src.MenuId))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.FoodId, opt => opt.MapFrom(src => src.FoodId))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.Qty))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.FoodName, opt => opt.MapFrom(src => 
                    src.Food != null ? src.Food.Name : 
                    src.Menu != null ? src.Menu.Name : 
                    null))
                .ForMember(dest => dest.MenuName, opt => opt.MapFrom(src => src.Menu != null ? src.Menu.Name : null))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastModifiedAt))
                .ForMember(dest => dest.Food, opt => opt.MapFrom(src => src.Food));
            CreateMap<OrderDetailsDto, OrderDetails>();

            // SystemLog ->  SystemLogDto
            CreateMap<SystemLog, SystemLogDto>();
            CreateMap<AddSystemLogDto, SystemLog>();
            CreateMap<SystemLog, AddSystemLogDto>()
             .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.LastModifiedAt.HasValue ? src.LastModifiedAt.Value : src.CreatedAt));

            // Patient -> PatientDto
            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.DiseaseCategories,
                    opt => opt.MapFrom(src => src.PatientDiseaseCategories));
            CreateMap<PatientDto, Patient>();
            CreateMap<CreatePatientDto, Patient>();
            CreateMap<UpdatePatientDto, Patient>();

            CreateMap<PatientDiseaseCategory, PatientDiseaseCategoryDto>();

            // Area mappings
            CreateMap<Area, AreaDto>();
            CreateMap<CreateAreaDto, Area>();
            CreateMap<UpdateAreaDto, Area>();

            // Location mappings
            CreateMap<Location, LocationDto>()
                .ForMember(dest => dest.Area, opt => opt.Ignore()); // Prevent cycle
            CreateMap<CreateLocationDto, Location>();
            CreateMap<UpdateLocationDto, Location>();

 
            // Feedback mappings
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();


 
            // DiseaseCategoryFoodRestriction mappings
            CreateMap<DiseaseCategoryFoodRestriction, DiseaseCategoryFoodRestrictionDto>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null))
                .ForMember(dest => dest.DiseaseCategoryName, opt => opt.MapFrom(src => src.DiseaseCategory != null ? src.DiseaseCategory.Name : null))
                .ForMember(dest => dest.DiseaseCategoryCode, opt => opt.MapFrom(src => src.DiseaseCategory != null ? src.DiseaseCategory.Code : null))
                .ForMember(dest => dest.FoodName, opt => opt.MapFrom(src => src.Food != null ? src.Food.Name : null))
                .ForMember(dest => dest.FoodPrice, opt => opt.MapFrom(src => src.Food != null ? src.Food.PriceForGuest : null))
                .ForMember(dest => dest.RestrictionLevelName, opt => opt.Ignore()) // Computed property
                .ForMember(dest => dest.RestrictionLevelColor, opt => opt.Ignore()); // Computed property
            
            CreateMap<CreateDiseaseCategoryFoodRestrictionDto, DiseaseCategoryFoodRestriction>();
            
            CreateMap<UpdateDiseaseCategoryFoodRestrictionDto, DiseaseCategoryFoodRestriction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.DiseaseCategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.FoodId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
        }
    }
}