using MyWebsite.Dtos;
using System.Threading.Tasks;

namespace MyWebsite.Business
{
    /// <summary>
    /// Defines the Stories Manager interface
    /// </summary>
    public interface IStoriesManager
    {
        /// <summary>
        /// Gets the best stories asynchronous.
        /// </summary>  
        /// <returns>The best stories dto.</returns>
        Task<BestStoriesDto> GetBestStoriesAsync();
    }
}