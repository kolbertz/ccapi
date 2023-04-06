using CCUserService.Data;

namespace CCUserService.DTOs
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid UserGroupId { get; set; }
        public string DisplayName { get; set; }


        public User(InternalUser internalUser) 
        {
        
        }
        public User(InternalUser internalUser, Guid sysId)
        { }

    }
}
