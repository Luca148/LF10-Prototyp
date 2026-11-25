namespace LF10.Data.Entities;

public class FilterWordEntity
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public FilterCategoryEntity Category { get; set; }
    public string Language { get; set; }
    public string CurseWord { get; set; }
    public string FilterWord { get; set; }
}
