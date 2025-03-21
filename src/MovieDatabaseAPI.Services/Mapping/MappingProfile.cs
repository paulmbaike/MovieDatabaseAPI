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
            .ForMember(dest => dest.DirectorName, opt =>
                opt.MapFrom(src => src.Director != null ? src.Director.Name : string.Empty))
            .ForMember(dest => dest.Genres, opt =>
                opt.MapFrom(src => src.Genres.Select(g => g.Name)))
            .ForMember(dest => dest.Actors, opt =>
                opt.MapFrom(src => src.Actors.Select(a => a.Name)));

        CreateMap<CreateMovieDto, Movie>();
        CreateMap<UpdateMovieDto, Movie>();

        // Actor mappings
        CreateMap<Actor, ActorDto>();
        CreateMap<CreateActorDto, Actor>();
        CreateMap<UpdateActorDto, Actor>();

        // Director mappings
        CreateMap<Director, DirectorDto>();
        CreateMap<CreateDirectorDto, Director>();
        CreateMap<UpdateDirectorDto, Director>();

        // Genre mappings
        CreateMap<Genre, GenreDto>();
        CreateMap<CreateGenreDto, Genre>();
        CreateMap<UpdateGenreDto, Genre>();
    }
}