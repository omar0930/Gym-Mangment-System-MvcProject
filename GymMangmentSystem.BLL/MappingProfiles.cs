using AutoMapper;
using GymMangmentSystem.BLL.ViewModels.SessionViewModels;
using GymMangmentSystem.BLL.ViewModels.TrainerViewModels;
using GymMangmentSystem.DAL.Data.Models;
using GymMangmentSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymMangmentSystem.BLL
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            MapSession();
        }


        //sessionMapper
        private void MapSession()
        {
            CreateMap<Session, SessionViewModel>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descreption))
                .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.AvailableSlots, opt => opt.Ignore());


            CreateMap<CreateSessionViewModel, Session>()
                .ForMember(dest => dest.Descreption, opt => opt.MapFrom(src => src.Description));

            CreateMap<UpdateSessionViewModel, Session>()
                .ForMember(dest => dest.Descreption, opt => opt.MapFrom(src => src.Description))
                .ReverseMap()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Descreption));

            CreateMap<Trainer, TrainerSelectViewModel>();
            CreateMap<Category, CategorySelectViewModel>();

        }
    }
}
