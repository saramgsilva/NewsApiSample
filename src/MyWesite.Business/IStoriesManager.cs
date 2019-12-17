using MyWebsite.Dtos;
using System.Threading.Tasks;

namespace MyWebsite.Business
{
    public interface IStoriesManager
    {
        Task<BestStoriesDto> GetBestStoriesAsync();
    }
}