using System.Threading.Tasks;

namespace AireLogic.ArtistData
{
    public interface IArtistAnalyser
    {
        Task<int?> Run(string[] args);
    }
}
