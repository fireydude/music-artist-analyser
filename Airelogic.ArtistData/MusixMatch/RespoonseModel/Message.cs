namespace Airelogic.ArtistData.MusixMatch.RespoonseModel
{
    public class Message<T>
    {
        public Header Header { get; set; }
        public T Body { get; set; }
    }
}