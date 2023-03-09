namespace CCCategoryService.Data;

public class InternalCategory
{
    public Guid Id { get; set; }

    public int CategoryKey { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public Guid CreatedUser { get; set; }

    public DateTimeOffset LastUpdatedDate { get; set; }

    public Guid LastUpdatedUser { get; set; }

    public Guid CategoryPoolId { get; set; }

    public virtual ICollection<InternalCategoryString> CategoryStrings { get; } = new List<InternalCategoryString>();

    public InternalCategory()
    {
        
    }
}
