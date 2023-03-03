namespace CCCategoryService.Data
{
    public class Category
    {
        public Guid Id { get; set; }

        public int CategoryKey { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public Guid CreatedUser { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public Guid LastUpdateUser { get; set; }

        public Guid CategoryPoolId { get; set; }
       
    }
}
