using AutoMapper;
using MovieDatabaseAPI.Core.DTOs;
using MovieDatabaseAPI.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MovieDatabaseAPI.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Movie mappings
        CreateMap<Movie, MovieDto>()
            .ForMember(dest => dest.DirectorName, opt => opt.MapFrom(src => src.Director != null ? src.Director.Name : string.Empty))
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres.Select(g => g.Name)))
            .ForMember(dest => dest.Actors, opt => opt.MapFrom(src => src.Actors.Select(a => a.Name)));

        // Add mappings for other entities
    }
}