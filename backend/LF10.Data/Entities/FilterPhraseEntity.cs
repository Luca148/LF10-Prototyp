namespace LF10.Data.Entities;

public class FilterPhraseEntity
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public FilterCategoryEntity Category { get; set; }
    public string Language { get; set; }
    public string CursePhrase { get; set; }
    public string FilterPhrase { get; set; }
}
