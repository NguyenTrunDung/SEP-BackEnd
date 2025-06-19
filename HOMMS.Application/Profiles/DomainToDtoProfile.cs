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

            ///////
            CreateMap<Food, FoodDtoV2>()
               .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image))
               .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
            CreateMap<FoodDtoV2, Food>();
            CreateMap<FoodDtoV2_3, Food>();
            CreateMap<Food, FoodDtoV2_3>()
             .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image))
             .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
            ///////



            // FoodCategory -> FoodCategoryDto
            CreateMap<FoodCategory, FoodCategoryDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.Sort, opt => opt.MapFrom(src => src.Sort ?? 0));

            // Menu -> MenuDto
            CreateMap<Menu, MenuDto>();

            // MenuDetail -> MenuDetailDto
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
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.Qty))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));
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
            CreateMap<Location, LocationDto>();
            CreateMap<CreateLocationDto, Location>();
            CreateMap<UpdateLocationDto, Location>();

        }
    }
} 