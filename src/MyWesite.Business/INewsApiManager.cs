using MyWebsite.Dtos;
using System.Threading.Tasks;

namespace MyWebsite.Business
{
    public interface INewsApiManager
    {
        Task<BestStoriesDto> GetBestStoriesAsync(int total);
    }
}
