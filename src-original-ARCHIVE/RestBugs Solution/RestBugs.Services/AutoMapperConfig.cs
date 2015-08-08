using AutoMapper;
using RestBugs.Services.Model;

namespace RestBugs.Services
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            var bug2Dto = Mapper.CreateMap<Bug, BugDTO>();
            bug2Dto.ForMember(dto => dto.Title, e => e.MapFrom(b => b.Name));

            var dto2Bug = Mapper.CreateMap<BugDTO, Bug>();
            dto2Bug.ForMember(b => b.Name, e => e.MapFrom(d => d.Title));
        }
    }
}
