using System.Collections.Generic;
using System.Threading.Tasks;

namespace AireLogic.ArtistData.Services
{
    public interface IMusicService
    {
         Task<IEnumerable<string>> FindArtistTracks(string artist);
    }
}