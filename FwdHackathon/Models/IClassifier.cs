using Refit;

namespace FwdHackathon.Models
{
    public interface IClassifier
    {
        [Get("/topics/classify/?readKey=Pi3u4MimvqWb&text={text}")]
        Task<Dictionary<string,double>> GetMatches(string text);
    }
}
