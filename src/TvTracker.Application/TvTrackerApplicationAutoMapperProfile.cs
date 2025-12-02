using AutoMapper;
using TvTracker.Series;

namespace TvTracker;

public class TvTrackerApplicationAutoMapperProfile : Profile
{
    public TvTrackerApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Serie, SerieDto>();
        CreateMap<CreateUpdateSerieDto, Serie>();
    }
}
