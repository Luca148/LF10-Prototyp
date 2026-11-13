using HarassmentFilter.Core.Models;

namespace HarassmentFilter.Core.Interfaces;

public interface IFilterService
{
    FilterResult FilterMessage(string message);
}
