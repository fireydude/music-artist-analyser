using System.Threading.Tasks;

namespace AireLogic.ArtistData.Services
{
    public interface ILyricService
    {
         Task<string> GetLyrics(string artist, string track);
    }
}