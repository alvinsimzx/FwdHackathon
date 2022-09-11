namespace FwdHackathon.Models
{
  public class TransData
  {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Category { get; set; }
    public int Value { get; set; }
    public string Label { get; set; }

    public TransData(string category, int value, string label)
    {
      Category = category;
      Value = value;
      Label = label;
    }
  }
}
