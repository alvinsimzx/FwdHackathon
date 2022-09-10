namespace FwdHackathon.Models
{
    public class Tweet
    {
        public string fullText { get; set; }
        public string Author { get; set; }

        public Tweet(string fulltext,string author)
        {
            fullText = fulltext;
            Author = author;
        }
    }
}
