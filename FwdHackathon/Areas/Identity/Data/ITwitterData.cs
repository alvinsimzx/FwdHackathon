using Refit;

namespace FwdHackathon.Areas.Identity.Data
{
  [Headers("Authorization: Bearer AAAAAAAAAAAAAAAAAAAAANqQgwEAAAAAzcfkIYoUAIjokrrVrRfAM7nOUyg%3DDgBVuqmoXkFG5MEbOb6A2UO0sipn7Uy6arPZC29YLHFhz0C2oR", "consumer_key: PfgyHVWL8HKdwE1MvXE8e3ACJ", "consumer_secret: QpIttrscjlgXYl4zBv588LbQ8nrwqJfyxiuqIiN3HYtefeIoQo", "access_token: 1407690580314923016-WCzqJVVG9k1YENj6CpAiLe9kexDIEp", "token_secret: 2wwvljH13EvIqVLeshzZ873JU3grWILP4OW0jRK2JzN0b")]
  public interface ITwitterData
  {
    [Get("/trends/place.json?id={id}")]
    Task<string> GetTweets(int id);

    [Get("/Monsters")]
    Task<IEnumerable<object>> GetMonsters();
  }
}
