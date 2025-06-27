using AutoMapper;
using MindflowAI.Entities.Books;
using MindflowAI.Services.Dtos.Books;

namespace MindflowAI.ObjectMapping;

public class MindflowAIAutoMapperProfile : Profile
{
    public MindflowAIAutoMapperProfile()
    {
        CreateMap<Book, BookDto>();
        CreateMap<CreateUpdateBookDto, Book>();
        CreateMap<BookDto, CreateUpdateBookDto>();
        /* Create your AutoMapper object mappings here */
    }
}