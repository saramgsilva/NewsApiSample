using MyWebsite.Dtos;
using System.Threading.Tasks;

namespace MyWebsite.Business
{
    /// <summary>
    /// Defines the News API Manager interface
    /// </summary>
    public interface INewsApiManager
    {
        /// <summary>
        /// Gets the best stories asynchronous.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <returns>The best stories dto.</returns>
        Task<BestStoriesDto> GetBestStoriesAsync(int total);
    }
}
