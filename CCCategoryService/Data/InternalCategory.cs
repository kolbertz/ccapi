using CCCategoryService.Dtos;

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

    public InternalCategory() { }

    public InternalCategory(Category category) {

        if (category != null)
        {
            Id = category.Id;
            CategoryKey = category.CategoryKey.Value;
            List<string> cultures = category.CategoryNames.Select(sn => sn.Culture).ToList();
            cultures.AddRange(category.Comments.Where(ln => !cultures.Contains(ln.Culture)).Select(ln => ln.Culture));
            cultures.AddRange(category.Descriptions.Where(ld => !cultures.Contains(ld.Culture)).Select(ld => ld.Culture));
            if (cultures != null && cultures.Count > 0)
            {
                foreach (string culture in cultures)
                {
                    CategoryStrings.Add(new InternalCategoryString
                    {
                        CategoryId = category.Id,
                        Culture = culture,
                        CategoryName = category.CategoryNames.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault(),
                        Comment = category.Comments.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault(),
                        Description = category.Descriptions.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault()
                    });
                }
            }

        }
    }
}
