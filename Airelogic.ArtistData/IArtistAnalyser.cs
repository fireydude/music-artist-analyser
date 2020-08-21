using System.Threading.Tasks;

namespace AireLogic.ArtistData
{
    public interface IArtistAnalyser
    {
        Task Run(string[] args);
    }
}
