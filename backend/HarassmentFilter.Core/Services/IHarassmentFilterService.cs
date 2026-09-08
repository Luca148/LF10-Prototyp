using HarassmentFilter.Core.Models;

namespace HarassmentFilter.Core.Services;

public interface IHarassmentFilterService
{
    FilterResult FilterMessage(string message);
}
