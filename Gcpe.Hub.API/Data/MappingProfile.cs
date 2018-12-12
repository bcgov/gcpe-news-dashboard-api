﻿using System.Linq;
using AutoMapper;
using Gcpe.Hub.Data.Entity;

namespace Gcpe.Hub.API.Data
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Activity, Models.Activity>()
                .ForMember(dest => dest.MinistriesSharedWith, opt => opt.MapFrom(src => src.ActivitySharedWith.Select(sw => sw.Ministry.Key)))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.ActivityCategories.Select(ac => ac.Category.Name)))
                .ReverseMap();

            CreateMap<NewsRelease, Models.NewsRelease>()
                .ReverseMap();

            CreateMap<NewsReleaseLog, Models.NewsReleaseLog>()
                .ReverseMap();

            CreateMap<Message, Models.Message>()
                .ReverseMap();

            CreateMap<SocialMediaPost, Models.SocialMediaPost>()
                .ReverseMap();
        }
    }
}
